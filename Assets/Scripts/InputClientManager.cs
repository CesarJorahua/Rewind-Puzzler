using UnityEngine;
using RewindPuzzler.Input;
using UnityEngine.InputSystem;

public class InputClientManager : MonoBehaviour
{
    private RewindPuzzlerApp _rewindPuzzlerApp;
    private PlayerInputSystemActions inputActions;

    [SerializeField]
    private MovementReciever movementReciever;

    private void Awake()
    {
        _rewindPuzzlerApp =  new();
        inputActions = new();
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePlayer;
        inputActions.Player.Undo.performed += OnUndo;
    }

    private void OnMovePlayer(InputAction.CallbackContext context)
    {
        ICommand moveCommand = new MoveCommand(context.ReadValue<Vector2>(), movementReciever);
        _rewindPuzzlerApp.AddCommand(moveCommand);
    }

    private void OnUndo(InputAction.CallbackContext context)
    {
        _rewindPuzzlerApp.UndoCommand();
    }
}
