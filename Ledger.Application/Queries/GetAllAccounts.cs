using Ledger.Domain.Enums;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;

namespace Ledger.Application.Queries
{
    public static class GetAllAccounts
    {
        public record Request() : IRequest<List<Response>>;
        public record Response(Guid Id, string? Name, Direction Direction, decimal Balance);

        public class Handler : IRequestHandler<Request, List<Response>>
        {
            private readonly IAccountRepository _accountRepository;
            public Handler(IAccountRepository accountRepository)
            {
                _accountRepository = accountRepository;
            }
            public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                // In a real world scenario I would do pagination here.
                List<Account> accounts = await _accountRepository.GetAllAsync();
                return accounts.Select(account => new Response(account.Id, account.Name, account.Direction, account.Balance)).ToList();
            }
        }
    }
}
