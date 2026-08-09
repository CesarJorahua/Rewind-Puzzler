using UnityEngine;

public class MovementReciever : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    private const float boardSpacing = 1f;
    public void Move(Vector2 movement)
    {
        transform.position = transform.position + new Vector3(movement.x,movement.y);
    }
    public bool IsValidMove(Vector2 movement)
    {
        return !Physics2D.Raycast(transform.position, movement, boardSpacing, obstacleMask);
    }
}
