namespace MenuSoda.Domain.Exceptions;

public sealed class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message) : base(message) { }
}
