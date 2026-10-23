using System;

public class PlayerController : IServiceScoped, IDisposable
{
    public ObservableValue<SceneObjectPawn> CurrentPlayer = new();

    public PlayerController()
    {
    }

    public void Dispose()
    {
        CurrentPlayer.Value = null;
    }
}
