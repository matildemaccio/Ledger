using Ledger.Domain.Model;

namespace Ledger.Infrastructure.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> ExistsAsync(Guid id);
        Task<Dictionary<Guid, Account>> GetAllByIdAsync(List<Guid> ids);
    }
}
