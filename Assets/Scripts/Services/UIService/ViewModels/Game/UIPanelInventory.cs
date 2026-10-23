using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class UIPanelInventory : UIPanelService
{
    private readonly UIPanelController _uiPanelController;
    private readonly InputController _inputController;
    private readonly PlayerController _playerController;
    private readonly SpawnerService _spawnerService;

    private UiStorage _inventoryPanel;
    private UiStorage _equipmentPanel;

    private Storage _dragFrom = null;
    private int _dragFromIndex = 0;
    private Label _dragInventoryItem = null;
    private Label _dragObject = null;
    private UiStorageItemState _itemState = UiStorageItemState.Idle;

    protected override bool ShowCursor => true;

    public UIPanelInventory(
        UIService uiService,
        UIPanelController uiPanelController,
        InputController inputController,
        PlayerController playerController,
        SpawnerService spawnerService)
        : base(uiService, nameof(UIPanelInventory))
    {
        _uiPanelController = uiPanelController;
        _inputController = inputController;
        _playerController = playerController;
        _spawnerService = spawnerService;
    }

    public override void OnShow()
    {
        if (_playerController.CurrentPlayer != null
            && _playerController.CurrentPlayer.Value is IStorageInventoryComponent storageInventoryComponent
            && _playerController.CurrentPlayer.Value is IStorageEquipmentComponent storageEquipmentComponent)
        {
            _inventoryPanel = new(
                Panel,
                StyleClassNames.InventoryPanel,
                storageInventoryComponent.InventoryStorage,
                string.Empty);

            _equipmentPanel = new(
                Panel,
                StyleClassNames.EquipmentPanel,
                storageEquipmentComponent.EquipmentStorage,
                string.Empty);

            Panel.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            Panel.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            Panel.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        }
        else
        {
            OnBackEventHandle();
        }

        _inputController.UICancel.performed += OnBackEventHandle;
        _inputController.Inventory.performed += OnBackEventHandle;
    }

    public override void OnHide()
    {
        _inputController.UICancel.performed -= OnBackEventHandle;
        _inputController.Inventory.performed -= OnBackEventHandle;

        if (_inventoryPanel != null && _equipmentPanel != null)
        {
            _inventoryPanel.Dispose();
            _equipmentPanel.Dispose();

            Panel.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            Panel.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            Panel.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);

            _inventoryPanel = null;
            _equipmentPanel = null;
        }
    }

    private void OnBackEventHandle(CallbackContext context)
    {
        OnBackEventHandle();
    }

    private void OnBackEventHandle()
    {
        if (_itemState == UiStorageItemState.Idle)
        {
            _uiPanelController.Show<UIPanelHud>();
        }
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        switch (evt.button)
        {
            case 0:
                ReadyDragAndDrop(evt);
                break;

            case 1:
                switch (_itemState)
                {
                    case UiStorageItemState.Idle:
                        break;

                    case UiStorageItemState.PointerDown:
                        _itemState = UiStorageItemState.Idle;
                        break;

                    case UiStorageItemState.DragAndDrop:
                        CancelDragAndDrop();
                        break;
                }

                break;

            case 2:
                break;
        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        switch (evt.button)
        {
            case 0:
                switch (_itemState)
                {
                    case UiStorageItemState.Idle:
                        break;

                    case UiStorageItemState.PointerDown:
                        Action(evt);
                        break;

                    case UiStorageItemState.DragAndDrop:
                        CompleteDragAndDrop(evt);
                        break;
                }
                break;

            case 1:
                break;

            case 2:
                break;
        }
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        switch (_itemState)
        {
            case UiStorageItemState.Idle:
                break;

            case UiStorageItemState.PointerDown:
                StartDragAndDrop();
                goto case UiStorageItemState.DragAndDrop;

            case UiStorageItemState.DragAndDrop:
                MoveDragAndDrop(evt);
                break;
        }
    }

    private void Action(PointerUpEvent evt)
    {
        DropToScene(_dragFrom, _dragFromIndex);

        _dragFrom = null;
        _dragFromIndex = 0;
        _dragInventoryItem = null;
        _dragObject = null;
        _itemState = UiStorageItemState.Idle;
    }

    private void ReadyDragAndDrop(PointerDownEvent evt)
    {
        if (_itemState != UiStorageItemState.Idle)
        {
            return;
        }

        if (_dragFrom == null)
        {
            var inventoryItem = _inventoryPanel.GetExistingItemUnderThePointer(evt.position);
            if (inventoryItem != default)
            {
                _dragFrom = _inventoryPanel.GetStorage();
                _dragFromIndex = inventoryItem.idx;
                _dragInventoryItem = inventoryItem.val as Label;
            }
        }

        if (_dragFrom == null)
        {
            var equipmentItem = _equipmentPanel.GetExistingItemUnderThePointer(evt.position);
            if (equipmentItem != default)
            {
                _dragFrom = _equipmentPanel.GetStorage();
                _dragFromIndex = equipmentItem.idx;
                _dragInventoryItem = equipmentItem.val as Label;
            }
        }

        if (_dragFrom != null)
        {
            _itemState = UiStorageItemState.PointerDown;
        }
    }

    private void StartDragAndDrop()
    {
        if (_dragObject == null)
        {
            _dragObject = Panel
                .Create<Label>()
                .WithClassName(StyleClassNames.InventoryItem)
                .WithTexture(_dragInventoryItem.resolvedStyle.backgroundImage);

            _dragObject.style.position = Position.Absolute;

            _itemState = UiStorageItemState.DragAndDrop;
        }
    }

    private void MoveDragAndDrop(PointerMoveEvent evt)
    {
        Vector3 pointerDelta = evt.position;

        _dragObject.style.translate = new Vector2(
            pointerDelta.x - Panel.resolvedStyle.width / 2,
            pointerDelta.y - Panel.resolvedStyle.height / 2);
    }

    private void CancelDragAndDrop()
    {
        if (_dragObject != null)
        {
            Panel.Remove(_dragObject);
        }

        _dragFrom = null;
        _dragFromIndex = 0;
        _dragInventoryItem = null;
        _dragObject = null;
        _itemState = UiStorageItemState.Idle;
    }

    private void CompleteDragAndDrop(PointerUpEvent evt)
    {
        if (_dragObject != null)
        {
            Panel.Remove(_dragObject);
        }

        Storage dragTo = null;
        int dragToIndex = 0;

        if (dragTo == null)
        {
            var inventoryItem = _inventoryPanel.GetItemUnderThePointer(evt.position);
            if (inventoryItem != default)
            {
                dragTo = _inventoryPanel.GetStorage();
                dragToIndex = inventoryItem.idx;
            }
        }

        if (dragTo == null)
        {
            var equipmentItem = _equipmentPanel.GetItemUnderThePointer(evt.position);
            if (equipmentItem != default)
            {
                dragTo = _equipmentPanel.GetStorage();
                dragToIndex = equipmentItem.idx;
            }
        }

        if (dragTo != null)
        {
            StorageHelper.SwapItems(_dragFrom, _dragFromIndex, dragTo, dragToIndex);
        }
        else
        {
            if (!_inventoryPanel.Get().worldBound.Contains(evt.position)
                && !_equipmentPanel.Get().worldBound.Contains(evt.position))
            {
                DropToScene(_dragFrom, _dragFromIndex);
            }
        }

        _dragFrom = null;
        _dragFromIndex = 0;
        _dragInventoryItem = null;
        _dragObject = null;
        _itemState = UiStorageItemState.Idle;
    }

    private void DropToScene(Storage storage, int idx)
    {
        if (storage.GiveItem(idx, out var itemKey))
        {
            _spawnerService.SpawnItem(
                itemKey,
                _playerController.CurrentPlayer.Value.Transform.position
                +
                _playerController.CurrentPlayer.Value.Transform.forward * 2.5f,
                new Quaternion(),
                _playerController.CurrentPlayer.Value.Parent,
                (item) => item.Phys = true);
        }
    }
}
