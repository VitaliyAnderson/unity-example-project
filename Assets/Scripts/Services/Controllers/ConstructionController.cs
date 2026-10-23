using System;
using UnityEngine;

public class ConstructionController : IServiceScoped, IDisposable
{
    private readonly SpawnerService _spawnerService;

    private Buildings.Building _buildingKey = Buildings.Building.None;
    private SceneObjectBuildingObjectExample _buildingTemp;

    private readonly Marks.Mark _markKey = Marks.Mark.Mark1;
    private SceneObjectBuildingMark _buildingMark;

    public ConstructionController(
        SpawnerService spawnerService)
    {
        _spawnerService = spawnerService;
    }

    public void Dispose()
    {
        if (IsBuild())
        {
            StopBuild();
        }
    }

    public void StartBuild(Buildings.Building building)
    {
        if (IsBuild())
        {
            StopBuild();
        }

        if (_buildingKey is Buildings.Building.None)
        {
            _buildingKey = building;
            _buildingTemp = _spawnerService.SpawnBuildingObjectExample(building);
            _buildingMark = _spawnerService.SpawnBuildingObjectMark(_markKey);
            _buildingMark.CanBuild = _buildingTemp.CanBuild;
        }
    }

    public void StopBuild()
    {
        if (IsBuild())
        {
            _buildingTemp.Dispose();
            _buildingMark.Dispose();

            _buildingKey = Buildings.Building.None;
            _buildingTemp = null;
            _buildingMark = null;
        }
    }

    public bool IsBuild()
    {
        return _buildingKey is not Buildings.Building.None;
    }

    public void SpawnBuilding(Storage storage, Spawnable parent)
    {
        if (_buildingTemp.CanBuild)
        {
            var buildingInfo = Buildings.List[(int)_buildingKey];

            if (buildingInfo.ItemsForBuild != null)
            {
                if (storage.CanGiveItems(buildingInfo.ItemsForBuild) == false)
                {
                    return;
                }

                storage.GiveItems(buildingInfo.ItemsForBuild);
            }

            _spawnerService.SpawnBuildingObject(
                _buildingKey,
                _buildingTemp.Position,
                _buildingTemp.Rotation,
                parent);
        }
    }

    public void ExampleMove(Vector3 position)
    {
        var markInfo = Marks.List[(int)_markKey];

        _buildingTemp.Position = position;
        _buildingMark.Position = position + markInfo.Offset;

        _buildingMark.CanBuild = _buildingTemp.CanBuild;
    }

    public void ExampleRotate(Vector3 rotation)
    {
        _buildingTemp.Transform.Rotate(rotation);
        _buildingMark.Transform.Rotate(rotation);

        _buildingMark.CanBuild = _buildingTemp.CanBuild;
    }
}
