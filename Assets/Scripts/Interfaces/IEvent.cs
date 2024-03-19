public interface IEvent<T>
{
    public void TriggerEvent(T value);
}
