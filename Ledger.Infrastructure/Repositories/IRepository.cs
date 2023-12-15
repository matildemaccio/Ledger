namespace Ledger.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task SaveChangesAsync();
        Task<T?> GetByKeyAsync<TKey>(TKey key);
        Task<List<T>> GetAllAsync();
    }
}