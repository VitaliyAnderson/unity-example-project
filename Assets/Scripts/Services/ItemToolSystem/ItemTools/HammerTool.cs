using System.Linq;
using UnityEngine;

public class HammerTool : NoneTool
{
    private enum HammerToolState : int
    {
        None,
        Ready,
        Destroying,
    }

    private readonly ItemToolActionDestroy itemToolActionDestroy = new();
    private readonly ItemToolActionDestroying itemToolActionDestroying = new();

    public override bool Lock => State is HammerToolState.Destroying;

    private HammerToolState _state = HammerToolState.None;
    private HammerToolState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                switch (_state)
                {
                    case HammerToolState.None:
                        {
                            itemToolActionDestroy.SetTarget(null);
                            itemToolActionDestroying.SetTarget(null);
                            _hudPanel.FirstAction = null;
                        }
                        break;

                    case HammerToolState.Ready:
                        {
                            itemToolActionDestroy.SetTarget(Buildings.List[(int)_objectToDestruct.BuildingKey].Name);
                            _hudPanel.FirstAction = itemToolActionDestroy;
                        }
                        break;

                    case HammerToolState.Destroying:
                        {
                            itemToolActionDestroying.SetTarget(Buildings.List[(int)_objectToDestruct.BuildingKey].Name);
                            _hudPanel.FirstAction = itemToolActionDestroying;
                        }
                        break;
                }
            }
        }
    }

    private IBuilding _objectToDestruct;
    private float _destroyingTimer;
    private readonly float _destroyingTimeSec = 3;

    public HammerTool(
        UIPanelHud uiPanelHud,
        RaycasterService raycasterService)
        : base(uiPanelHud, raycasterService)
    {
    }

    public override void OnSpawn(SceneObjectPawn from)
    {
        State = HammerToolState.None;

        base.OnSpawn(from);
    }

    public override void OnDespawn(SceneObjectPawn from)
    {
        State = HammerToolState.None;

        base.OnDespawn(from);
    }

    public override void OnUpdate(SceneObjectPawn from, float deltaTime, RaycastHit[] rays)
    {
        switch (State)
        {
            case HammerToolState.None:
                if (_hudPanel.IsVisible())
                {
                    var result = _raycasterService.GetByType<IBuilding>(rays);

                    var first = result.FirstOrDefault(x => x.Result.CanDestroy(from, this));

                    if (first is not null)
                    {
                        _objectToDestruct = first.Result;
                        State = HammerToolState.Ready;
                        break;
                    }
                }

                base.OnUpdate(from, deltaTime, rays);
                break;

            case HammerToolState.Ready:
                if (_hudPanel.IsVisible())
                {
                    var result = _raycasterService.GetByType<IBuilding>(rays);

                    if (result.Any(x => x.Result == _objectToDestruct))
                    {
                    }
                    else
                    {
                        State = HammerToolState.None;
                        _objectToDestruct = null;
                        _destroyingTimer = 0.0f;
                    }
                }
                break;

            case HammerToolState.Destroying:
                if (_hudPanel.IsVisible())
                {
                    var result = _raycasterService.GetByType<IBuilding>(rays);

                    if (_objectToDestruct.CanDestroy(from, this)
                        && result.Any(x => x.Result == _objectToDestruct))
                    {
                        _destroyingTimer -= deltaTime;
                        if (_destroyingTimer <= 0)
                        {
                            State = HammerToolState.None;
                            _objectToDestruct.Destroy(from, this);
                            _objectToDestruct = null;
                            _destroyingTimer = 0.0f;
                        }
                    }
                    else
                    {
                        State = HammerToolState.None;
                        _objectToDestruct = null;
                        _destroyingTimer = 0.0f;
                    }
                }
                break;
        }
    }

    public override void NextAction(SceneObjectPawn from)
    {
    }

    public override void PrevAction(SceneObjectPawn from)
    {
    }

    public override void FirstActionStart(SceneObjectPawn from)
    {
        switch (State)
        {
            case HammerToolState.None:
                base.FirstActionStart(from);
                break;

            case HammerToolState.Ready:
                State = HammerToolState.Destroying;
                _destroyingTimer = _destroyingTimeSec;

                if (from is SceneObjectPawnPlayer player)
                {
                    player.ItemTool.StartAnimation(ItemToolAnimationsEnum.FirstAction);
                }

                break;

            case HammerToolState.Destroying:
                break;
        }
    }

    public override void FirstActionStop(SceneObjectPawn from)
    {
        switch (State)
        {
            case HammerToolState.None:
                base.FirstActionStop(from);
                break;

            case HammerToolState.Ready:
                break;

            case HammerToolState.Destroying:
                State = HammerToolState.None;
                _destroyingTimer = 0.0f;

                if (from is SceneObjectPawnPlayer player)
                {
                    player.ItemTool.StartAnimation(ItemToolAnimationsEnum.Idle);
                }

                break;
        }
    }

    public override void SecondActionStart(SceneObjectPawn from)
    {
        if (State == HammerToolState.None)
        {
            base.SecondActionStart(from);
        }
    }

    public override void SecondActionStop(SceneObjectPawn from)
    {
        if (State == HammerToolState.None)
        {
            base.SecondActionStop(from);
        }
    }

    public override void SpecialActionExecute(SceneObjectPawn from)
    {
        if (State == HammerToolState.Ready)
        {
            base.SpecialActionExecute(from);
        }
    }
}
