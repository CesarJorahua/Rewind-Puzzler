using UnityEngine;

public class MovementReciever : MonoBehaviour
{
    public void MovePlayer(Vector2 movement)
    {
        transform.position+=new Vector3(movement.x,movement.y,0f);
    }
}
