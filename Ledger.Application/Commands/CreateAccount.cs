using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ledger.Application.Commands
{
    public static class CreateAccount
    {
        public record Command(Guid? Id, string? Name, Direction Direction) : IRequest<Response>;
        public record Response(Guid Id, string? Name, Direction Direction, decimal Balance);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IAccountRepository _accountRepository;

            public Handler(IAccountRepository accountRepository)
            {
                _accountRepository = accountRepository;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                await ValidateAccountNotExists(command.Id);
                
                Account newAccount = new Account(command.Id, command.Name, command.Direction);
                _accountRepository.Add(newAccount);
                await _accountRepository.SaveChangesAsync();

                return new Response(newAccount.Id, newAccount.Name, newAccount.Direction, newAccount.Balance);
            }

            private async Task ValidateAccountNotExists(Guid? id)
            {
                if (id != null)
                {
                    bool accountExists = await _accountRepository.ExistsAsync(id.Value);
                    if (accountExists)
                    {
                        throw new EntityAlreadyExistsException($"Error: An account with the ID '{id}' already exists.");
                    }
                }
            }
        }

    }
}
