using System.Linq;
using UnityEngine;

public class NoneTool : ItemToolService
{
    protected readonly UIPanelHud _hudPanel;
    protected readonly RaycasterService _raycasterService;

    public override bool Lock => false;

    private ISceneObjectFirstAction _actionFirst;
    protected ISceneObjectFirstAction ActionFirst
    {
        get => _actionFirst;
        set
        {
            if (_actionFirst != value)
            {
                _actionFirst = value;
                if (_hudPanel is not null)
                {
                    _hudPanel.FirstAction = _actionFirst?.ActionInfo();
                }
            }
        }
    }

    private ISceneObjectSpecialAction _actionSpecial;

    protected ISceneObjectSpecialAction ActionSpecial
    {
        get => _actionSpecial;
        set
        {
            if (_actionSpecial != value)
            {
                _actionSpecial = value;
                _hudPanel.SpecialAction = _actionSpecial?.ActionInfo();
            }
        }
    }

    public NoneTool(
        UIPanelHud uiPanelHud,
        RaycasterService raycasterService)
    {
        _hudPanel = uiPanelHud;
        _raycasterService = raycasterService;
    }

    public override void OnSpawn(SceneObjectPawn from)
    {
        ActionFirst = null;
        ActionSpecial = null;
    }

    public override void OnDespawn(SceneObjectPawn from)
    {
        ActionFirst = null;
        ActionSpecial = null;
    }

    public override void OnUpdate(SceneObjectPawn from, float deltaTime, RaycastHit[] rays)
    {
        ISceneObjectFirstAction actionFirst = null;
        ISceneObjectSpecialAction actionSpecial = null;

        if (_hudPanel.IsVisible())
        {
            {
                var result = _raycasterService.GetByType<ISceneObjectFirstAction>(rays);
                var action = result.FirstOrDefault(x => x.Result.CanExecute(from, this));
                if (action != null)
                {
                    actionFirst = action.Result;
                }
            }

            {
                var result = _raycasterService.GetByType<ISceneObjectSpecialAction>(rays);
                var action = result.FirstOrDefault(x => x.Result.CanExecute(from, this));
                if (action != null)
                {
                    actionSpecial = action.Result;
                }
            }
        }

        ActionFirst = actionFirst;
        ActionSpecial = actionSpecial;
    }

    public override void NextAction(SceneObjectPawn from)
    {
    }

    public override void PrevAction(SceneObjectPawn from)
    {
    }

    public override void FirstActionStart(SceneObjectPawn from)
    {
        ActionFirst?.Execute(from, this);
    }

    public override void FirstActionStop(SceneObjectPawn from)
    {
    }

    public override void SecondActionStart(SceneObjectPawn from)
    {
    }

    public override void SecondActionStop(SceneObjectPawn from)
    {
    }

    public override void SpecialActionExecute(SceneObjectPawn from)
    {
        ActionSpecial?.Execute(from, this);
    }
}
