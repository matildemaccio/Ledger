namespace Ledger.Domain.Exceptions
{
    public class InvalidEntriesAmountException : Exception
    {
        public InvalidEntriesAmountException(string message) : base(message)
        {
        }
    }
}
