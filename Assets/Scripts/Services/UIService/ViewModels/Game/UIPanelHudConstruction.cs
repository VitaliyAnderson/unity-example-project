using System.Windows.Input;
using Unity.Properties;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class UIPanelHudConstruction : UIPanelService
{
    private readonly ConstructionController _constructionController;
    private readonly PlayerController _playerController;
    private readonly InputController _inputController;

    protected override bool ShowCursor => false;

    [CreateProperty] public ICommand PointerDownEvent { get; }

    [CreateProperty] public ICommand PointerUpEvent { get; }

    public UIPanelHudConstruction(
        UIService uiService,
        UIPanelController uiPanelController,
        ConstructionController constructionController,
        PlayerController playerController,
        InputController inputController)
        : base(uiService, nameof(UIPanelHudConstruction))
    {
        _constructionController = constructionController;
        _playerController = playerController;
        _inputController = inputController;

        PointerDownEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerDownEvent pointerDownEvent)
            {
                switch (pointerDownEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStart(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStart(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });

        PointerUpEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerUpEvent pointerUpEvent)
            {
                switch (pointerUpEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStop(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStop(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });
    }

    public override void OnShow()
    {
        _inputController.Move.performed += Move;
        _inputController.Move.canceled += Move;
        _inputController.Look.performed += Look;
        _inputController.Look.canceled += Look;
        _inputController.Sprint.performed += Sprint;
        _inputController.Sprint.canceled += Sprint;
        _inputController.Jump.performed += Jump;
        _inputController.Jump.canceled += Jump;
        _inputController.Scroll.performed += Scroll;
    }

    public override void OnHide()
    {
        _inputController.Move.performed -= Move;
        _inputController.Move.canceled -= Move;
        _inputController.Look.performed -= Look;
        _inputController.Look.canceled -= Look;
        _inputController.Sprint.performed -= Sprint;
        _inputController.Sprint.canceled -= Sprint;
        _inputController.Jump.performed -= Jump;
        _inputController.Jump.canceled -= Jump;
        _inputController.Scroll.performed -= Scroll;
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Move.Set(Vector2.zero.x, Vector2.zero.y);
            player.Look.Set(Vector2.zero.x, Vector2.zero.y);
            player.Sprint = false;
            player.Jump = false;
        }

        _constructionController.StopBuild();
    }

    private void Move(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            var vec2 = context.ReadValue<Vector2>();
            player.Move.Set(vec2.x, vec2.y);
        }
    }

    private void Look(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            var vec2 = context.ReadValue<Vector2>();
            player.Look.Set(vec2.x, vec2.y);
        }
    }

    private void Sprint(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Sprint = context.ReadValueAsButton();
        }
    }

    private void Jump(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is SceneObjectPawnPlayer player)
        {
            player.Jump = context.ReadValueAsButton();
        }
    }

    private void Scroll(CallbackContext context)
    {
        if (_playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent)
        {
            var scrollValue = context.ReadValue<Vector2>();
            if (scrollValue.y < 0)
            {
                itemToolComponent.ItemTool.ItemToolService?.NextAction(_playerController.CurrentPlayer.Value);
            }
            if (scrollValue.y > 0)
            {
                itemToolComponent.ItemTool.ItemToolService?.PrevAction(_playerController.CurrentPlayer.Value);
            }
        }
    }
}
