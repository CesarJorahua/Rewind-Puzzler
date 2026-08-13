using RewindPuzzler.Core.EventBus;
using UnityEngine;

public class MovementReciever : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    private const float boardSpacing = 1f;

    private bool isAllowToMove = true;
    private EventBinding<ReachEndMaze> endMazeBiding;
    private EventBinding<ResetMaze> resetMazeBiding;

    void OnEnable()
    {
        endMazeBiding = new EventBinding<ReachEndMaze>(DisableMovement);
        resetMazeBiding = new EventBinding<ResetMaze>(EnableMovement);
        EventBus<ReachEndMaze>.Register(endMazeBiding);
        EventBus<ResetMaze>.Register(resetMazeBiding);
    }

    void OnDisable()
    {
        EventBus<ReachEndMaze>.Deregister(endMazeBiding);
        EventBus<ResetMaze>.Deregister(resetMazeBiding);
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

    private void EnableMovement()
    {
        isAllowToMove=true;
    }
}
