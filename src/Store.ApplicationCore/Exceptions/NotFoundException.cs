using System;
using System.Diagnostics.CodeAnalysis;

namespace Store.ApplicationCore.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }
        public NotFoundException(string message) : base(message)
        {
        }

    }
}