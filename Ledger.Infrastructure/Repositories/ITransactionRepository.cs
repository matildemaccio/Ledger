using Ledger.Domain.Model;

namespace Ledger.Infrastructure.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<bool> ExistsAsync(Guid id);
    }
}
