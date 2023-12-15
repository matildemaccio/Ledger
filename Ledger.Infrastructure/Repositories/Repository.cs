using Ledger.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly LedgerContext context;

        public Repository(LedgerContext context)
        {
            this.context = context;
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }
        
        public async Task<T?> GetByKeyAsync<TKey>(TKey key)
        {
            return await context.Set<T>().FindAsync(key);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
