using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

public class SceneObjectItemTool : Spawnable
{
    private readonly NoneTool _noneTool;
    private readonly ConstructorTool _constructorTool;
    private readonly HammerTool _hammerTool;

    public ItemTools.ItemTool ItemToolKey { get; set; }

    [JsonIgnore]
    public ItemToolService ItemToolService { get; private set; }

    private Animator _animator;

    public SceneObjectItemTool(
        NoneTool noneTool,
        ConstructorTool constructorTool,
        HammerTool hammerTool)
    {
        _noneTool = noneTool;
        _constructorTool = constructorTool;
        _hammerTool = hammerTool;
    }

    public void StartAnimation(ItemToolAnimationsEnum animationNumber)
    {
        _animator?.SetInteger("Animation", (int)animationNumber);
    }

    protected override void OnSpawn()
    {
        ItemToolService = ItemToolKey switch
        {
            ItemTools.ItemTool.None => _noneTool,
            ItemTools.ItemTool.Constructor => _constructorTool,
            ItemTools.ItemTool.Hammer => _hammerTool,
            _ => null
        };

        _animator = GetBehaviour<Animator>();

        Parent.Children.ByType<SceneObjectItemTool>()
            .Where(x => x != this)
            .ToList()
            .ForEach(x => x.Dispose());

        ItemToolService?.OnSpawn(Parent as SceneObjectPawn);

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        ItemToolService?.OnDespawn(Parent as SceneObjectPawn);

        base.OnDespawn();
    }

    protected override GameObject ResolvePrefab()
    {
        return ItemTools.List[(int)ItemToolKey].Prefab;
    }
}
