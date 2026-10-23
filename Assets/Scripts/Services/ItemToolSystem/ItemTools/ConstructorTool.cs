using System.Linq;
using UnityEngine;

public class ConstructorTool : NoneTool
{
    private enum ConstructorToolState : int
    {
        None,
        Select,
        Construction,
    }

    private readonly UIPanelController _uiPanelController;
    private readonly PlayerController _playerController;
    private readonly ConstructionController _constructionController;

    public override bool Lock => State is not ConstructorToolState.None;

    private Buildings.Building _buildingKey;
    public Buildings.Building BuildingKey
    {
        get => _buildingKey;
        set
        {
            _buildingKey = value;
        }
    }

    private ConstructorToolState _state = ConstructorToolState.None;
    private ConstructorToolState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                switch (_state)
                {
                    case ConstructorToolState.None:
                        _uiPanelController.Show<UIPanelHud>();
                        break;

                    case ConstructorToolState.Select:
                        _constructionController.StopBuild();
                        _uiPanelController.Show<UIPanelBuildingSelect>();
                        break;

                    case ConstructorToolState.Construction:
                        _constructionController.StartBuild(BuildingKey);
                        _uiPanelController.Show<UIPanelHudConstruction>();
                        break;
                }
            }
        }
    }

    public ConstructorTool(
        UIPanelHud uiPanelHud,
        RaycasterService raycasterService,
        UIPanelController uiPanelController,
        PlayerController playerController,
        ConstructionController constructionController)
        : base(uiPanelHud, raycasterService)
    {
        _uiPanelController = uiPanelController;
        _playerController = playerController;
        _constructionController = constructionController;
    }

    public override void OnUpdate(SceneObjectPawn from, float deltaTime, RaycastHit[] rays)
    {
        switch (State)
        {
            case ConstructorToolState.None:
                base.OnUpdate(from, deltaTime, rays);
                break;

            case ConstructorToolState.Select:
                break;

            case ConstructorToolState.Construction:
                var buildingRays = _raycasterService.RayAllFromEyes(_raycasterService.DefaultBuildLength);

                var result = _raycasterService.GetByType<SceneObjectBiome>(buildingRays);

                if (result.Any())
                {
                    var gridSize = 1;

                    var roundedVector = new Vector3(
                        Mathf.Round(result.First().Hit.point.x / gridSize) * gridSize,
                        Mathf.Round(result.First().Hit.point.y / gridSize) * gridSize,
                        Mathf.Round(result.First().Hit.point.z / gridSize) * gridSize);

                    _constructionController.ExampleMove(roundedVector);
                }
                break;
        }
    }

    public override void NextAction(SceneObjectPawn from)
    {
        _constructionController.ExampleRotate(new(0.0f, 90.0f, 0.0f));
    }

    public override void PrevAction(SceneObjectPawn from)
    {
        _constructionController.ExampleRotate(new(0.0f, -90.0f, 0.0f));
    }

    public override void FirstActionStart(SceneObjectPawn from)
    {
        switch (State)
        {
            case ConstructorToolState.None:
                base.FirstActionStart(from);
                break;

            case ConstructorToolState.Select:
                if (_buildingKey is not Buildings.Building.None)
                {
                    State = ConstructorToolState.Construction;
                }
                break;

            case ConstructorToolState.Construction:
                if (_playerController.CurrentPlayer.Value is IStorageInventoryComponent storageInventoryComponent)
                {
                    _constructionController.SpawnBuilding(storageInventoryComponent.InventoryStorage, _playerController.CurrentPlayer.Value.Parent);
                }
                break;
        }
    }

    public override void FirstActionStop(SceneObjectPawn from)
    {
        switch (State)
        {
            case ConstructorToolState.None:
                base.FirstActionStop(from);
                break;

            case ConstructorToolState.Select:
                break;

            case ConstructorToolState.Construction:
                break;
        }
    }

    public override void SecondActionStart(SceneObjectPawn from)
    {
        switch (State)
        {
            case ConstructorToolState.None:
                BuildingKey = Buildings.Building.None;
                State = ConstructorToolState.Select;
                break;

            case ConstructorToolState.Select:
                BuildingKey = Buildings.Building.None;
                State = ConstructorToolState.None;
                break;

            case ConstructorToolState.Construction:
                State = ConstructorToolState.Select;
                break;
        }
    }

    public override void SecondActionStop(SceneObjectPawn from)
    {
    }

    public override void SpecialActionExecute(SceneObjectPawn from)
    {
        base.SpecialActionExecute(from);
    }
}
