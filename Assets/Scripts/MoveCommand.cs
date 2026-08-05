using UnityEngine;

public class MoveCommand : ICommand
{
    MovementReciever _player;
    Vector2 _movement;
    public MoveCommand(Vector2 movement, MovementReciever manager)
    {
        _movement = movement;
        _player = manager;
    }

    public void Execute()
    {
        _player.MovePlayer(_movement);
    }

    public void Undo()
    {
        _player.MovePlayer(_movement*-1f);
    }
}
