using UnityEngine;

public class SceneObjectCamera : Spawnable
{
    public Camera Camera { get; private set; }

    public SceneObjectCamera()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return ServiceObjects.List[(int)ServiceObjects.ServiceObject.Camera].Prefab;
    }

    protected override void OnSpawn()
    {
        Camera = GetBehaviour<Camera>();

        base.OnSpawn();
    }
}
