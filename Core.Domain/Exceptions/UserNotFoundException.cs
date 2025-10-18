namespace Core.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string identifier) 
        : base($"User '{identifier}' was not found.") { }
}
