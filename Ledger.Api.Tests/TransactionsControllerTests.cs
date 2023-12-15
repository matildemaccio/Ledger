using Ledger.Api.Controllers.DTOs;
using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ledger.Api.Tests
{
    public class TransactionsControllerTests : BaseControllerTests
    {
        [Test]
        public async Task CreateTransaction_WhenValidRequest_ReturnsCreatedTransaction()
        {
            var mockResponseEntry1 = new CreateTransaction.ResponseEntry(Guid.NewGuid(), 100, Direction.Credit, Guid.NewGuid());
            var mockResponseEntry2 = new CreateTransaction.ResponseEntry(Guid.NewGuid(), 100, Direction.Debit, Guid.NewGuid());
            var mockResponseEntries = new List<CreateTransaction.ResponseEntry>() { mockResponseEntry1, mockResponseEntry2 };
            var mockCommandResponse = new CreateTransaction.Response(Guid.NewGuid(), "accountName", mockResponseEntries);

            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockCommandResponse);

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = mockCommandResponse.Id,
                Name = mockCommandResponse.Name,
                Entries = new EntryRequest[]
                {
                    new EntryRequest()
                    {
                        Id = mockResponseEntry1.Id,
                        AccountId = mockResponseEntry1.AccountId,
                        Amount = mockResponseEntry1.Amount,
                        Direction = mockResponseEntry1.Direction
                    },
                    new EntryRequest()
                    {
                        Id = mockResponseEntry2.Id,
                        AccountId = mockResponseEntry2.AccountId,
                        Amount = mockResponseEntry2.Amount,
                        Direction = mockResponseEntry2.Direction
                    }
                }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);

            Assert.That(result.StatusCode, Is.EqualTo(201));
            var transactionResponse = result.Value as TransactionResponse;
            Assert.IsNotNull(transactionResponse);
            Assert.That(transactionResponse.Id, Is.EqualTo(mockCommandResponse.Id));
            Assert.That(transactionResponse.Name, Is.EqualTo(mockCommandResponse.Name));
            Assert.That(transactionResponse.Entries.Count, Is.EqualTo(mockCommandResponse.Entries.Count));

            Assert.That(transactionResponse.Entries[0].Id, Is.EqualTo(mockCommandResponse.Entries[0].Id));
            Assert.That(transactionResponse.Entries[0].Direction, Is.EqualTo(mockCommandResponse.Entries[0].Direction));
            Assert.That(transactionResponse.Entries[0].Amount, Is.EqualTo(mockCommandResponse.Entries[0].Amount));

            Assert.That(transactionResponse.Entries[1].Id, Is.EqualTo(mockCommandResponse.Entries[1].Id));
            Assert.That(transactionResponse.Entries[1].Direction, Is.EqualTo(mockCommandResponse.Entries[1].Direction));
            Assert.That(transactionResponse.Entries[1].Amount, Is.EqualTo(mockCommandResponse.Entries[1].Amount));

        }

        [Test]
        public async Task CreateTransaction_WhenInvalidEntryDirections_ReturnsBadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidEntriesDirectionsException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateTransaction_WhenInvalidEntryAmounts_ReturnsBadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidEntriesAmountException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateTransaction_WhenUnbalancedTransation_ReturnsBadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new UnbalancedTransactionException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateTransaction_WhenDuplicatedEntryIds_ReturnsBadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new DuplicatedEntryIdsException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateTransaction_WhenEntityAlreadyExists_ReturnsConflict()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new EntityAlreadyExistsException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(409));
        }

        [Test]
        public async Task CreateTransaction_WhenAccountNotFound_ReturnsNotFound()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new EntityNotFoundException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task CreateTransaction_WhenInsufficientFunds_ReturnsBadRequest()
        {
            mediator.Setup(x => x.Send(It.IsAny<CreateTransaction.Command>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InsufficientFundsException(""));

            var actionResult = await transactionsController.CreateTransaction(new TransactionRequest
            {
                Id = Guid.NewGuid(),
                Entries = new EntryRequest[] { }
            });

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetTransaction_WhenValidRequest_ReturnsOk()
        {
            var mockResponseEntry1 = new GetTransaction.ResponseEntry(Guid.NewGuid(), 100, Direction.Credit, Guid.NewGuid());
            var mockResponseEntry2 = new GetTransaction.ResponseEntry(Guid.NewGuid(), 100, Direction.Debit, Guid.NewGuid());
            var mockResponseEntries = new List<GetTransaction.ResponseEntry>() { mockResponseEntry1, mockResponseEntry2 };
            var mockQueryResponse = new GetTransaction.Response(Guid.NewGuid(), "t1", mockResponseEntries);
            mediator.Setup(x => x.Send(It.IsAny<GetTransaction.Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockQueryResponse);

            var actionResult = await transactionsController.GetTransaction(mockQueryResponse.Id);

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var transactionResult = result.Value as TransactionResponse;
            Assert.IsNotNull(transactionResult);
            Assert.That(transactionResult.Id, Is.EqualTo(mockQueryResponse.Id));
            Assert.That(transactionResult.Name, Is.EqualTo(mockQueryResponse.Name));
            Assert.That(transactionResult.Entries.Count, Is.EqualTo(mockQueryResponse.Entries.Count));

            Assert.That(transactionResult.Entries[0].Id, Is.EqualTo(mockQueryResponse.Entries[0].Id));
            Assert.That(transactionResult.Entries[0].Direction, Is.EqualTo(mockQueryResponse.Entries[0].Direction));
            Assert.That(transactionResult.Entries[0].Amount, Is.EqualTo(mockQueryResponse.Entries[0].Amount));

            Assert.That(transactionResult.Entries[1].Id, Is.EqualTo(mockQueryResponse.Entries[1].Id));
            Assert.That(transactionResult.Entries[1].Direction, Is.EqualTo(mockQueryResponse.Entries[1].Direction));
            Assert.That(transactionResult.Entries[1].Amount, Is.EqualTo(mockQueryResponse.Entries[1].Amount));
        }

        [Test]
        public async Task GetTransaction_WhenNotFound_ReturnsNotFound()
        {
            mediator.Setup(x => x.Send(It.IsAny<GetTransaction.Request>(), It.IsAny<CancellationToken>())).ThrowsAsync(new EntityNotFoundException(""));

            var actionResult = await transactionsController.GetTransaction(Guid.NewGuid());

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllTransactions_ReturnsOk()
        {
            var mockResponseEntry1 = new GetAllTransactions.ResponseEntry(Guid.NewGuid(), 100, Direction.Credit, Guid.NewGuid());
            var mockResponseEntry2 = new GetAllTransactions.ResponseEntry(Guid.NewGuid(), 100, Direction.Debit, Guid.NewGuid());
            var mockResponseEntries = new List<GetAllTransactions.ResponseEntry>() { mockResponseEntry1, mockResponseEntry2 };
            var mockQueryResponse = new List<GetAllTransactions.Response>() { new GetAllTransactions.Response(Guid.NewGuid(), "t1", mockResponseEntries) };
            mediator.Setup(x => x.Send(It.IsAny<GetAllTransactions.Request>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockQueryResponse);

            var actionResult = await transactionsController.GetAllTransactions();

            Assert.IsNotNull(actionResult);
            var result = actionResult.Result as ObjectResult;
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var transactionResult = result.Value as List<TransactionResponse>;

            Assert.IsNotNull(transactionResult);
            Assert.That(transactionResult.Count, Is.EqualTo(1));
            Assert.That(transactionResult[0].Id, Is.EqualTo(mockQueryResponse[0].Id));
            Assert.That(transactionResult[0].Name, Is.EqualTo(mockQueryResponse[0].Name));
            Assert.That(transactionResult[0].Entries.Length, Is.EqualTo(mockQueryResponse[0].Entries.Count));

            Assert.That(transactionResult[0].Entries[0].Id, Is.EqualTo(mockResponseEntry1.Id));
            Assert.That(transactionResult[0].Entries[0].Direction, Is.EqualTo(mockResponseEntry1.Direction));
            Assert.That(transactionResult[0].Entries[0].Amount, Is.EqualTo(mockResponseEntry1.Amount));

            Assert.That(transactionResult[0].Entries[1].Id, Is.EqualTo(mockResponseEntry2.Id));
            Assert.That(transactionResult[0].Entries[1].Direction, Is.EqualTo(mockResponseEntry2.Direction));
            Assert.That(transactionResult[0].Entries[1].Amount, Is.EqualTo(mockResponseEntry2.Amount));
        }
    }
}
