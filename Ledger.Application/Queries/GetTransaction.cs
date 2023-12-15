using Ledger.Domain.Enums;
using Ledger.Domain.Exceptions;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.Queries
{
    public static class GetTransaction
    {
        public record Request(Guid Id) : IRequest<Response>;
        public record Response(Guid Id, string? Name, List<ResponseEntry> Entries);
        public record ResponseEntry(Guid AccountId, decimal Amount, Direction Direction, Guid Id);
        
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ITransactionRepository _transactionRepository;
            public Handler(ITransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository;
            }
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                Transaction? transaction = await _transactionRepository.GetByKeyAsync(request.Id);
                if (transaction == null)
                {
                    throw new EntityNotFoundException($"The transaction with ID '{request.Id}' was not found.");
                }
                var entries = transaction.Entries.Select(entry => new ResponseEntry(entry.AccountId, entry.Amount, entry.Direction, entry.Id)).ToList();
                return new Response(transaction.Id, transaction.Name, entries);
            }
        }   
    }
}
