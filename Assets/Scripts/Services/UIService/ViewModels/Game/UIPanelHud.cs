using System;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class UIPanelHud : UIPanelService
{
    private readonly UIPanelController _uiPanelController;
    private readonly PlayerController _playerController;
    private readonly LocalizationService _localizationService;
    private readonly InputController _inputController;

    protected override bool ShowCursor => false;

    private ItemToolAction _firstAction;
    public ItemToolAction FirstAction
    {
        get => _firstAction;
        set
        {
            if (_firstAction != value)
            {
                _firstAction = value;
                (_firstActionLS["Name"] as LocalizedString).TableEntryReference = _firstAction?.Name
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
                (_firstActionLS["Target"] as LocalizedString).TableEntryReference = _firstAction?.Target
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
            }
        }
    }

    private ItemToolAction _secondAction;
    public ItemToolAction SecondAction
    {
        get => _secondAction;
        set
        {
            if (_secondAction != value)
            {
                _secondAction = value;
                (_secondActionLS["Name"] as LocalizedString).TableEntryReference = _secondAction?.Name
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
                (_secondActionLS["Target"] as LocalizedString).TableEntryReference = _secondAction?.Target
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
            }
        }
    }

    private ItemToolAction _specialAction;
    public ItemToolAction SpecialAction
    {
        get => _specialAction;
        set
        {
            if (_specialAction != value)
            {
                _specialAction = value;
                (_specialActionLS["Name"] as LocalizedString).TableEntryReference = _specialAction?.Name
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
                (_specialActionLS["Target"] as LocalizedString).TableEntryReference = _specialAction?.Target
                    ?? _localizationService.CommonStringTableEntryReferenceEmpty;
            }
        }
    }

    private readonly LocalizedString _firstActionLS;
    private readonly LocalizedString _secondActionLS;
    private readonly LocalizedString _specialActionLS;

    [CreateProperty] public string FirstActionText { get; set; }
    [CreateProperty] public string SecondActionText { get; set; }
    [CreateProperty] public string SpecialActionText { get; set; }

    [CreateProperty] public ICommand PointerMoveEvent { get; }

    [CreateProperty] public ICommand PointerUpEvent { get; }

    [CreateProperty] public ICommand PointerDownEvent { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UiStorage EquipmentPanel { get; set; }

    private IDisposable _onCurrentPlayerChangedSubscription;
    private IDisposable _onEquipmentPanelChangedSubscription;

    public UIPanelHud(
        UIService uiService,
        UIPanelController uiPanelController,
        PlayerController playerController,
        LocalizationService localizationService,
        InputController inputController)
        : base(uiService, nameof(UIPanelHud))
    {
        _uiPanelController = uiPanelController;
        _playerController = playerController;
        _localizationService = localizationService;
        _inputController = inputController;

        _firstActionLS = new LocalizedString(localizationService.CommonStringTableReference, "ActionInfo")
        {
            { "Name", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
            { "Target", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
        };
        _secondActionLS = new LocalizedString(localizationService.CommonStringTableReference, "ActionInfo")
        {
            { "Name", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
            { "Target", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
        };
        _specialActionLS = new LocalizedString(localizationService.CommonStringTableReference, "ActionInfo")
        {
            { "Name", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
            { "Target", new LocalizedString(localizationService.CommonStringTableReference, localizationService.CommonStringTableEntryReferenceEmpty) },
        };

        PointerDownEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerDownEvent pointerDownEvent)
            {
                switch (pointerDownEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStart(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStart(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });

        PointerUpEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerUpEvent pointerUpEvent)
            {
                switch (pointerUpEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStop(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStop(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }

    public override void OnShow()
    {
        _firstActionLS.StringChanged += SetFirstActionText;
        _secondActionLS.StringChanged += SetSecondActionText;
        _specialActionLS.StringChanged += SetSpecialActionText;

        _inputController.Move.performed += Move;
        _inputController.Move.canceled += Move;
        _inputController.Look.performed += Look;
        _inputController.Look.canceled += Look;
        _inputController.Sprint.performed += Sprint;
        _inputController.Sprint.canceled += Sprint;
        _inputController.Jump.performed += Jump;
        _inputController.Jump.canceled += Jump;
        _inputController.Interact.performed += Interact;
        _inputController.Scroll.performed += Scroll;
        _inputController.Inventory.performed += ShowInventory;

        _onCurrentPlayerChangedSubscription = _playerController.CurrentPlayer.Subscribe(
            onInit: value => OnCurrentPlayerInit(value),
            onChange: (oldValue, newValue) => OnCurrentPlayerChanged(oldValue, newValue),
            onDeinit: value => OnCurrentPlayerDeinit(value));
    }

    public override void OnHide()
    {
        _firstActionLS.StringChanged -= SetFirstActionText;
        _secondActionLS.StringChanged -= SetSecondActionText;
        _specialActionLS.StringChanged -= SetSpecialActionText;

        _inputController.Move.performed -= Move;
        _inputController.Move.canceled -= Move;
        _inputController.Look.performed -= Look;
        _inputController.Look.canceled -= Look;
        _inputController.Sprint.performed -= Sprint;
        _inputController.Sprint.canceled -= Sprint;
        _inputController.Jump.performed -= Jump;
        _inputController.Jump.canceled -= Jump;
        _inputController.Interact.performed -= Interact;
        _inputController.Scroll.performed -= Scroll;
        _inputController.Inventory.performed -= ShowInventory;
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Move.Set(Vector2.zero.x, Vector2.zero.y);
            player.Look.Set(Vector2.zero.x, Vector2.zero.y);
            player.Sprint = false;
            player.Jump = false;
        }

        _onCurrentPlayerChangedSubscription.Dispose();
        _onCurrentPlayerChangedSubscription = null;
    }

    private void Move(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            var vec2 = context.ReadValue<Vector2>();
            player.Move.Set(vec2.x, vec2.y);
        }
    }

    private void Look(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            var vec2 = context.ReadValue<Vector2>();
            player.Look.Set(vec2.x, vec2.y);
        }
    }

    private void Sprint(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Sprint = context.ReadValueAsButton();
        }
    }

    private void Jump(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Jump = context.ReadValueAsButton();
        }
    }

    private void Interact(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent)
        {
            itemToolComponent.ItemTool.ItemToolService?.SpecialActionExecute(_playerController.CurrentPlayer.Value);
        }
    }

    private void Scroll(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is IStorageEquipmentComponent storageEquipmentComponent
            && _playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent)
        {
            if (itemToolComponent.ItemTool is null || itemToolComponent.ItemTool.ItemToolService.Lock == false)
            {
                var scrollValue = context.ReadValue<Vector2>();
                if (scrollValue.y != 0f)
                {
                    var selTemp = storageEquipmentComponent.EquipmentStorageSelectedIndex.Value;
                    selTemp += (scrollValue.y > 0f) ? 1 : -1;
                    selTemp = Mathf.Clamp(selTemp, 0, storageEquipmentComponent.EquipmentStorage.CountStorage() - 1);
                    //selTemp = (selTemp + storageEquipmentComponent.EquipmentStorage.CountStorage()) % storageEquipmentComponent.EquipmentStorage.CountStorage();
                    storageEquipmentComponent.EquipmentStorageSelectedIndex.Value = selTemp;
                }
            }
        }
    }

    private void ShowInventory(CallbackContext context)
    {
        _uiPanelController.Show<UIPanelInventory>();
    }

    public void OnCurrentPlayerInit(SceneObjectPawn value)
    {
        EquipmentPanel?.Dispose();
        EquipmentPanel = null;

        if (value is IStorageEquipmentComponent storageEquipmentComponent)
        {
            EquipmentPanel = new(
                Panel,
                StyleClassNames.EquipmentPanel,
                storageEquipmentComponent.EquipmentStorage,
                StyleClassNames.InventoryItemActive);

            _onEquipmentPanelChangedSubscription = storageEquipmentComponent.EquipmentStorageSelectedIndex.Subscribe(
                onInit: value => OnCurrentPlayerEqiupmentSelectedIndexInit(value),
                onChange: (oldValue, newValue) => OnCurrentPlayerEqiupmentSelectedIndexChanged(oldValue, newValue),
                onDeinit: value => OnCurrentPlayerEqiupmentSelectedIndexDeinit(value));
        }
    }

    public void OnCurrentPlayerDeinit(SceneObjectPawn value)
    {
        EquipmentPanel?.Dispose();
        EquipmentPanel = null;

        if (value is IStorageEquipmentComponent storageEquipmentComponent)
        {
            _onEquipmentPanelChangedSubscription.Dispose();
            _onEquipmentPanelChangedSubscription = null;
        }
    }

    public void OnCurrentPlayerChanged(SceneObjectPawn oldPawn, SceneObjectPawn newPawn)
    {
        if (oldPawn != null)
        {
            EquipmentPanel?.Dispose();
            EquipmentPanel = null;

            if (oldPawn is IStorageEquipmentComponent storageEquipmentComponent)
            {
                _onEquipmentPanelChangedSubscription.Dispose();
                _onEquipmentPanelChangedSubscription = null;
            }
        }

        if (newPawn != null)
        {
            EquipmentPanel?.Dispose();
            EquipmentPanel = null;

            if (newPawn is IStorageEquipmentComponent storageEquipmentComponent)
            {
                EquipmentPanel = new(
                    Panel,
                    StyleClassNames.EquipmentPanel,
                    storageEquipmentComponent.EquipmentStorage,
                    StyleClassNames.InventoryItemActive);

                _onEquipmentPanelChangedSubscription = storageEquipmentComponent.EquipmentStorageSelectedIndex.Subscribe(
                    onInit: value => OnCurrentPlayerEqiupmentSelectedIndexInit(value),
                    onChange: (oldValue, newValue) => OnCurrentPlayerEqiupmentSelectedIndexChanged(oldValue, newValue),
                    onDeinit: value => OnCurrentPlayerEqiupmentSelectedIndexDeinit(value));
            }
        }
    }

    private void OnCurrentPlayerEqiupmentSelectedIndexInit(int value)
    {
        EquipmentPanel.SetSelected(value);
    }

    private void OnCurrentPlayerEqiupmentSelectedIndexDeinit(int value)
    {
    }

    private void OnCurrentPlayerEqiupmentSelectedIndexChanged(int oldValue, int newValue)
    {
        EquipmentPanel.SetSelected(newValue);
    }

    private void SetFirstActionText(string value)
    {
        FirstActionText = value;
    }

    private void SetSecondActionText(string value)
    {
        SecondActionText = value;
    }

    private void SetSpecialActionText(string value)
    {
        SpecialActionText = value;
    }
}
