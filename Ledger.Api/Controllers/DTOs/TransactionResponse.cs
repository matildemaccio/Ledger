using Ledger.Domain.Enums;
using System.Text.Json.Serialization;

namespace Ledger.Api.Controllers.DTOs
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public EntryResponse[] Entries { get; set; }

        public TransactionResponse(Guid id, string? name, EntryResponse[] entries)
        {
            Id = id;
            Name = name;
            Entries = entries;
        }
    }

    public class EntryResponse

    {
        [JsonPropertyName("account_id")]
        public Guid Account_Id { get; set; }
        public decimal Amount { get; set; }
        public Direction Direction { get; set; }
        public Guid Id { get; set; }

        public EntryResponse(Guid id, Guid accountId, decimal amount, Direction direction)
        {
            Id = id;
            Account_Id = accountId;
            Amount = amount;
            Direction = direction;
        }
    }
}
