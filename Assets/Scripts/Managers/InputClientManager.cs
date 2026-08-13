using UnityEngine;
using RewindPuzzler.Input;
using UnityEngine.InputSystem;
using RewindPuzzler.Core.EventBus;

public class InputClientManager : MonoBehaviour
{
    private RewindPuzzlerApp _rewindPuzzlerApp;
    private PlayerInputSystemActions inputActions;

    [SerializeField]
    private MovementReciever movementReciever;

    [SerializeField]
    private SeededMazeGenerator maze;

    private EventBinding<ResetMaze> resetMazeBinding;

    private bool endMazeFlag;

    private void Awake()
    {
        _rewindPuzzlerApp =  new();
        inputActions = new();
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePlayer;
        inputActions.Player.Undo.performed += OnUndo;
        Reset();
    }

    void OnEnable()
    {
        resetMazeBinding =  new EventBinding<ResetMaze>(Reset);
        EventBus<ResetMaze>.Register(resetMazeBinding);
    }

    void OnDisable()
    {        
        EventBus<ResetMaze>.Deregister(resetMazeBinding);
    }

    //Called on GenerateButton UI unityButton
    public void Reset()
    {
        movementReciever.transform.position = maze.StartWorldPosition;
        _rewindPuzzlerApp.ResetStack();
        endMazeFlag = false;
    }

    private void OnMovePlayer(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();
        if (movementReciever.IsValidMove(movement))
        {
            ICommand moveCommand = new MoveCommand(movement, movementReciever, maze);
            _rewindPuzzlerApp.ExecuteCommand(moveCommand);

            // Check if player reached the end position after the move
            CheckIfPlayerReachedEnd();
        }
    }

    private void OnUndo(InputAction.CallbackContext context)
    {
        _rewindPuzzlerApp.UndoCommand();
    }

    private void CheckIfPlayerReachedEnd()
    {
        // Check if the player's current position matches the end position of the maze
        if (movementReciever.transform.position == maze.EndWorldPosition)
        {
            if (!endMazeFlag)
            {
                endMazeFlag = true;
                // Raise event that player has reached the end of the maze
                EventBus<ReachEndMaze>.Raise(new ReachEndMaze());
            }
        }
    }

    public void UIResetClick()
    {
        EventBus<ResetMaze>.Raise(new ResetMaze());
    }
}
