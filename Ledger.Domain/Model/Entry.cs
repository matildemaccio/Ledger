using Ledger.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Model
{
    public class Entry
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Account")]
        public Guid AccountId { get; set; }

        [ForeignKey("Transaction")]
        public Guid TransactionId { get; set; }

        [Column(TypeName = "decimal(16,0)")]
        public decimal Amount { get; set; }

        public Direction Direction { get; set; }
        public DateTime CreatedAt { get; private set; }

        public Account Account { get; private set; }
        public Transaction Transaction { get; private set; }

        public Entry() { }

        public Entry(Guid? id, Guid accountId, decimal amount, Direction direction)
        {
            Id = id ?? Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Direction = direction;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
