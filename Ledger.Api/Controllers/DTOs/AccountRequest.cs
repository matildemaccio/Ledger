using Ledger.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Ledger.Api.Controllers.DTOs
{
    public class AccountRequest
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }

        [Required]
        public Direction Direction { get; set; }
    }
}
