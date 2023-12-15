using Ledger.Application.Commands;
using Ledger.Domain.Enums;
using Ledger.Domain.Model;
using Ledger.Infrastructure.Database;
using Ledger.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ledger.Tests
{
    public class BaseTests
    {
        protected IMediator mediator { get; set; }
        protected LedgerContext context;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateAccount).Assembly));
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddDbContext<LedgerContext>(options =>
            {
                options.UseInMemoryDatabase("Ledger");
            }, ServiceLifetime.Scoped);
            var provider = services.BuildServiceProvider();
            context = provider.GetRequiredService<LedgerContext>();
            mediator = provider.GetRequiredService<IMediator>();

            ClearContext();
        }

        public void ClearContext()
        {
            context.Entries.RemoveRange(context.Entries);
            context.Transactions.RemoveRange(context.Transactions);
            context.Accounts.RemoveRange(context.Accounts);
            context.SaveChanges();
        }

        public Account InsertTestAccount(Guid? id, string? name, Direction direction, decimal balance)
        {
            var account = new Account()
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Direction = direction,
                Balance = balance
            };
            context.Accounts.Add(account);
            context.SaveChanges();
            return account;
        }

        public Transaction InsertTestTransaction(Guid? id, string? name, List<Entry> entries)
        {
            var transaction = new Transaction()
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Entries = entries
            };
            context.Transactions.Add(transaction);
            context.SaveChanges();
            return transaction;
        }
    }
}
