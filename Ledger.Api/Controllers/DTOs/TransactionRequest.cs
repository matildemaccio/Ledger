using Ledger.Domain.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ledger.Api.Controllers.DTOs
{
    public class TransactionRequest
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public EntryRequest[] Entries { get; set; }
    }

    public class EntryRequest
    {
        public Guid? Id { get; set; }
        [Required]
        [JsonPropertyName("account_id")]
        public Guid AccountId { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Entry amount must be an integer.")]
        public decimal Amount { get; set; }
        [Required]
        public Direction Direction { get; set; }
    }
}
