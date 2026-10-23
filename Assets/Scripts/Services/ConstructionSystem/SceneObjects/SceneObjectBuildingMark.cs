using UnityEngine;

public class SceneObjectBuildingMark : Spawnable
{
    public Marks.Mark MarkKey { get; set; }

    private MeshRenderer _meshRenderer;

    private bool _canBuild = false;
    public bool CanBuild
    {
        get
        {
            return _canBuild;
        }
        set
        {
            _canBuild = value;
            var markInfo = Marks.List[(int)MarkKey];
            _meshRenderer.material = _canBuild ? markInfo.Enabled : markInfo.Disabled;
        }
    }

    public SceneObjectBuildingMark()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return Marks.List[(int)MarkKey].Prefab;
    }

    protected override void OnSpawn()
    {
        _meshRenderer = GetBehaviour<MeshRenderer>();

        base.OnSpawn();
    }
}
