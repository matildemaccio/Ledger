using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;

namespace Ledger.Application.Queries
{
    public static class GetAccount
    {
        public record Request(Guid Id) : IRequest<Response>;
        public record Response(Guid Id, string? Name, Direction Direction, decimal Balance);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IAccountRepository _accountRepository;
            public Handler(IAccountRepository accountRepository)
            {
                _accountRepository = accountRepository;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Account? account = await _accountRepository.GetByKeyAsync(request.Id);
                if (account == null)
                {
                    throw new EntityNotFoundException($"The account with ID '{request.Id}' was not found.");
                }
                return new Response(account.Id, account.Name, account.Direction, account.Balance);
            }
        }
    }
}
