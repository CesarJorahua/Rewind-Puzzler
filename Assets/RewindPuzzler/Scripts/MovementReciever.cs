using RewindPuzzler.Core.EventBus;
using UnityEngine;

public class MovementReciever : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    private const float BoardSpacing = 1f;

    private bool _isAllowToMove = false;
    private EventBinding<ReachEndMaze> _endMazeBiding;
    private EventBinding<ResetMaze> _resetMazeBiding;
    private EventBinding<GameStarted> _gameStartBiding;

    void OnEnable()
    {
        _endMazeBiding = new EventBinding<ReachEndMaze>(DisableMovement);
        _resetMazeBiding = new EventBinding<ResetMaze>(EnableMovement);
        _gameStartBiding = new EventBinding<GameStarted>(EnableMovement);
        EventBus<ReachEndMaze>.Register(_endMazeBiding);
        EventBus<ResetMaze>.Register(_resetMazeBiding);
        EventBus<GameStarted>.Register(_gameStartBiding);
    }

    void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(_endMazeBiding);
        EventBus<ResetMaze>.Deregister(_resetMazeBiding);
        EventBus<GameStarted>.Deregister(_gameStartBiding);
    }


    public void Move(Vector2Int movement)
    {
        if(!_isAllowToMove)
            return;
        transform.position += new Vector3(movement.x, movement.y) * BoardSpacing;
    }
    public bool IsValidMove(Vector2 movement)
    {
        return !Physics2D.Raycast(transform.position, movement, BoardSpacing, obstacleMask);
    }

    private void DisableMovement()
    {
        _isAllowToMove = false;
    }

    public void EnableMovement()
    {
        gameObject.SetActive(true);
        _isAllowToMove=true;
    }

    public void SetObstacleMask(LayerMask mask)
    {
        obstacleMask = mask;
    }
}
