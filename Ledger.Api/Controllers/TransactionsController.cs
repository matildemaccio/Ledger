using Ledger.Api.Controllers.DTOs;
using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ledger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : BaseController
    {
        public TransactionsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost()]
        [Produces(typeof(TransactionResponse))]
        [ProducesResponseType((int)HttpStatusCode.Created), ProducesResponseType((int)HttpStatusCode.BadRequest), ProducesResponseType((int)HttpStatusCode.Conflict), ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction(TransactionRequest request)
        {
            try
            {
                var commandEntries = request.Entries.Select(entry => new CreateTransaction.CommandEntry(entry.Id, entry.AccountId, entry.Amount, entry.Direction)).ToList();
                var command = new CreateTransaction.Command(request.Id, request.Name, commandEntries);

                var transaction = await Mediator.Send(command);

                var entriesResponse = transaction.Entries.Select(entry => new EntryResponse(entry.Id, entry.AccountId, entry.Amount, entry.Direction)).ToArray();
                var transactionResponse = new TransactionResponse(transaction.Id, transaction.Name, entriesResponse);
                return CreatedAtAction(nameof(CreateTransaction), new { id = transactionResponse.Id }, transactionResponse);
            }
            catch (InvalidEntriesDirectionsException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch (InvalidEntriesAmountException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch (UnbalancedTransactionException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch (DuplicatedEntryIdsException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
            catch (EntityAlreadyExistsException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.Conflict);
            }
            catch (EntityNotFoundException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.NotFound);
            }
            catch (InsufficientFundsException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(TransactionResponse))]
        [ProducesResponseType((int)HttpStatusCode.OK), ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TransactionResponse>> GetTransaction(Guid id)
        {
            try
            {
                var transaction = await Mediator.Send(new GetTransaction.Request(id));
                var entriesResponse = transaction.Entries.Select(entry => new EntryResponse(entry.Id, entry.AccountId, entry.Amount, entry.Direction)).ToArray();
                var transactionResponse = new TransactionResponse(transaction.Id, transaction.Name, entriesResponse);
                return Ok(transactionResponse);
            }
            catch (EntityNotFoundException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.NotFound);
            }
        }

        [HttpGet()]
        [Produces(typeof(List<TransactionResponse>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TransactionResponse>>> GetAllTransactions()
        {
            List<GetAllTransactions.Response> transactions = await Mediator.Send(new GetAllTransactions.Request());

            var transactionsResponse = new List<TransactionResponse>();
            foreach (var transaction in transactions)
            {
                var entriesResponse = transaction.Entries.Select(entry => new EntryResponse(entry.Id, entry.AccountId, entry.Amount, entry.Direction)).ToArray();
                var transactionResponse = new TransactionResponse(transaction.Id, transaction.Name, entriesResponse);
                transactionsResponse.Add(transactionResponse);
            }
            return Ok(transactionsResponse);
        }
    }
}
