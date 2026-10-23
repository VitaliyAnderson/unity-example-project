using UnityEngine;
using UnityEngine.UIElements;

public class SceneObjectUi : Spawnable
{
    public UIDocument UIDocument { get; private set; }

    public SceneObjectUi()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return ServiceObjects.List[(int)ServiceObjects.ServiceObject.Ui].Prefab;
    }

    protected override void OnSpawn()
    {
        UIDocument = GetBehaviour<UIDocument>();

        base.OnSpawn();
    }
}
