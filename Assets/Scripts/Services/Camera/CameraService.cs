using System;
using UnityEngine;

public class CameraService : IServiceScoped, IDisposable
{
    private readonly SceneObjectCamera _currentCamera;

    public Camera Camera => _currentCamera.Camera;

    public CameraService(
        SpawnerService spawnerService)
    {
        _currentCamera = spawnerService.SpawnCamera(Vector3.zero, Quaternion.identity);
    }

    public void Dispose()
    {
        _currentCamera.Dispose();
    }
}
