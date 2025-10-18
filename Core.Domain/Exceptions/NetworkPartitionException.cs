namespace Core.Domain.Exceptions;

public class NetworkPartitionException : DomainException
{
    public NetworkPartitionException(Guid sourceId, Guid destinationId) 
        : base($"Nodes '{sourceId}' and '{destinationId}' are in separate network partitions.") { }
}