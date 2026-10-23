using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class UIPanelInventoryExchange : UIPanelService
{
    private readonly UIPanelController _uiPanelController;
    private readonly InputController _inputController;
    private readonly PlayerController _playerController;
    private readonly SpawnerService _spawnerService;

    private UiStorage _inventoryPanelSource;
    private UiStorage _inventoryPanelTarget;

    public Storage StorageToExchange { get; set; }

    private Storage _dragFrom = null;
    private int _dragFromIndex = 0;
    private Label _dragInventoryItem = null;
    private Label _dragObject = null;
    private UiStorageItemState _itemState = UiStorageItemState.Idle;

    protected override bool ShowCursor => true;

    public UIPanelInventoryExchange(
        UIService uiService,
        UIPanelController uiPanelController,
        InputController inputController,
        PlayerController playerController,
        SpawnerService spawnerService)
        : base(uiService, nameof(UIPanelInventoryExchange))
    {
        _uiPanelController = uiPanelController;
        _inputController = inputController;
        _playerController = playerController;
        _spawnerService = spawnerService;
    }

    public override void OnShow()
    {
        if (_playerController.CurrentPlayer != null
            && StorageToExchange != null
            && _playerController.CurrentPlayer.Value is IStorageInventoryComponent storageInventoryComponent)
        {
            _inventoryPanelSource = new(
                Panel,
                StyleClassNames.InventoryPanel,
                storageInventoryComponent.InventoryStorage,
                string.Empty);

            _inventoryPanelTarget = new(
                Panel,
                StyleClassNames.InventoryPanel,
                StorageToExchange,
                string.Empty);

            Panel.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            Panel.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            Panel.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        }
        else
        {
            OnBackEventHandle();
        }

        _inputController.Interact.performed += OnBackEventHandle;
        _inputController.UICancel.performed += OnBackEventHandle;
    }

    public override void OnHide()
    {
        if (_inventoryPanelSource != null && _inventoryPanelTarget != null)
        {
            _inventoryPanelSource.Dispose();
            _inventoryPanelTarget.Dispose();

            StorageToExchange = null;

            Panel.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            Panel.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            Panel.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);

            _inventoryPanelSource = null;
            _inventoryPanelTarget = null;
        }

        _inputController.Interact.performed -= OnBackEventHandle;
        _inputController.UICancel.performed -= OnBackEventHandle;
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
            var inventoryItem = _inventoryPanelSource.GetExistingItemUnderThePointer(evt.position);
            if (inventoryItem != default)
            {
                _dragFrom = _inventoryPanelSource.GetStorage();
                _dragFromIndex = inventoryItem.idx;
                _dragInventoryItem = inventoryItem.val as Label;
            }
        }

        if (_dragFrom == null)
        {
            var targetItem = _inventoryPanelTarget.GetExistingItemUnderThePointer(evt.position);
            if (targetItem != default)
            {
                _dragFrom = _inventoryPanelTarget.GetStorage();
                _dragFromIndex = targetItem.idx;
                _dragInventoryItem = targetItem.val as Label;
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
            var sourceItem = _inventoryPanelSource.GetItemUnderThePointer(evt.position);
            if (sourceItem != default)
            {
                dragTo = _inventoryPanelSource.GetStorage();
                dragToIndex = sourceItem.idx;
            }
        }

        if (dragTo == null)
        {
            var targetItem = _inventoryPanelTarget.GetItemUnderThePointer(evt.position);
            if (targetItem != default)
            {
                dragTo = _inventoryPanelTarget.GetStorage();
                dragToIndex = targetItem.idx;
            }
        }

        if (dragTo != null)
        {
            StorageHelper.SwapItems(_dragFrom, _dragFromIndex, dragTo, dragToIndex);
        }
        else
        {
            if (!_inventoryPanelSource.Get().worldBound.Contains(evt.position)
                && !_inventoryPanelTarget.Get().worldBound.Contains(evt.position))
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
