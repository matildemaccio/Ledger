using Ledger.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Infrastructure.Database
{
    public class LedgerContext : DbContext
    {
        public LedgerContext(DbContextOptions<LedgerContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
