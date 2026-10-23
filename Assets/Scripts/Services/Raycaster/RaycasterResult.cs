using UnityEngine;

public class RaycasterResult<T>
{
    public RaycastHit Hit { get; }
    public T Result { get; }

    public RaycasterResult(RaycastHit hit, T result)
    {
        Hit = hit;
        Result = result;
    }
}
