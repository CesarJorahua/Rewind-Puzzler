using UnityEngine;

public class MoveCommand : ICommand
{
    MovementReciever _player;
    Vector2 _movement;
    SeededMazeGenerator _maze;

    public MoveCommand(Vector2 movement, MovementReciever manager, SeededMazeGenerator maze)
    {
        _movement = movement;
        _player = manager;
        _maze = maze;
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
