using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.Properties;

public class UIPanelLoading : UIPanelService
{
    private readonly LevelService _levelService;

    protected override bool ShowCursor => true;

    [CreateProperty] public List<UISavefileItem> Items { get; set; } = new();
    private int _selectedIndex = -1;
    [CreateProperty] public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            LoadGameButtonEnabled = _selectedIndex != -1;
        }
    }

    [CreateProperty] public bool LoadGameButtonEnabled { get; set; }
    [CreateProperty] public ICommand LoadGameButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelLoading(
        UIService uiService,
        UIPanelController uiPanelController,
        LevelService levelService)
        : base(uiService, nameof(UIPanelLoading))
    {
        _levelService = levelService;

        LoadGameButtonEnabled = false;
        LoadGameButtonClick = new DelegateCommand(_ =>
        {
            if (SelectedIndex != -1)
            {
                var savefile = Items[SelectedIndex];
                uiPanelController.Show<UIPanelMainMenu>();
                levelService.LoadGame(savefile.Savefile);
            }
        });

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }

    public override void OnShow()
    {
        Items.AddRange(_levelService.RetrieveSaves().Select(savefile => new UISavefileItem(savefile)));
        SelectedIndex = -1;
    }

    public override void OnHide()
    {
        Items.Clear();
        SelectedIndex = -1;
    }
}
