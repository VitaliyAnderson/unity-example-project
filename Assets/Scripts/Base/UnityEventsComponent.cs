using UnityEngine;

public class UnityEventsComponent : MonoBehaviour
{
    public delegate void AwakeHandler();
    public delegate void StartHandler();
    public delegate void UpdateHandler(float deltaTime);
    public delegate void LateUpdateHandler(float deltaTime);
    public delegate void FixedUpdateHandler();

    public event AwakeHandler OnAwake;
    public event StartHandler OnStart;
    public event UpdateHandler OnUpdate;
    public event LateUpdateHandler OnLateUpdate;
    public event FixedUpdateHandler OnFixedUpdate;

    private void Awake()
    {
        OnAwake?.Invoke();
    }

    private void Start()
    {
        OnStart?.Invoke();
    }

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }

    private void LateUpdate()
    {
        OnLateUpdate?.Invoke(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
}
