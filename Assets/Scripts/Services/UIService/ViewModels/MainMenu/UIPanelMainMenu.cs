using System.Windows.Input;
using Unity.Properties;

public class UIPanelMainMenu : UIPanelService
{
    private readonly LevelService _levelService;

    protected override bool ShowCursor => true;

    [CreateProperty] public bool NewGameButtonEnabled => _levelService.IsMenu;
    [CreateProperty] public ICommand NewGameButtonClick { get; }

    [CreateProperty] public ICommand LoadGameButtonClick { get; }

    [CreateProperty] public bool SaveGameButtonEnabled => _levelService.IsMenu is false;
    [CreateProperty] public ICommand SaveGameButtonClick { get; }

    [CreateProperty] public ICommand LocalizationsButtonClick { get; }

    [CreateProperty] public ICommand SettingsButtonClick { get; }

    [CreateProperty] public ICommand ControlsButtonClick { get; }

    [CreateProperty] public ICommand QuitButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelMainMenu(
        UIService uiService,
        UIPanelController uiPanelController,
        LevelService levelService)
        : base(uiService, nameof(UIPanelMainMenu))
    {
        _levelService = levelService;

        NewGameButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelNewGame>());

        LoadGameButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelLoading>());

        SaveGameButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelSaving>());

        LocalizationsButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelLocalizations>());

        SettingsButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelSettings>());

        ControlsButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelControls>());

        QuitButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelQuit>());

        NavigationCancelEvent = new DelegateCommand(_ =>
        {
            if (levelService.IsMenu is false)
            {
                uiPanelController.Show<UIPanelHud>();
            }
        });
    }
}
