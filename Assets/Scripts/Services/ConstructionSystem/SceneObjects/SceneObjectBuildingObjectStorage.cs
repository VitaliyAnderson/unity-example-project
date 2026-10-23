public class SceneObjectBuildingObjectStorage : SceneObjectBuildingObject, IStorageInventoryComponent, ISceneObjectSpecialAction
{
    private readonly UIPanelController _uiPanelController;
    private readonly UIPanelInventoryExchange _gamePanelInventoryExchange;

    private readonly ItemToolActionOpen _itemToolActionOpen = new();

    public Storage InventoryStorage { get; } = new(StorageTypes.StorageType.Inventory);

    public SceneObjectBuildingObjectStorage(
        UIPanelController uiPanelController,
        UIPanelInventoryExchange gamePanelInventoryExchange)
    {
        _uiPanelController = uiPanelController;
        _gamePanelInventoryExchange = gamePanelInventoryExchange;
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
        return InventoryStorage.IsEmpty();
    }

    public override void Destroy(SceneObjectPawn from, ItemToolService tool)
    {
        base.Destroy(from, tool);
    }
}
