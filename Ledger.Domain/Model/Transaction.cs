using Ledger.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Model
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; private set; }

        public List<Entry> Entries { get; set; }

        public Transaction() {}
        public Transaction(Guid? id, string? name, List<Entry> entries)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
            Entries = entries;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
