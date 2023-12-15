namespace Ledger.Domain.Exceptions
{
    public class InvalidRequestParametersException : Exception
    {
        public InvalidRequestParametersException(string message) : base(message)
        {
        }
    }
}
