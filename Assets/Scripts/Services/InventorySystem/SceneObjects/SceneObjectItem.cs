using Newtonsoft.Json;
using System;
using UnityEngine;

public class SceneObjectItem : Spawnable, ISceneObjectFirstAction, IDisposable
{
    private readonly UIPanelController _uiPanelController;

    private readonly ItemToolActionTake _itemToolActionTake = new();

    private Items.Item _itemKey;
    public Items.Item ItemKey
    {
        get => _itemKey;
        set
        {
            _itemKey = value;
            _itemToolActionTake.SetTarget(Items.List[(int)_itemKey].Name);
        }
    }

    private TriggerEventsComponent Triggerable { get; set; }
    private Rigidbody _rigidBody;
    private BoxCollider _boxCollider;

    private bool _phys;

    public Action<SceneObjectItem> OnSpawnEvent { get; set; }
    public Action<SceneObjectItem> OnDespawnEvent { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Phys
    {
        get => _phys;
        set
        {
            _phys = value;

            if (_phys)
            {
                _rigidBody = InjectBehaviour<Rigidbody>();
                _rigidBody.isKinematic = false;

                _boxCollider.isTrigger = false;

                Triggerable = InjectBehaviour<TriggerEventsComponent>();

                Triggerable.OnEnter += OnTriggerEnter;
                Triggerable.OnExit += OnTriggerExit;
            }
            else
            {
                DestroyBehaviour(_rigidBody);
                _rigidBody = null;

                _boxCollider.isTrigger = true;

                Triggerable.OnEnter -= OnTriggerEnter;
                Triggerable.OnExit -= OnTriggerExit;

                UnityEngine.Object.Destroy(Triggerable);
                Triggerable = null;
            }
        }
    }

    public SceneObjectItem(
        UIPanelController uiPanelController)
    {
        _uiPanelController = uiPanelController;
    }

    protected override GameObject ResolvePrefab()
    {
        return Items.List[(int)ItemKey].Prefab;
    }

    protected override void OnSpawn()
    {
        _boxCollider = Transform.GetComponent<BoxCollider>();

        OnSpawnEvent?.Invoke(this);

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        if (Phys)
        {
            Phys = false;
        }

        OnDespawnEvent?.Invoke(this);

        base.OnDespawn();
    }

    private void OnTriggerEnter(Spawnable spawnable)
    {
        Phys = false;
    }

    private void OnTriggerExit(Spawnable spawnable)
    {
    }

    bool ISceneObjectFirstAction.CanExecute(SceneObjectPawn from, ItemToolService tool)
    {
        if (_uiPanelController.ActivePanel is not UIPanelHud)
        {
            return false;
        }

        if (from is IStorageEquipmentComponent storageEquipmentComponent)
        {
            if (storageEquipmentComponent.EquipmentStorage.CanTakeItem(ItemKey))
            {
                return true;
            }
        }

        if (from is IStorageInventoryComponent storageInventoryComponent)
        {
            if (storageInventoryComponent.InventoryStorage.CanTakeItem(ItemKey))
            {
                return true;
            }
        }

        return false;
    }

    void ISceneObjectFirstAction.Execute(SceneObjectPawn from, ItemToolService tool)
    {
        if (_uiPanelController.ActivePanel is not UIPanelHud)
        {
            return;
        }

        if (from is IStorageEquipmentComponent storageEquipmentComponent)
        {
            if (storageEquipmentComponent.EquipmentStorage.CanTakeItem(ItemKey)
                && storageEquipmentComponent.EquipmentStorage.TakeItem(ItemKey))
            {
                Dispose();
                return;
            }
        }

        if (from is IStorageInventoryComponent storageInventoryComponent)
        {
            if (storageInventoryComponent.InventoryStorage.CanTakeItem(ItemKey)
                && storageInventoryComponent.InventoryStorage.TakeItem(ItemKey))
            {
                Dispose();
                return;
            }
        }
    }

    ItemToolAction ISceneObjectFirstAction.ActionInfo()
    {
        return _itemToolActionTake;
    }
}
