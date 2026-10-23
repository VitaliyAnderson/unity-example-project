using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.Properties;

public class UIPanelSettings : UIPanelService
{
    private readonly ConfigService _configService;
    private readonly DisplayService _displayService;

    protected override bool ShowCursor => true;

    [CreateProperty] public List<string> DisplayResolutionItems { get; set; } = new();
    private string _displayResolutionValue;
    [CreateProperty] public string DisplayResolutionValue
    {
        get => _displayResolutionValue;
        set
        {
            _displayResolutionValue = value;
            ApplyButtonEnabledRefresh();
        }
    }
    private bool _displayFullscreenValue;
    [CreateProperty] public bool DisplayFullscreenValue
    {
        get => _displayFullscreenValue;
        set
        {
            _displayFullscreenValue = value;
            ApplyButtonEnabledRefresh();
        }
    }

    [CreateProperty] public bool ApplyButtonEnabled { get; set; }
    [CreateProperty] public ICommand ApplyButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelSettings(
        UIService uiService,
        UIPanelController uiPanelController,
        ConfigService configService,
        DisplayService displayService)
        : base(uiService, nameof(UIPanelSettings))
    {
        _configService = configService;
        _displayService = displayService;

        ApplyButtonEnabled = false;
        ApplyButtonClick = new DelegateCommand(_ =>
        {
            configService.DisplayResolution.Value = DisplayResolutionValue;
            configService.DisplayFullscreen.Value = DisplayFullscreenValue.ToString();
            configService.Apply();
            configService.Save();
            ApplyButtonEnabledRefresh();
        });

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }

    public override void OnShow()
    {
        DisplayResolutionItems.AddRange(_displayService.GetResolutions().Select(x => x.ToString()));
        DisplayResolutionValue = _configService.DisplayResolution.Value;
        DisplayFullscreenValue = bool.Parse(_configService.DisplayFullscreen.Value);

        ApplyButtonEnabledRefresh();
    }

    public override void OnHide()
    {
        DisplayResolutionItems.Clear();
        DisplayResolutionValue = null;
        DisplayFullscreenValue = false;

        ApplyButtonEnabled = false;
    }

    private void ApplyButtonEnabledRefresh()
    {
        ApplyButtonEnabled =
            DisplayResolutionValue != _configService.DisplayResolution.Value
            || DisplayFullscreenValue.ToString() != _configService.DisplayFullscreen.Value;
    }
}
