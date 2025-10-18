namespace Core.Domain.Exceptions;

public class NodeNotFoundException : DomainException
{
    public Guid NodeId { get; }
    
    public NodeNotFoundException(Guid nodeId) 
        : base($"Node with ID '{nodeId}' was not found.")
    {
        NodeId = nodeId;
    }
}
