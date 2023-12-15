using Ledger.Application.Queries;
using Ledger.Domain.Enums;
using Ledger.Domain.Model;
using Ledger.Tests;

namespace Ledger.Application.Tests.Queries
{
    public class GetAllTransactionTests : BaseTests
    {
        [Test]
        public async Task GetAllTransaction_ShouldSuccess()
        {
            var debitAccount = InsertTestAccount(null, null, Direction.Debit, 15);
            var creditAccount = InsertTestAccount(null, null, Direction.Credit, 15);

            var entry1 = new Entry(null, debitAccount.Id, 10, Direction.Debit);
            var entry2 = new Entry(null, creditAccount.Id, 10, Direction.Credit);
            var entries1 = new List<Entry>() { entry1, entry2 };
            var transaction1 = InsertTestTransaction(null, "t1", entries1);

            var entry3 = new Entry(null, debitAccount.Id, 10, Direction.Debit);
            var entry4 = new Entry(null, creditAccount.Id, 10, Direction.Credit);
            var entry5 = new Entry(null, debitAccount.Id, 5, Direction.Credit);
            var entry6 = new Entry(null, creditAccount.Id, 5, Direction.Debit);
            var entries2 = new List<Entry>() { entry3, entry4, entry5, entry6 };
            var transaction2 = InsertTestTransaction(null, "t2", entries2);

            var result = await mediator.Send(new GetAllTransactions.Request());

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(transaction1.Id));
            Assert.That(result[0].Name, Is.EqualTo(transaction1.Name));
            Assert.That(result[0].Entries.Count, Is.EqualTo(2));

            Assert.That(result[1].Id, Is.EqualTo(transaction2.Id));
            Assert.That(result[1].Name, Is.EqualTo(transaction2.Name));
            Assert.That(result[1].Entries.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task GetAllTransactions_WhenNoTransactions_ShouldReturnEmpty()
        {
            var request = new GetAllTransactions.Request();
            var response = await mediator.Send(request);
            Assert.NotNull(response);
            Assert.That(response.Count(), Is.EqualTo(0));
        }
    }
}
