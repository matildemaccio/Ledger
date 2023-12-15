using Ledger.Api.Controllers.DTOs;
using Ledger.Application.Commands;
using Ledger.Application.Queries;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ledger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : BaseController
    {
        public AccountsController(IMediator mediator) : base(mediator)
        {

        }

        [HttpPost()]
        [Produces(typeof(AccountResponse))]
        [ProducesResponseType((int)HttpStatusCode.Created), ProducesResponseType((int)HttpStatusCode.BadRequest), ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<AccountResponse>> CreateAccount(AccountRequest request)
        {
            try
            {
                var command = new CreateAccount.Command(request.Id, request.Name, request.Direction);
                
                var createdAccount = await Mediator.Send(command);
                
                var accountResponse = new AccountResponse(createdAccount.Id, createdAccount.Name, createdAccount.Direction, createdAccount.Balance);
                return CreatedAtAction(nameof(CreateAccount), new { id = accountResponse.Id }, accountResponse);
            }
            catch (EntityAlreadyExistsException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.Conflict);
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(AccountResponse))]
        [ProducesResponseType((int)HttpStatusCode.OK), ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<AccountResponse>> GetAccount(Guid id)
        {
            try
            {
                var account = await Mediator.Send(new GetAccount.Request(id));
                var accountResponse = new AccountResponse(account.Id, account.Name, account.Direction, account.Balance);
                return Ok(accountResponse);
            }
            catch (EntityNotFoundException ex)
            {
                return Problem(ex.Message, statusCode: (int)HttpStatusCode.NotFound);
            }
        }

        [HttpGet()]
        [Produces(typeof(List<AccountResponse>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<AccountResponse>>> GetAllAccounts()
        {
            List<GetAllAccounts.Response> accounts = await Mediator.Send(new GetAllAccounts.Request());

            var accountsResponse = accounts.Select(account => new AccountResponse(account.Id, account.Name, account.Direction, account.Balance)).ToList();
            return Ok(accountsResponse);
        }
    }
}
