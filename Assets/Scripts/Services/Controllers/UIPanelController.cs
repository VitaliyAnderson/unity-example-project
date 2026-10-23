using Autofac;

public class UIPanelController : IServiceScoped
{
    private readonly ILifetimeScope _lifetimeScope;

    public UIPanelService ActivePanel { get; set; }

    public UIPanelController(
        ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    public void Show<T>()
        where T : UIPanelService
    {
        var uiPanelService = _lifetimeScope.Resolve<T>();

        ActivePanel?.Hide();
        ActivePanel = uiPanelService;
        ActivePanel.Show();
    }
}
