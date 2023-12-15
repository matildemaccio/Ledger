using Ledger.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ledger.Domain.Model
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        [Column(TypeName = "decimal(16,0)")]
        public decimal Balance { get; set; }

        public Direction Direction { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; set; }

        public Account() { }

        public Account(Guid? id, string? name, Direction direction)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
            Balance = 0;
            Direction = direction;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }
    }
}