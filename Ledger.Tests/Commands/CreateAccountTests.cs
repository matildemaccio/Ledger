using FluentAssertions;
using Ledger.Application.Commands;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using System.Xml.Linq;

namespace Ledger.Tests.Commands
{
    public class CreateAccountTests : BaseTests
    {
        [Test]
        public async Task CreateAccount_ShouldSuccess()
        {
            CreateAccount.Command command = new CreateAccount.Command(null, "accountName", Direction.Debit);

            var account = await mediator.Send(command);

            Assert.NotNull(account);
            Assert.That(account.Name, Is.EqualTo(command.Name));
            Assert.That(account.Direction, Is.EqualTo(command.Direction));
            Assert.That(account.Balance, Is.EqualTo(0));
            Assert.IsNotNull(account.Id);
        }

        [Test]
        public async Task CreateAccount_WhenAccountAlreadyExists_ShouldThrow()
        {
            var account = InsertTestAccount(null, null, Direction.Debit, 0);
            
            CreateAccount.Command command = new CreateAccount.Command(account.Id, null, Direction.Debit);
            await mediator.Awaiting(x => x.Send(command)).Should().ThrowAsync<EntityAlreadyExistsException>();
        }
    }
}
