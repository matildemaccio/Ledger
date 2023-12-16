using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;
using System.Data;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Ledger.Application.Commands
{
    public static class CreateTransaction
    {
        public record Command(Guid? Id, string? Name, List<CommandEntry> Entries) : IRequest<Response>;
        public record CommandEntry(Guid? Id, Guid AccountId, decimal Amount, Direction Direction);
        public record Response(Guid Id, string? Name, List<ResponseEntry> Entries);
        public record ResponseEntry(Guid AccountId, decimal Amount, Direction Direction, Guid Id);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IAccountRepository _accountRepository;
            private readonly ITransactionRepository _transactionRepository;
            private readonly IEntryRepository _entryRepository;
            public Handler(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IEntryRepository entryRepository)
            {
                _accountRepository = accountRepository;
                _transactionRepository = transactionRepository;
                _entryRepository = entryRepository;
            }
            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                ValidateEntryDirections(command.Entries);
                ValidateEntryAmounts(command.Entries);
                ValidateEntriesBalanced(command.Entries);
                ValidateDuplicatedEntryIds(command.Entries);
                await ValidateTransactionNotExists(command.Id);
                await ValidateEntriesNotExist(command.Entries);

                List<Guid> commandEntryAccountIds = command.Entries.Select(x => x.AccountId).Distinct().ToList();
                Dictionary<Guid, Account> accounts = await _accountRepository.GetAllByIdAsync(commandEntryAccountIds);
                ValidateAccountsExist(commandEntryAccountIds, accounts);

                foreach (var entry in command.Entries)
                {
                    var account = accounts[entry.AccountId];
                    if (entry.Direction == account.Direction)
                    {
                        account.Balance += entry.Amount;
                    }
                    else
                    {
                        account.Balance -= entry.Amount;
                    }
                    account.UpdatedAt = DateTime.UtcNow;
                }
                ValidateAccountBalance(accounts);

                var newEntries = command.Entries.Select(entry => new Entry(entry.Id, entry.AccountId, entry.Amount, entry.Direction)).ToList();
                Transaction newTransaction = new Transaction(command.Id, command.Name, newEntries);
                _transactionRepository.Add(newTransaction);
                await _transactionRepository.SaveChangesAsync();

                var responseEntries = newTransaction.Entries.Select(entry => new ResponseEntry(entry.AccountId, entry.Amount, entry.Direction, entry.Id)).ToList();
                return new Response(newTransaction.Id, newTransaction.Name, responseEntries);
            }

            private void ValidateEntryDirections(List<CommandEntry> entries)
            {
                if (!entries.Any(entry => entry.Direction == Direction.Debit)
                    || !entries.Any(entry => entry.Direction == Direction.Credit))
                {
                    throw new InvalidEntriesDirectionsException($"Error: The transaction must have at least two entries, where the sum of all the debits must equal the sum of all the credits.");
                }
            }

            private void ValidateEntryAmounts(List<CommandEntry> entries)
            {
                if (entries.Any(x => x.Amount <= 0))
                {
                    throw new InvalidEntriesAmountException($"Error: Transaction entries must have amounts greater than zero. Please ensure that the amounts entered are positive values.");
                }
            }

            private void ValidateEntriesBalanced(List<CommandEntry> entries)
            {
                decimal totalDebits = entries.Where(entry => entry.Direction == Direction.Debit).Sum(entry => entry.Amount);
                decimal totalCredits = entries.Where(entry => entry.Direction == Direction.Credit).Sum(entry => entry.Amount);
                if (totalDebits != totalCredits)
                {
                    throw new UnbalancedTransactionException($"Error: The sum of all the debits must equal the sum of all the credits in the transaction.");
                }
            }

            private void ValidateDuplicatedEntryIds(List<CommandEntry> entries)
            {
                var existDuplicatedEntryIds = entries.Where(entry => entry.Id != null).GroupBy(x => x.Id).Any(x => x.Count() > 1);
                if (existDuplicatedEntryIds)
                {
                    throw new DuplicatedEntryIdsException($"Error: Duplicate IDs found in the Entries collection.");
                }
            }

            private async Task ValidateTransactionNotExists(Guid? id)
            {
                if (id != null)
                {
                    bool transactionExists = await _transactionRepository.ExistsAsync(id.Value);
                    if (transactionExists)
                    {
                        throw new EntityAlreadyExistsException($"Error: A transaction with the ID '{id}' already exists.");
                    }
                }
            }

            private async Task ValidateEntriesNotExist(List<CommandEntry> entries)
            {
                List<Guid> commandEntryIds = entries.Where(entry => entry.Id != null).Select(entry => entry.Id!.Value).ToList();
                if (commandEntryIds.Count > 0)
                {
                    bool entriesExist = await _entryRepository.ExistAsync(commandEntryIds);
                    if (entriesExist)
                    {
                        throw new EntityAlreadyExistsException($"Error: An entry with a duplicated ID already exists.");
                    }
                }
            }

            private void ValidateAccountsExist(List<Guid> accountIds, Dictionary<Guid, Account> existingAccounts)
            {
                var nonExistantAccountIds = accountIds.Where(accountId => !existingAccounts.ContainsKey(accountId)).ToList();
                if (nonExistantAccountIds.Count > 0)
                {
                    throw new EntityNotFoundException($"Error: The following accounts do not exist: {string.Join(", ", nonExistantAccountIds)}");
                }
            }

            private void ValidateAccountBalance(Dictionary<Guid, Account> accounts)
            {
                foreach (var account in accounts.Values)
                {
                    if (account.Balance < 0)
                    {
                        throw new InsufficientFundsException($"Error: The account with the ID '{account.Id}' does not have sufficient funds.");
                    }
                }
            }
        }
    }
}
