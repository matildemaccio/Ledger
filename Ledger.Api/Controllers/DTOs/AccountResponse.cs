using Ledger.Domain.Enums;

namespace Ledger.Api.Controllers.DTOs
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Direction Direction { get; set; }
        public decimal Balance { get; set; }
        public AccountResponse(Guid id, string? name, Direction direction, decimal balance)
        {
            Id = id;
            Name = name;
            Direction = direction;
            Balance = balance;
        }
    }
}
