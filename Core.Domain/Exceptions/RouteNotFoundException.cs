namespace Core.Domain.Exceptions;

public class RouteNotFoundException : DomainException
{
    public Guid SourceId { get; }
    public Guid DestinationId { get; }
    
    public RouteNotFoundException(Guid sourceId, Guid destinationId) 
        : base($"No route found from node '{sourceId}' to '{destinationId}'.")
    {
        SourceId = sourceId;
        DestinationId = destinationId;
    }
}