using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawnable : IServiceTransient, IDisposable
{
    public Spawnable Parent { get; set; }

    private GameObject SpawnedObject { get; set; }

    [JsonIgnore]
    public string Name { get => SpawnedObject.name; set => SpawnedObject.name = value; }

    [JsonIgnore]
    public Transform Transform { get => SpawnedObject.transform; }

    [JsonIgnore]
    public Vector3 Position { get => SpawnedObject.transform.position; set => SpawnedObject.transform.position = value; }

    [JsonIgnore]
    public Quaternion Rotation { get => SpawnedObject.transform.rotation; set => SpawnedObject.transform.rotation = value; }

    [JsonIgnore]
    public Vector3 Scale { get => SpawnedObject.transform.localScale; set => SpawnedObject.transform.localScale = value; }

    public SpawnableInstantiate Instantiate
    {
        get
        {
            return SpawnedObject != null
                ? new()
                {
                    Position = Position,
                    Rotation = Rotation,
                }
                : new()
                {
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity,
                };
        }
        set
        {
            if (SpawnedObject is null)
            {
                SpawnedObject = UnityEngine.Object.Instantiate(ResolvePrefab(), value.Position, value.Rotation, Parent?.Transform);
                var spawnableComponent = InjectBehaviour<SpawnableComponent>();
                spawnableComponent.Spawnable = this;
                OnSpawn();
            }
            else
            {
                Position = value.Position;
                Rotation = value.Rotation;
            }
        }
    }

    public List<Spawnable> Children { get; set; } = new();

    public void Dispose()
    {
        if (Parent != null)
        {
            Parent.Children.Remove(this);
            Parent = null;
        }

        while (Children.Count > 0)
        {
            Children[0].Dispose();
        }

        if (SpawnedObject != null)
        {
            UnityEngine.Object.Destroy(SpawnedObject);
            SpawnedObject = null;
        }

        OnDespawn();
    }

    protected virtual void OnSpawn()
    {
    }

    protected virtual void OnDespawn()
    {
    }

    public void SetParent(Spawnable parent)
    {
        if (Parent == parent)
        {
            return;
        }

        if (Parent != null)
        {
            Parent.Children.Remove(this);

            Parent = null;
        }

        if (parent != null)
        {
            Parent = parent;

            Parent.Children.Add(this);
        }

        if (SpawnedObject != null)
        {
            SpawnedObject.transform.parent = parent?.SpawnedObject.transform;
        }
    }

    public T InjectBehaviour<T>()
        where T : Component
    {
        return SpawnedObject.AddComponent<T>();
    }

    public T GetBehaviour<T>()
        where T : Component
    {
        return SpawnedObject.GetComponent<T>();
    }

    public void DestroyBehaviour(Component component)
    {
        UnityEngine.Object.Destroy(component);
    }

    protected abstract GameObject ResolvePrefab();
}
