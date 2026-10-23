using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine.Localization.Settings;

public class UIPanelLocalizations : UIPanelService
{
    private readonly ConfigService _configService;

    protected override bool ShowCursor => true;

    [CreateProperty] public List<UILocalizationItem> Items { get; set; } = new();
    private int _selectedIndex = -1;
    [CreateProperty] public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            ApplyButtonEnabled = _selectedIndex != -1 && Items[_selectedIndex].Name != _configService.Language.Value;
        }
    }

    [CreateProperty] public bool ApplyButtonEnabled { get; set; }
    [CreateProperty] public ICommand ApplyButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelLocalizations(
        UIService uiService,
        UIPanelController uiPanelController,
        ConfigService configService)
        : base(uiService, nameof(UIPanelLocalizations))
    {
        _configService = configService;

        ApplyButtonEnabled = false;
        ApplyButtonClick = new DelegateCommand(_ =>
        {
            if (SelectedIndex != -1)
            {
                var item = Items[SelectedIndex];
                configService.Language.Value = item.Locale.LocaleName;
                configService.Apply();
                configService.Save();
                ApplyButtonEnabled = false;
            }
        });

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }

    public override void OnShow()
    {
        Items.AddRange(LocalizationSettings.AvailableLocales.Locales.Select(locale => new UILocalizationItem(locale)));
        SelectedIndex = -1;
    }

    public override void OnHide()
    {
        Items.Clear();
        SelectedIndex = -1;
    }
}
