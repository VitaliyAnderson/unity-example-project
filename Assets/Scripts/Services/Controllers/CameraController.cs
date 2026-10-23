public class CameraController : IServiceScoped
{
    private readonly CameraService _cameraService;

    public ObservableValue<SceneObjectPawnPlayer> CurrentPlayer = new();

    public CameraController(
        CameraService cameraService)
    {
        _cameraService = cameraService;
    }
}
