using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Exceptions
{
    public class UnbalancedTransactionException : Exception
    {
        public UnbalancedTransactionException(string message) : base(message)
        {
        }
    }
}
