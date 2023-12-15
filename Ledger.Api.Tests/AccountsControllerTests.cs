using Ledger.Api.Controllers.DTOs;
using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ledger.Api.Tests
{
    public class AccountsControllerTests : BaseControllerTests
    {
        [Test]
        public async Task CreateAccount_WhenValidRequest_ReturnsCreatedAccount()
        {
            var mockCommandResponse = new CreateAccount.Response(Guid.NewGuid(), "accountName", Direction.Debit, 0);
            mediator.Setup(x => x.Send(It.IsAny<CreateAccount.Command>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockCommandResponse);

            var actionResult = await accountsController.CreateAccount(new AccountRequest
            {
                Id = mockCommandResponse.Id,
                Name = mockCommandResponse.Name,
                Direction = mockCommandResponse.Direction
            });

            mediator.Verify(x => x.Send(new CreateAccount.Command(mockCommandResponse.Id,
                mockCommandResponse.Name, mockCommandResponse.Direction), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(201));
            var accountResult = result.Value as AccountResponse;
            Assert.IsNotNull(accountResult);
            Assert.That(accountResult.Id, Is.EqualTo(mockCommandResponse.Id));
            Assert.That(accountResult.Name, Is.EqualTo(mockCommandResponse.Name));
            Assert.That(accountResult.Direction, Is.EqualTo(mockCommandResponse.Direction));
        }

        [Test]
        public async Task CreateAccount_WhenAccountAlreadyExists_ReturnsConflict()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateAccount.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new EntityAlreadyExistsException(""));

            var actionResult = await accountsController.CreateAccount(new AccountRequest
            {
                Id = Guid.NewGuid(),
                Name = "accountName",
                Direction = Direction.Debit
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(409));
        }

        [Test]
        public async Task GetAccount_WhenValidRequest_ReturnsOk()
        {
            var mockQueryResponse = new GetAccount.Response(Guid.NewGuid(), "accountName", Direction.Debit, 200);
            mediator.Setup(x => x.Send(It.IsAny<GetAccount.Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockQueryResponse);

            var actionResult = await accountsController.GetAccount(mockQueryResponse.Id);

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var accountResult = result.Value as AccountResponse;
            Assert.IsNotNull(accountResult);
            Assert.That(accountResult.Id, Is.EqualTo(mockQueryResponse.Id));
            Assert.That(accountResult.Name, Is.EqualTo(mockQueryResponse.Name));
            Assert.That(accountResult.Direction, Is.EqualTo(mockQueryResponse.Direction));
            Assert.That(accountResult.Balance, Is.EqualTo(mockQueryResponse.Balance));
        }

        [Test]
        public async Task GetAccount_WhenNotFound_ReturnsNotFound()
        {
            mediator.Setup(x => x.Send(It.IsAny<GetAccount.Request>(), It.IsAny<CancellationToken>())).ThrowsAsync(new EntityNotFoundException(""));

            var actionResult = await accountsController.GetAccount(Guid.NewGuid());

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllAccounts_ReturnsOk()
        {
            var mockAccount = new GetAllAccounts.Response(Guid.NewGuid(), "accountName", Direction.Debit, 200);
            var mockQueryResponse = new List<GetAllAccounts.Response>() { mockAccount };
            mediator.Setup(x => x.Send(It.IsAny<GetAllAccounts.Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockQueryResponse);

            var actionResult = await accountsController.GetAllAccounts();

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var accountResult = result.Value as List<AccountResponse>;
            Assert.IsNotNull(accountResult);
            Assert.That(accountResult.Count, Is.EqualTo(1));
            Assert.That(accountResult[0].Id, Is.EqualTo(mockAccount.Id));
            Assert.That(accountResult[0].Name, Is.EqualTo(mockAccount.Name));
            Assert.That(accountResult[0].Direction, Is.EqualTo(mockAccount.Direction));
            Assert.That(accountResult[0].Balance, Is.EqualTo(mockAccount.Balance));
        }
    }
}