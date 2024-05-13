namespace amorphie.shield
{
    public class EntityNotFoundException : BusinessException
    {
        public Type Type { get; set; }
        public object Key { get; set; }
        public EntityNotFoundException(Type type, object key)
        {
            Type = type;
            Key = key;
        }
    }
}
