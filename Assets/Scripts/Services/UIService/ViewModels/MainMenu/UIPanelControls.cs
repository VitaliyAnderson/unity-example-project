using System;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine.InputSystem;

public class UIPanelControls : UIPanelService
{
    private readonly ConfigService _configService;

    protected override bool ShowCursor => true;

    private string _inputActionInteractValue;
    [CreateProperty] public string InputActionInteractValue
    {
        get => _inputActionInteractValue;
        set
        {
            _inputActionInteractValue = value;
            ApplyButtonEnabledRefresh();
        }
    }
    [CreateProperty] public ICommand InputActionInteractChange { get; }
    private string _inputActionJumpValue;
    [CreateProperty] public string InputActionJumpValue
    {
        get => _inputActionJumpValue;
        set
        {
            _inputActionJumpValue = value;
            ApplyButtonEnabledRefresh();
        }
    }
    [CreateProperty] public ICommand InputActionJumpChange { get; }
    private string _inputActionSprintValue;
    [CreateProperty] public string InputActionSprintValue
    {
        get => _inputActionSprintValue;
        set
        {
            _inputActionSprintValue = value;
            ApplyButtonEnabledRefresh();
        }
    }
    [CreateProperty] public ICommand InputActionSprintChange { get; }
    private string _inputActionInventoryValue;
    [CreateProperty] public string InputActionInventoryValue
    {
        get => _inputActionInventoryValue;
        set
        {
            _inputActionInventoryValue = value;
            ApplyButtonEnabledRefresh();
        }
    }
    [CreateProperty] public ICommand InputActionInventoryChange { get; }

    [CreateProperty] public bool PressKeyLabelVisible { get; set; }

    [CreateProperty] public bool ApplyButtonEnabled { get; set; }
    [CreateProperty] public ICommand ApplyButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelControls(
        UIService uiService,
        UIPanelController uiPanelController,
        ConfigService configService)
        : base(uiService, nameof(UIPanelControls))
    {
        _configService = configService;

        InputActionInteractChange = new DelegateCommand(_ => OnPressKeyHandle(value => _inputActionInteractValue = value));
        InputActionJumpChange = new DelegateCommand(_ => OnPressKeyHandle(value => _inputActionJumpValue = value));
        InputActionSprintChange = new DelegateCommand(_ => OnPressKeyHandle(value => _inputActionSprintValue = value));
        InputActionInventoryChange = new DelegateCommand(_ => OnPressKeyHandle(value => _inputActionInventoryValue = value));

        ApplyButtonEnabled = false;
        ApplyButtonClick = new DelegateCommand(_ =>
        {
            if (PressKeyLabelVisible is false)
            {
                configService.Interact.Value = InputActionInteractValue;
                configService.Jump.Value = InputActionJumpValue;
                configService.Sprint.Value = InputActionSprintValue;
                configService.Inventory.Value = InputActionInventoryValue;
                configService.Apply();
                configService.Save();
                ApplyButtonEnabledRefresh();
            }
        });

        BackButtonClick = new DelegateCommand(_ =>
        {
            if (PressKeyLabelVisible is false)
            {
                uiPanelController.Show<UIPanelMainMenu>();
            }
        });

        NavigationCancelEvent = new DelegateCommand(_ =>
        {
            if (PressKeyLabelVisible is false)
            {
                uiPanelController.Show<UIPanelMainMenu>();
            }
        });
    }

    public override void OnShow()
    {
        InputActionInteractValue = _configService.Interact.Value;
        InputActionJumpValue = _configService.Jump.Value;
        InputActionSprintValue = _configService.Sprint.Value;
        InputActionInventoryValue = _configService.Inventory.Value;

        PressKeyLabelVisible = false;
        ApplyButtonEnabled = false;
    }

    public override void OnHide()
    {
        InputActionInteractValue = null;
        InputActionJumpValue = null;
        InputActionSprintValue = null;
        InputActionInventoryValue = null;

        PressKeyLabelVisible = false;
        ApplyButtonEnabled = false;
    }

    private void OnPressKeyHandle(Action<string> onValueUpdated)
    {
        PressKeyLabelVisible = true;

        var tempAction = new InputAction();

        var operation = tempAction.PerformInteractiveRebinding()
            .WithRebindAddingNewBinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                if (tempAction.bindings.Count > 0)
                {
                    onValueUpdated?.Invoke(tempAction.bindings[0].path);
                    ApplyButtonEnabledRefresh();
                }

                PressKeyLabelVisible = false;
                operation.Dispose();
            })
            .OnCancel(operation =>
            {
                PressKeyLabelVisible = false;
                operation.Dispose();
            })
            .Start();
    }

    private void ApplyButtonEnabledRefresh()
    {
        ApplyButtonEnabled =
            InputActionInteractValue != _configService.Interact.Value
            || InputActionJumpValue != _configService.Jump.Value
            || InputActionSprintValue != _configService.Sprint.Value
            || InputActionInventoryValue != _configService.Inventory.Value;
    }
}
