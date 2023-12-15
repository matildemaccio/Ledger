using FluentAssertions;
using Ledger.Application.Commands;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;

namespace Ledger.Tests.Commands
{
    [TestFixture]
    public class CreateTransactionTests : BaseTests
    {

        [Test]
        public async Task CreateTransaction_ShouldSuccess()
        {
            var debitAccount1 = InsertTestAccount(null, null, Direction.Debit, 100);
            var creditAccount1 = InsertTestAccount(null, null, Direction.Credit, 100);
            var debitAccount2 = InsertTestAccount(null, null, Direction.Debit, 100);
            var creditAccount2 = InsertTestAccount(null, null, Direction.Credit, 100);

            var entry1 = new CreateTransaction.CommandEntry(null, debitAccount1.Id, 100, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(null, creditAccount1.Id, 100, Direction.Credit);
            var entry3 = new CreateTransaction.CommandEntry(null, debitAccount2.Id, 100, Direction.Credit);
            var entry4 = new CreateTransaction.CommandEntry(null, creditAccount2.Id, 100, Direction.Debit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2, entry3, entry4 };
            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), "transaction1", entries);
            var transaction = await mediator.Send(command);

            Assert.NotNull(transaction);
            Assert.That(transaction.Name, Is.EqualTo(command.Name));
            Assert.That(transaction.Entries.Count, Is.EqualTo(4));
            Assert.That(transaction.Entries.Any(x => x.AccountId == debitAccount1.Id));
            Assert.That(transaction.Entries.Any(x => x.AccountId == creditAccount1.Id));
            Assert.That(transaction.Entries.Any(x => x.AccountId == debitAccount2.Id));
            Assert.That(transaction.Entries.Any(x => x.AccountId == creditAccount2.Id));

            var updatedDebitAccount1 = context.Accounts.First(x => x.Id == debitAccount1.Id);
            Assert.That(updatedDebitAccount1.Balance, Is.EqualTo(200));

            var updatedCreditAccount1 = context.Accounts.First(x => x.Id == creditAccount1.Id);
            Assert.That(updatedCreditAccount1.Balance, Is.EqualTo(200));

            var updatedDebitAccount2 = context.Accounts.First(x => x.Id == debitAccount2.Id);
            Assert.That(updatedDebitAccount2.Balance, Is.EqualTo(0));

            var updatedCreditAccount2 = context.Accounts.First(x => x.Id == creditAccount2.Id);
            Assert.That(updatedCreditAccount2.Balance, Is.EqualTo(0));
        }

        [Test]
        public async Task CreateTransaction_WhenAmountGreaterThanMaxInt_ShouldSuccess()
        {
            var debitAccount1 = InsertTestAccount(null, null, Direction.Debit, Int32.MaxValue);
            var creditAccount1 = InsertTestAccount(null, null, Direction.Credit, Int32.MaxValue);

            var entry1 = new CreateTransaction.CommandEntry(null, debitAccount1.Id, Int32.MaxValue, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(null, creditAccount1.Id, Int32.MaxValue, Direction.Credit);

            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2 };
            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), "transaction1", entries);
            var transaction = await mediator.Send(command);

            Assert.NotNull(transaction);
            Assert.That(transaction.Name, Is.EqualTo(command.Name));
            Assert.That(transaction.Entries.Count, Is.EqualTo(2));
            Assert.That(transaction.Entries.Any(x => x.AccountId == debitAccount1.Id));
            Assert.That(transaction.Entries.Any(x => x.AccountId == creditAccount1.Id));

            var updatedDebitAccount1 = context.Accounts.First(x => x.Id == debitAccount1.Id);
            Assert.That(updatedDebitAccount1.Balance, Is.EqualTo((decimal)2 * Int32.MaxValue));

            var updatedCreditAccount1 = context.Accounts.First(x => x.Id == creditAccount1.Id);
            Assert.That(updatedCreditAccount1.Balance, Is.EqualTo((decimal)2 * Int32.MaxValue));
        }

        [Test]
        public async Task CreateTransaction_WithEntriesOverSameAccount_ShouldSuccess()
        {
            var debitAccount1 = InsertTestAccount(null, null, Direction.Debit, 100);

            var entry1 = new CreateTransaction.CommandEntry(null, debitAccount1.Id, 1000, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(null, debitAccount1.Id, 1000, Direction.Credit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), "transaction1", entries);
            var transaction = await mediator.Send(command);

            Assert.NotNull(transaction);
            Assert.That(transaction.Name, Is.EqualTo(command.Name));
            Assert.That(transaction.Entries.Count, Is.EqualTo(2));
            Assert.That(transaction.Entries.All(x => x.AccountId == debitAccount1.Id));

            var updatedDebitAccount1 = context.Accounts.First(x => x.Id == debitAccount1.Id);
            Assert.That(updatedDebitAccount1.Balance, Is.EqualTo(100));
        }

        [Test]
        public async Task CreateTransaction_WhenNoEntries_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);
            var entries = new List<CreateTransaction.CommandEntry>() { };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), null, entries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<InvalidEntriesDirectionsException>();
        }

        [Test]
        public async Task CreateTransaction_WhenInvalidEntryDirections_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);

            var entry = new CreateTransaction.CommandEntry(null, debitAccount.Id, 100, Direction.Debit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), null, entries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<InvalidEntriesDirectionsException>();
        }

        [Test]
        public async Task CreateTransaction_WhenInvalidEntryAmounts_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 0);

            var entry1 = new CreateTransaction.CommandEntry(null, debitAccount.Id, -100, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Credit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), null, entries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<InvalidEntriesAmountException>();
        }

        [Test]
        public async Task CreateTransaction_WhenEntriesUnbalanced_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 0);

            var entry1 = new CreateTransaction.CommandEntry(null, debitAccount.Id, 200, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Credit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), null, entries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<UnbalancedTransactionException>();
        }

        [Test]
        public async Task CreateTransaction_WhenDuplicatedEntryIds_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 0);

            var entryId = Guid.NewGuid();
            var entry1 = new CreateTransaction.CommandEntry(entryId, debitAccount.Id, 100, Direction.Debit);
            var entry2 = new CreateTransaction.CommandEntry(entryId, creditAccount.Id, 100, Direction.Credit);
            var entries = new List<CreateTransaction.CommandEntry>() { entry1, entry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(Guid.NewGuid(), null, entries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<DuplicatedEntryIdsException>();
        }

        [Test]
        public async Task CreateTransaction_WhenTransactionAlreadyExists_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 0);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 0);
            var entries = new List<Entry>();
            var transaction = InsertTestTransaction(null, null, entries);

            var commandEntry1 = new CreateTransaction.CommandEntry(null, debitAccount.Id, 100, Direction.Debit);
            var commandEntry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Credit);
            var commandEntries = new List<CreateTransaction.CommandEntry>() { commandEntry1, commandEntry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(transaction.Id, null, commandEntries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Test]
        public async Task CreateTransaction_WhenEntriesAlreadyExist_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 10);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 10);
            var entry1 = new Entry(null, debitAccount.Id, 10, Direction.Debit);
            var entry2 = new Entry(null, creditAccount.Id, 10, Direction.Credit);
            var entries = new List<Entry>() { entry1, entry2 };
            var transaction = InsertTestTransaction(null, null, entries);

            var commandEntry1 = new CreateTransaction.CommandEntry(entry1.Id, debitAccount.Id, 100, Direction.Debit);
            var commandEntry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Credit);
            var commandEntries = new List<CreateTransaction.CommandEntry>() { commandEntry1, commandEntry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(null, null, commandEntries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<EntityAlreadyExistsException>();
        }

        [Test]
        public async Task CreateTransaction_WhenAccountsNotFound_ShouldThrow()
        {
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 10);

            var commandEntry1 = new CreateTransaction.CommandEntry(null, Guid.NewGuid(), 100, Direction.Debit);
            var commandEntry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Credit);
            var commandEntries = new List<CreateTransaction.CommandEntry>() { commandEntry1, commandEntry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(null, null, commandEntries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<EntityNotFoundException>();
        }

        [Test]
        public async Task CreateTransaction_WhenAccountInsufficientFunds_ShouldThrow()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 100);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 0);

            var commandEntry1 = new CreateTransaction.CommandEntry(null, debitAccount.Id, 100, Direction.Credit);
            var commandEntry2 = new CreateTransaction.CommandEntry(null, creditAccount.Id, 100, Direction.Debit);
            var commandEntries = new List<CreateTransaction.CommandEntry>() { commandEntry1, commandEntry2 };

            CreateTransaction.Command command = new CreateTransaction.Command(null, null, commandEntries);

            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<InsufficientFundsException>();
        }
    }
}
