using UnityEngine;

public class SceneObjectBiome : Spawnable
{
    private readonly BiomeGeneratorService _biomeGeneratorService;

    public int Width { get; set; }
    public int Height { get; set; }
    public int Seed { get; set; }
    public int VoxelSize { get; set; } = 1;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;

    public SceneObjectBiome(
        BiomeGeneratorService biomeGeneratorService)
    {
        _biomeGeneratorService = biomeGeneratorService;
    }

    protected override GameObject ResolvePrefab()
    {
        return ServiceObjects.List[(int)ServiceObjects.ServiceObject.Empty].Prefab;
    }

    protected override void OnSpawn()
    {
        _mesh = _biomeGeneratorService.VoxelMapGenerate(Width, Height, Seed, VoxelSize);
        _meshFilter = InjectBehaviour<MeshFilter>();
        _meshRenderer = InjectBehaviour<MeshRenderer>();
        _meshCollider = InjectBehaviour<MeshCollider>();

        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = Color.green;
        _meshRenderer.material = material;

        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;

        base.OnSpawn();
    }
}
