using UnityEngine.InputSystem;

public class InputController : IServiceScoped
{
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;

    public InputAction Move { get; }
    public InputAction Look { get; }
    public InputAction FirstAction { get; }
    public InputAction SecondAction { get; }
    public InputAction Scroll { get; }
    public InputAction Interact { get; }
    public InputAction Crouch { get; }
    public InputAction Jump { get; }
    public InputAction Previous { get; }
    public InputAction Next { get; }
    public InputAction Sprint { get; }
    public InputAction Inventory { get; }
    public InputAction UICancel { get; }

    public InputController()
    {
        _playerActionMap = InputSystem.actions.FindActionMap("Player");
        _uiActionMap = InputSystem.actions.FindActionMap("UI");

        Move = _playerActionMap.FindAction("Move");
        Look = _playerActionMap.FindAction("Look");
        FirstAction = _playerActionMap.FindAction("First Action");
        SecondAction = _playerActionMap.FindAction("Second Action");
        Scroll = _playerActionMap.FindAction("Scroll");
        Interact = _playerActionMap.FindAction("Interact");
        Crouch = _playerActionMap.FindAction("Crouch");
        Jump = _playerActionMap.FindAction("Jump");
        Previous = _playerActionMap.FindAction("Previous");
        Next = _playerActionMap.FindAction("Next");
        Sprint = _playerActionMap.FindAction("Sprint");
        Inventory = _playerActionMap.FindAction("Inventory");
        UICancel = _uiActionMap.FindAction("Cancel");
    }
}
