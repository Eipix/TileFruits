namespace Commons.Systems
{
    public interface IRegistry<in T>
    {
        void Register(T item);
        void Unregister(T item);
    }
}
