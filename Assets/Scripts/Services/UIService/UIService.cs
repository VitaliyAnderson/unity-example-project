using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIService : IServiceScoped, IDisposable
{
    private readonly SceneObjectUi _sceneObjectUi;

    public VisualElement RootVisualElement => _sceneObjectUi.UIDocument.rootVisualElement;

    public UIService(
        SpawnerService spawnerService)
    {
        _sceneObjectUi = spawnerService.SpawnUi(Vector3.zero, Quaternion.identity);
    }

    public void Dispose()
    {
        _sceneObjectUi.Dispose();
    }
}
