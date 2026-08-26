using UnityEngine;

public class MoveCommand : ICommand
{
    MovementReciever _player;
    Vector2Int _movement;

    public MoveCommand(Vector2Int movement, MovementReciever manager)
    {
        _movement = movement;
        _player = manager;
    }

    public void Execute()
    {
        _player.Move(_movement);
    }

    public void Undo()
    {
        _player.Move(-_movement);
    }
}
