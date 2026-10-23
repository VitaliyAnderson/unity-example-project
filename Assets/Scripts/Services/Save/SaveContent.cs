public class SaveContent : IServiceTransient
{
    public WorldController WorldController { get; }
    public PlayerController PlayerController { get; }

    public SaveContent(
        WorldController worldController,
        PlayerController playerController)
    {
        WorldController = worldController;
        PlayerController = playerController;
    }
}
