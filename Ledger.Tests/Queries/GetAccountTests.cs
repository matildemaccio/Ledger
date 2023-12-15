using FluentAssertions;
using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;

namespace Ledger.Tests.Queries
{
    public class GetAccountTests : BaseTests
    {
        [Test]
        public async Task GetExistingAccount_ShouldSuccess()
        {
            var account = InsertTestAccount(null, null, Direction.Debit, 0);
            
            GetAccount.Request request = new GetAccount.Request(account.Id);
            var response = await mediator.Send(request);

            Assert.NotNull(response);
            Assert.That(response.Name, Is.EqualTo(account.Name));
            Assert.That(response.Direction, Is.EqualTo(account.Direction));
            Assert.That(response.Balance, Is.EqualTo(account.Balance));
            Assert.That(response.Id, Is.EqualTo(account.Id));
        }

        [Test]
        public async Task GetAccount_WhenNotFound_ShouldThrow()
        {
            GetAccount.Request request = new GetAccount.Request(Guid.NewGuid());
            await mediator.Awaiting(x => x.Send(request)).Should().ThrowAsync<EntityNotFoundException>();
        }
    }
}
