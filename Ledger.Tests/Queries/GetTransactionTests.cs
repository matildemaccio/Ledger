using FluentAssertions;
using Ledger.Application.Queries;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using Ledger.Tests;

namespace Ledger.Application.Tests.Queries
{
    public class GetTransactionTests : BaseTests
    {
        [Test]
        public async Task GetTransaction_ShouldSuccess()
        {
            var debitAccount = InsertTestAccount(null, "debitAccount", Direction.Debit, 10);
            var creditAccount = InsertTestAccount(null, "creditAccount", Direction.Credit, 10);
            var entry1 = new Entry(null, debitAccount.Id, 10, Direction.Debit);
            var entry2 = new Entry(null, creditAccount.Id, 10, Direction.Credit);
            var entries = new List<Entry>() { entry1, entry2 };
            var transaction = InsertTestTransaction(null, "t1", entries);

            var result = await mediator.Send(new GetTransaction.Request(transaction.Id));
            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(transaction.Id));
            Assert.That(transaction.Name, Is.EqualTo(result.Name));
            Assert.That(transaction.Entries.Count, Is.EqualTo(2));
            Assert.That(transaction.Entries[0].AccountId, Is.EqualTo(entry1.AccountId));
            Assert.That(transaction.Entries[0].Amount, Is.EqualTo(entry1.Amount));
            Assert.That(transaction.Entries[0].Direction, Is.EqualTo(entry1.Direction));
            Assert.That(transaction.Entries[1].AccountId, Is.EqualTo(entry2.AccountId));
            Assert.That(transaction.Entries[1].Amount, Is.EqualTo(entry2.Amount));
            Assert.That(transaction.Entries[1].Direction, Is.EqualTo(entry2.Direction));
        }
        [Test]
        public async Task GetTransaction_WhenTransactionDoesNotExist_ShouldThrow()
        {
            await mediator.Awaiting(x => x.Send(new GetTransaction.Request(Guid.NewGuid()))).Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
