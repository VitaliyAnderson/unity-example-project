using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Linq;

public class SceneObjectBuildingObjectGrowbed : SceneObjectBuildingObject, IStorageInventoryComponent, ISceneObjectSpecialAction
{
    private readonly UIPanelController _uiPanelController;
    private readonly UIPanelInventoryExchange _gamePanelInventoryExchange;
    private readonly SpawnerService _spawnerService;
    private readonly UnityEventsService _unityEventsService;

    private readonly ItemToolActionOpen _itemToolActionOpen = new();

    public Storage InventoryStorage { get; } = new(StorageTypes.StorageType.Growbed);

    [JsonProperty(IsReference = true)]
    public SceneObjectSeed SceneSeed { get; set; }

    [JsonProperty(IsReference = true)]
    public SceneObjectItem SceneItem { get; set; }

    public float GrowTimer { get; set; }

    public SceneObjectBuildingObjectGrowbed(
        UIPanelController uiPanelController,
        UIPanelInventoryExchange gamePanelInventoryExchange,
        SpawnerService spawnerService,
        UnityEventsService unityEventsService)
    {
        _uiPanelController = uiPanelController;
        _gamePanelInventoryExchange = gamePanelInventoryExchange;
        _spawnerService = spawnerService;
        _unityEventsService = unityEventsService;
    }

    protected override void OnSpawn()
    {
        _unityEventsService.Events.OnUpdate += OnUpdate;

        InventoryStorage.ObservableItems.CollectionChanged += OnCollectionChanged;

        base.OnSpawn();
    }

    protected override void OnDespawn()
    {
        InventoryStorage.ObservableItems.CollectionChanged -= OnCollectionChanged;

        _unityEventsService.Events.OnUpdate -= OnUpdate;

        DespawnSeed();
        DespawnItem();

        base.OnDespawn();
    }

    private void OnUpdate(float deltaTime)
    {
        if (SceneSeed == null)
        {
            if (SceneItem == null)
            {
                var itemKey = InventoryStorage.ObservableItems.First();
                if (itemKey != Items.Item.None)
                {
                    SpawnSeed(itemKey);
                }
            }
        }
        else
        {
            var itemKey = InventoryStorage.ObservableItems.First();
            if (itemKey != (int)Items.Item.None)
            {
                GrowTimer -= deltaTime;
                if (GrowTimer < 0)
                {
                    GrowTimer = 0;

                    // Is it needs to remove the seed?
                    InventoryStorage.Clear();

                    DespawnSeed();
                    SpawnItem(itemKey);
                }
            }
        }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        DespawnSeed();
        DespawnItem();
    }

    private void SpawnSeed(Items.Item item)
    {
        var itemInfo = Items.List[(int)item];

        if (itemInfo.AdditionalInfo is ItemInfoSeed itemInfoSeed)
        {
            GrowTimer = itemInfoSeed.RipeningPeriod.Seconds;

            _spawnerService.SpawnSeed(
                item,
                Position + new UnityEngine.Vector3(0, 1, 0), // TODO: move it to additional building info
                Rotation,
                Parent,
                (seed) => SceneSeed = seed,
                (seed) => SceneSeed = null);
        }
    }

    private void DespawnSeed()
    {
        SceneSeed?.Dispose();
    }

    private void SpawnItem(Items.Item item)
    {
        var itemInfo = Items.List[(int)item];

        if (itemInfo.AdditionalInfo is ItemInfoSeed itemInfoSeed)
        {
            _spawnerService.SpawnItem(
                itemInfoSeed.ItemToSpawn,
                Position + new UnityEngine.Vector3(0, 1, 0), // TODO: move it to additional building info
                Rotation,
                Parent,
                (item) => SceneItem = item,
                (item) => SceneItem = null);
        }
    }

    private void DespawnItem()
    {
        SceneItem?.Dispose();
    }

    bool ISceneObjectSpecialAction.CanExecute(SceneObjectPawn from, ItemToolService tool)
    {
        return _uiPanelController.ActivePanel is UIPanelHud;
    }

    void ISceneObjectSpecialAction.Execute(SceneObjectPawn from, ItemToolService tool)
    {
        if (_uiPanelController.ActivePanel is UIPanelHud)
        {
            _gamePanelInventoryExchange.StorageToExchange = InventoryStorage;

            _uiPanelController.Show<UIPanelInventoryExchange>();
        }
    }

    ItemToolAction ISceneObjectSpecialAction.ActionInfo()
    {
        var buildingInfo = Buildings.List[(int)BuildingKey];

        _itemToolActionOpen.SetTarget(buildingInfo.Name);

        return _itemToolActionOpen;
    }

    public override bool CanDestroy(SceneObjectPawn from, ItemToolService tool)
    {
        var buildingInfo = Buildings.List[(int)BuildingKey];

        return from is IStorageInventoryComponent storageComponent
            && buildingInfo.ItemsForBuild is not null
            && storageComponent.InventoryStorage.CountEmpty() >= InventoryStorage.CountItems() + buildingInfo.ItemsForBuild.Sum(x => x.Value);
    }

    public override void Destroy(SceneObjectPawn from, ItemToolService tool)
    {
        var buildingInfo = Buildings.List[(int)BuildingKey];

        if (from is IStorageInventoryComponent storageComponent
            && storageComponent.InventoryStorage.TakeAllItems(InventoryStorage)
            && storageComponent.InventoryStorage.TakeItems(buildingInfo.ItemsForBuild))
        {
            base.Destroy(from, tool);
        }
    }
}
