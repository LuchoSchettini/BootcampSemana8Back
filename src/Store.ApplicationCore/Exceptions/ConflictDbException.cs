using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ConflictDbException : Exception
    {

        public ConflictDbException()
        {
        }
        public ConflictDbException(string message) : base(message)
        {
        }
    }
}
