using System;
using UnityEngine;

public class SceneObjectSeed : Spawnable
{
    public Items.Item ItemKey { get; set; }

    public Action<SceneObjectSeed> OnSpawnEvent { get; set; }
    public Action<SceneObjectSeed> OnDespawnEvent { get; set; }

    public SceneObjectSeed()
    {
    }

    protected override void OnSpawn()
    {
        OnSpawnEvent?.Invoke(this);

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        OnDespawnEvent?.Invoke(this);

        base.OnDespawn();
    }

    protected override GameObject ResolvePrefab()
    {
        return Items.List[(int)ItemKey].AdditionalInfo is ItemInfoSeed itemInfoSeed
            ? itemInfoSeed.Prefab
            : null;
    }
}
