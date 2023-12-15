using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Enums;

namespace Ledger.Tests.Queries
{
    public class GetAllAccountsTests : BaseTests
    {
        [Test]
        public async Task GetAllAccounts_ShouldSuccess()
        {
            var account1 = InsertTestAccount(null, null, Direction.Debit, 0);
            var account2 = InsertTestAccount(null, null, Direction.Debit, 0);
            var account3 = InsertTestAccount(null, null, Direction.Debit, 0);

            GetAllAccounts.Request request = new GetAllAccounts.Request();
            var response = await mediator.Send(request);

            Assert.NotNull(response);
            Assert.That(response.Count, Is.EqualTo(3));
            Assert.IsTrue(response.Any(x => x.Id == account1.Id));
            Assert.IsTrue(response.Any(x => x.Id == account2.Id));
            Assert.IsTrue(response.Any(x => x.Id == account3.Id));
        }

        [Test]
        public async Task GetAllAccounts_WhenNoAccounts_ShouldReturnEmpty()
        {
            var request = new GetAllAccounts.Request();
            var response = await mediator.Send(request);
            Assert.NotNull(response);
            Assert.That(response.Count(), Is.EqualTo(0));
        }
    }
}
