using UnityEngine;

public class TriggerEventsComponent : MonoBehaviour
{
    public delegate void OnTriggerEnterHandler(Spawnable spawnable);
    public delegate void OnTriggerExitHandler(Spawnable spawnable);

    public event OnTriggerEnterHandler OnEnter;
    public event OnTriggerExitHandler OnExit;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out SpawnableComponent component))
        {
            OnEnter?.Invoke(component.Spawnable);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out SpawnableComponent component))
        {
            OnExit?.Invoke(component.Spawnable);
        }
    }
}
