using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine.Localization;

public class UIPanelSaving : UIPanelService
{
    private readonly LevelService _levelService;

    protected override bool ShowCursor => true;

    private readonly LocalizedString _newSaveLocalizedString;
    private readonly Savefile _newSaveSavefile;

    [CreateProperty] public List<UISavefileItem> Items { get; set; } = new();
    private int _selectedIndex = -1;
    [CreateProperty] public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            SaveGameButtonEnabled = _selectedIndex != -1;
        }
    }

    [CreateProperty] public bool SaveGameButtonEnabled { get; set; }
    [CreateProperty] public ICommand SaveGameButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelSaving(
        UIService uiService,
        UIPanelController uiPanelController,
        LocalizationService localizationService,
        LevelService levelService,
        SaveService saveService)
        : base(uiService, nameof(UIPanelSaving))
    {
        _levelService = levelService;

        _newSaveLocalizedString = new LocalizedString
        {
            TableReference = localizationService.CommonStringTableReference,
            TableEntryReference = "New Save",
        };
        _newSaveSavefile = new()
        {
            Name = "New Save",
            Path = null,
        };

        SaveGameButtonEnabled = false;
        SaveGameButtonClick = new DelegateCommand(_ =>
        {
            if (SelectedIndex != -1)
            {
                var savefile = Items[SelectedIndex];
                uiPanelController.Show<UIPanelMainMenu>();
                saveService.SaveGame(savefile.Savefile);
            }
        });

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }

    public override void OnShow()
    {
        _newSaveLocalizedString.StringChanged += NewGameLocalizedStringChanged;

        Items.Add(new UISavefileItem(_newSaveSavefile));
        Items.AddRange(_levelService.RetrieveSaves().Select(savefile => new UISavefileItem(savefile)));
        SelectedIndex = -1;
    }

    public override void OnHide()
    {
        _newSaveLocalizedString.StringChanged -= NewGameLocalizedStringChanged;

        Items.Clear();
        SelectedIndex = -1;
    }

    private void NewGameLocalizedStringChanged(string value)
    {
        _newSaveSavefile.Name = value;
    }
}
