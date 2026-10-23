using Autofac;
using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerService : IServiceScoped
{
    private readonly ILifetimeScope _lifetimeScope;

    public SpawnerService(
        ILifetimeScope sceneScope)
    {
        _lifetimeScope = sceneScope;
    }

    public SceneObjectCamera SpawnCamera(
        Vector3 position,
        Quaternion rotation)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectCamera>();

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Camera";

        return spawnable;
    }

    public SceneObjectUnityEvents SpawnUnityEvents(
        Vector3 position,
        Quaternion rotation)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectUnityEvents>();

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "UnityEventsService";

        return spawnable;
    }

    public SceneObjectUi SpawnUi(
        Vector3 position,
        Quaternion rotation)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectUi>();

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Ui";

        return spawnable;
    }

    public SceneObjectEmpty SpawnEmpty(
        Vector3 position,
        Quaternion rotation)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectEmpty>();

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Empty";

        return spawnable;
    }

    public SceneObjectBiome SpawnBiome(
        int width,
        int height,
        int seed,
        Vector3 position,
        Quaternion rotation)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectBiome>();

        spawnable.Width = width;
        spawnable.Height = height;
        spawnable.Seed = seed;

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Biome";

        return spawnable;
    }
    
    public SceneObjectLightSun SpawnLightSun(
        Vector3 position,
        Quaternion rotation,
        SceneObjectBiome parent)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectLightSun>();

        spawnable.SetParent(parent);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Light Sun";

        return spawnable;
    }
    
    public SceneObjectPawn SpawnPawn(
        Pawns.Pawn pawnKey,
        Vector3 position,
        Quaternion rotation,
        SceneObjectBiome parent)
    {
        SceneObjectPawn spawnable = null;

        spawnable = pawnKey switch
        {
            Pawns.Pawn.Player => _lifetimeScope.Resolve<SceneObjectPawnPlayer>(),
            _ => _lifetimeScope.Resolve<SceneObjectPawnPlayer>()
        };

        spawnable.PawnKey = pawnKey;

        spawnable.SetParent(parent);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Pawn";

        return spawnable;
    }

    public SceneObjectItemTool SpawnItemTool(
        ItemTools.ItemTool itemToolKey,
        SceneObjectPawn pawn)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectItemTool>();

        spawnable.ItemToolKey = itemToolKey;

        spawnable.SetParent(pawn);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
        };

        spawnable.Transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        spawnable.Name = "ItemTool";

        return spawnable;
    }

    public SceneObjectItem SpawnItem(
        Items.Item itemKey,
        Vector3 position,
        Quaternion rotation,
        Spawnable parent,
        Action<SceneObjectItem> onSpawnEvent = null,
        Action<SceneObjectItem> onDespawnEvent = null)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectItem>();

        spawnable.ItemKey = itemKey;
        spawnable.OnSpawnEvent = onSpawnEvent;
        spawnable.OnDespawnEvent = onDespawnEvent;

        spawnable.SetParent(parent);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Item";

        return spawnable;
    }

    public SceneObjectSeed SpawnSeed(
        Items.Item itemKey,
        Vector3 position,
        Quaternion rotation,
        Spawnable parent,
        Action<SceneObjectSeed> onSpawnEvent = null,
        Action<SceneObjectSeed> onDespawnEvent = null)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectSeed>();

        spawnable.ItemKey = itemKey;
        spawnable.OnSpawnEvent = onSpawnEvent;
        spawnable.OnDespawnEvent = onDespawnEvent;

        spawnable.SetParent(parent);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Seed";

        return spawnable;
    }

    public SceneObjectBuildingObject SpawnBuildingObject(
        Buildings.Building buildingKey,
        Vector3 position,
        Quaternion rotation,
        Spawnable parent)
    {
        SceneObjectBuildingObject spawnable = null;

        spawnable = buildingKey switch
        {
            Buildings.Building.Storage => _lifetimeScope.Resolve<SceneObjectBuildingObjectStorage>(),
            Buildings.Building.Growbed => _lifetimeScope.Resolve<SceneObjectBuildingObjectGrowbed>(),
            _ => _lifetimeScope.Resolve<SceneObjectBuildingObjectStatic>()
        };

        spawnable.BuildingKey = buildingKey;

        spawnable.SetParent(parent);

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = position,
            Rotation = rotation,
        };

        spawnable.Name = "Building";

        return spawnable;
    }

    public SceneObjectBuildingObjectExample SpawnBuildingObjectExample(
        Buildings.Building buildingKey)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectBuildingObjectExample>();

        spawnable.Prefab = Buildings.List[(int)buildingKey].PrefabExample;

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
        };

        spawnable.Name = "BuildingExample";

        return spawnable;
    }

    public SceneObjectBuildingMark SpawnBuildingObjectMark(
        Marks.Mark markKey)
    {
        var spawnable = _lifetimeScope.Resolve<SceneObjectBuildingMark>();

        spawnable.MarkKey = markKey;

        spawnable.Instantiate = new SpawnableInstantiate
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
        };

        spawnable.Name = "Mark";

        return spawnable;
    }
}