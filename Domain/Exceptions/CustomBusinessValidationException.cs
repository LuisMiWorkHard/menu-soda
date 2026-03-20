using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MenuSoda.Domain.Exceptions
{
    public sealed class CustomBusinessValidationException : Exception
    {
        public CustomBusinessValidationException(string message) : base(message) {}
        public CustomBusinessValidationException(string message, Exception inner) : base(message, inner) {}
    }
}