using System.Windows.Input;
using Unity.Properties;

public class UIPanelQuit : UIPanelService
{
    protected override bool ShowCursor => true;

    [CreateProperty] public ICommand QuitButtonClick { get; }

    [CreateProperty] public ICommand BackButtonClick { get; }

    [CreateProperty] public ICommand NavigationCancelEvent { get; }

    public UIPanelQuit(
        UIService uiService,
        UIPanelController uiPanelController,
        LevelService levelService,
        QuitService quitService)
        : base(uiService, nameof(UIPanelQuit))
    {
        QuitButtonClick = new DelegateCommand(_ =>
        {
            if (levelService.IsMenu is false)
                levelService.LoadMenu();
            else
                quitService.Execute();
        });

        BackButtonClick = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());

        NavigationCancelEvent = new DelegateCommand(_ => uiPanelController.Show<UIPanelMainMenu>());
    }
}
