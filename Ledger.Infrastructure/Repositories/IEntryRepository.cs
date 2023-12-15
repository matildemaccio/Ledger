using Ledger.Domain.Model;

namespace Ledger.Infrastructure.Repositories
{
    public interface IEntryRepository : IRepository<Entry>
    {
        Task<bool> ExistAsync(List<Guid> ids);
    }
}
