using UnityEngine;

public abstract class SceneObjectBuildingObject : Spawnable, IBuilding
{
    public Buildings.Building BuildingKey { get; set; }

    protected override GameObject ResolvePrefab()
    {
        return Buildings.List[(int)BuildingKey].Prefab;
    }

    public virtual bool CanDestroy(SceneObjectPawn from, ItemToolService tool)
    {
        return true;
    }

    public virtual void Destroy(SceneObjectPawn from, ItemToolService tool)
    {
        Dispose();
    }
}
