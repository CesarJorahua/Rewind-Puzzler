using RewindPuzzler.Core.EventBus;
using UnityEngine;

public class MovementReciever : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    private const float boardSpacing = 1f;

    private bool isAllowToMove = false;
    private EventBinding<ReachEndMaze> endMazeBiding;
    private EventBinding<ResetMaze> resetMazeBiding;
    private EventBinding<GameStarted> gameStartBiding;

    void OnEnable()
    {
        endMazeBiding = new EventBinding<ReachEndMaze>(DisableMovement);
        resetMazeBiding = new EventBinding<ResetMaze>(EnableMovement);
        gameStartBiding = new EventBinding<GameStarted>(EnableMovement);
        EventBus<ReachEndMaze>.Register(endMazeBiding);
        EventBus<ResetMaze>.Register(resetMazeBiding);
        EventBus<GameStarted>.Register(gameStartBiding);
    }

    void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(endMazeBiding);
        EventBus<ResetMaze>.Deregister(resetMazeBiding);
        EventBus<GameStarted>.Deregister(gameStartBiding);
    }


    public void Move(Vector2 movement)
    {
        if(!isAllowToMove)
            return;
        transform.position = transform.position + new Vector3(movement.x,movement.y);
    }
    public bool IsValidMove(Vector2 movement)
    {
        return !Physics2D.Raycast(transform.position, movement, boardSpacing, obstacleMask);
    }

    private void DisableMovement()
    {
        isAllowToMove = false;
    }

    public void EnableMovement()
    {
        isAllowToMove=true;
    }

    public void SetObstacleMask(LayerMask mask)
    {
        obstacleMask = mask;
    }
}
