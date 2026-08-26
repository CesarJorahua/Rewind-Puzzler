using UnityEngine;
using RewindPuzzler.Input;
using UnityEngine.InputSystem;
using RewindPuzzler.Core.EventBus;

public class InputClientManager : MonoBehaviour
{
    [SerializeField]
    private SeededMazeGenerator maze;
    private RewindPuzzlerApp _rewindPuzzlerApp;
    private PlayerInputSystemActions _inputActions;
    private MovementReciever _movementReciever;
    private EventBinding<ResetMaze> _resetMazeBinding;
    private bool _endMazeFlag;
    public MovementReciever MovementReciever { get => _movementReciever; set => _movementReciever = value; }

    private void Awake()
    {
        _rewindPuzzlerApp =  new();
        _inputActions = new();
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMovePlayer;
        _inputActions.Player.Undo.performed += OnUndo;
    }

    void OnEnable()
    {
        _resetMazeBinding =  new EventBinding<ResetMaze>(Reset);
        EventBus<ResetMaze>.Register(_resetMazeBinding);
    }

    void OnDisable()
    {        
        EventBus<ResetMaze>.Deregister(_resetMazeBinding);
    }

    //Called on GenerateButton UI unityButton
    public void Reset()
    {
        _movementReciever.transform.position = maze.StartWorldPosition;
        _rewindPuzzlerApp.ResetStack();
        _endMazeFlag = false;
        _movementReciever.gameObject.SetActive(true);
        _movementReciever.EnableMovement();
    }

    private void OnMovePlayer(InputAction.CallbackContext context)
    {
        Vector2Int movement = ToCardinalStep(context.ReadValue<Vector2>());
        if(!_movementReciever)
            return;
        if (movement == Vector2Int.zero)
            return;
        if (_movementReciever.IsValidMove(movement))
        {
            ICommand moveCommand = new MoveCommand(movement, _movementReciever);
            _rewindPuzzlerApp.ExecuteCommand(moveCommand);

            // Check if player reached the end position after the move
            CheckIfPlayerReachedEnd();
        }
    }

    /// <summary>
    /// Turns raw input into a single one-unit step. Diagonals (both axes pressed) are
    /// ignored so the player can never move between two tiles at once.
    /// </summary>
    private static Vector2Int ToCardinalStep(Vector2 input)
    {
        const float threshold = 0.5f;
        bool horizontal = Mathf.Abs(input.x) >= threshold;
        bool vertical = Mathf.Abs(input.y) >= threshold;

        if (horizontal == vertical)
            return Vector2Int.zero;

        return horizontal
            ? new Vector2Int(input.x > 0f ? 1 : -1, 0)
            : new Vector2Int(0, input.y > 0f ? 1 : -1);
    }

    private void OnUndo(InputAction.CallbackContext context)
    {
        _rewindPuzzlerApp.UndoCommand();
    }

    private void CheckIfPlayerReachedEnd()
    {
        // Check if the player's current position matches the end position of the maze
        if (_movementReciever.transform.position == maze.EndWorldPosition)
        {
            if (!_endMazeFlag)
            {
                _endMazeFlag = true;
                // Raise event that player has reached the end of the maze
                EventBus<ReachEndMaze>.Raise(new ReachEndMaze());
                _movementReciever.gameObject.SetActive(false);
            }
        }
    }

    public void UIResetClick()
    {
        EventBus<ResetMaze>.Raise(new ResetMaze());
    }
}
