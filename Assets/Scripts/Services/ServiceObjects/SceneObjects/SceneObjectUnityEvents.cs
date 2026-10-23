using UnityEngine;

public class SceneObjectUnityEvents : Spawnable
{
    public UnityEventsComponent UpdateEvents { get; private set; }

    public SceneObjectUnityEvents()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return ServiceObjects.List[(int)ServiceObjects.ServiceObject.UnityEvents].Prefab;
    }

    protected override void OnSpawn()
    {
        UpdateEvents = GetBehaviour<UnityEventsComponent>();

        base.OnSpawn();
    }
}
