using Ledger.Domain.Model;
using Ledger.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories
{
    public class EntryRepository : Repository<Entry>, IEntryRepository
    {
        public EntryRepository(LedgerContext context) : base(context) { }
        public Task<bool> ExistAsync(List<Guid> ids)
        {
            return context.Entries.AnyAsync(entry => ids.Contains(entry.Id));
        }
    }
}
