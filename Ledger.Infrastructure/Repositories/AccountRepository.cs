using Ledger.Domain.Model;
using Ledger.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(LedgerContext context) : base(context) { }

        public Task<bool> ExistsAsync(Guid id)
        {
            return context.Accounts.AnyAsync(account => account.Id == id);
        }

        public Task<Dictionary<Guid, Account>> GetAllByIdAsync(List<Guid> ids)
        {
            return context.Accounts.Where(account => ids.Contains(account.Id)).ToDictionaryAsync(account => account.Id);
        }
    }
}
