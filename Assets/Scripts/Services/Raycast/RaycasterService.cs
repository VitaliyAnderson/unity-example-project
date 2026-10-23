using System.Collections.Generic;
using UnityEngine;

public class RaycasterService : IServiceScoped
{
    private readonly CameraService _cameraService;

    public readonly float DefaultBuildLength = 100.0f;
    public readonly float DefaultItemsTakeLength = 2.5f;

    public RaycasterService(CameraService cameraService)
    {
        _cameraService = cameraService;
    }

    public List<RaycasterResult<T>> GetByType<T>(RaycastHit[] rays)
    {
        var result = new List<RaycasterResult<T>>();

        foreach (var hit in rays)
        {
            if (hit.transform.gameObject.TryGetComponent(out SpawnableComponent component))
            {
                if (component.Spawnable is T castedItem)
                {
                    result.Add(new(hit, castedItem));
                }
            }
        }

        return result;
    }

    public RaycastHit[] RayAllFromEyes(float length)
    {
        return Physics.RaycastAll(
            _cameraService.Camera.transform.position,
            _cameraService.Camera.transform.forward,
            length);
    }
}
