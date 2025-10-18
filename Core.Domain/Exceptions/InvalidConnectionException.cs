namespace Core.Domain.Exceptions;

public class InvalidConnectionException : DomainException
{
    public InvalidConnectionException(string message) : base(message) { }
}