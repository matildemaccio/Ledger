using Ledger.Domain.Enums;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;

namespace Ledger.Application.Queries
{
    public static class GetAllTransactions
    {
        public record Request() : IRequest<List<Response>>;
        public record Response(Guid Id, string? Name, List<ResponseEntry> Entries);
        public record ResponseEntry(Guid AccountId, decimal Amount, Direction Direction, Guid Id);
        public class Handler : IRequestHandler<Request, List<Response>>
        {
            private readonly ITransactionRepository _transactionRepository;
            public Handler(ITransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository;
            }
            public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                // In a real world scenario I would do pagination here.
                List<Transaction> transactions = await _transactionRepository.GetAllAsync();
                var transactionsResponse = new List<Response>();
                foreach (var transaction in transactions)
                {
                    var entries = transaction.Entries.Select(entry => new ResponseEntry(entry.AccountId, entry.Amount, entry.Direction, entry.Id)).ToList();
                    var response = new Response(transaction.Id, transaction.Name, entries);
                    transactionsResponse.Add(response);
                }
                return transactionsResponse;
            }
        }
    }
}
