namespace Ledger.Domain.Exceptions
{
    public class DuplicatedEntryIdsException : Exception
    {
        public DuplicatedEntryIdsException(string message) : base(message)
        { 
        }
    }
}
