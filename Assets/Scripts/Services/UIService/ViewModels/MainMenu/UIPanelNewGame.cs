using System.Windows.Input;
using Unity.Properties;

public class UIPanelNewGame : UIPanelService
{
    protected override bool ShowCursor => true;

    [CreateProperty] public ICommand NewGameButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelNewGame(
        UIService uiService,
        UIPanelController uiPanelController,
        LevelService levelService)
        : base(uiService, nameof(UIPanelNewGame))
    {
        NewGameButtonClick = new DelegateCommand(_ => levelService.NewGame());

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }
}
