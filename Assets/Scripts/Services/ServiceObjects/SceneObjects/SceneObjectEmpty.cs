using UnityEngine;

public class SceneObjectEmpty : Spawnable
{
    public SceneObjectEmpty()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return ServiceObjects.List[(int)ServiceObjects.ServiceObject.Empty].Prefab;
    }
}
