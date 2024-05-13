using System.Net;

namespace amorphie.shield;

public class EntityNotFoundException : BusinessException
{
    public Type EntityType { get; set; }
    public object? Key { get; set; }

    public EntityNotFoundException(Type entityType, object? key, string details = "")
        : base(
            HttpStatusCode.NotFound.GetHashCode(), 
            "error", 
            key == null
                ? $"There is no such an entity given id. Entity type: {entityType.FullName}"
                : $"There is no such an entity. Entity type: {entityType.FullName}, id: {key}",
            details)
    {
        EntityType = entityType;
        Key = key;
    }
}
