public class UIStarterService : IServiceScoped
{
    public UIStarterService(
        LevelService levelService,
        UIPanelController uiPanelController)
    {
        if (levelService.IsMenu)
        {
            uiPanelController.Show<UIPanelMainMenu>();
        }
        else
        {
            uiPanelController.Show<UIPanelHud>();
        }
    }
}
