using System.Collections.Generic;

// Invoker class for the app
public class RewindPuzzlerApp
{
    Stack<ICommand> _commands;

    public RewindPuzzlerApp()
    {
        _commands =  new Stack<ICommand>();
    }

    public void AddCommand(ICommand command)
    {
        command.Execute();
        _commands.Push(command);
    }

    public void UndoCommand()
    {
        if (_commands.Count>0)
        {
            ICommand lastCommand = _commands.Pop();
            lastCommand.Undo();
        }
    }
}
