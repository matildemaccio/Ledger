using Ledger.Domain.Model;
using Ledger.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(LedgerContext context) : base(context) { }

        public Task<bool> ExistsAsync(Guid id)
        {
            return context.Transactions.AnyAsync(x => x.Id == id);
        }
    }
}
