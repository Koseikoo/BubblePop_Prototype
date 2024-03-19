using System.Collections;

public interface IVisualize<T>
{
    public IEnumerator Visualize(T newValue);
}
