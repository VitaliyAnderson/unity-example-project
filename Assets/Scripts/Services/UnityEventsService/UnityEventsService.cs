using System;
using UnityEngine;

public class UnityEventsService : IServiceScoped, IDisposable
{
    private readonly SceneObjectUnityEvents _sceneObjectUpdate;

    public UnityEventsComponent Events => _sceneObjectUpdate.UpdateEvents;

    public UnityEventsService(
        SpawnerService spawnerService)
    {
        _sceneObjectUpdate = spawnerService.SpawnUnityEvents(Vector3.zero, Quaternion.identity);
    }

    public void Dispose()
    {
        _sceneObjectUpdate.Dispose();
    }
}
