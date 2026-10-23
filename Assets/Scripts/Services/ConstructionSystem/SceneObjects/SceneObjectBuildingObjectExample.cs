using UnityEngine;

public class SceneObjectBuildingObjectExample : Spawnable
{
    public GameObject Prefab { get; set; }

    private TriggerEventsComponent _triggerEvents;
    private int _buildCollisions;

    public bool CanBuild { get => _buildCollisions == 0; }

    public SceneObjectBuildingObjectExample()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return Prefab;
    }

    protected override void OnSpawn()
    {
        _triggerEvents = InjectBehaviour<TriggerEventsComponent>();
        _triggerEvents.OnEnter += OnTriggerEnter;
        _triggerEvents.OnExit += OnTriggerExit;

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        _triggerEvents.OnEnter -= OnTriggerEnter;
        _triggerEvents.OnExit -= OnTriggerExit;

        base.OnDespawn();
    }

    private void OnTriggerEnter(Spawnable spawnable)
    {
        if (spawnable is not SceneObjectBiome)
        {
            _buildCollisions++;
        }
    }

    private void OnTriggerExit(Spawnable spawnable)
    {
        if (spawnable is not SceneObjectBiome)
        {
            _buildCollisions--;
        }
    }
}
