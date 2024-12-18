using System.Collections;
using System.Collections.Generic;

public class Recorder
{
    public static Recorder Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Recorder();

            return _instance;
        }
    }
    private static Recorder _instance;

    private Stack<ICommand> commandHistory = new Stack<ICommand>();

    public bool IsEmpty { get { return commandHistory.Count == 0; } }

    public void Push(ICommand command)
    {
        commandHistory.Push(command);
    }

    public ICommand Pop()
    {
        return commandHistory.Pop();
    }

    public void ClearHistory()
    {
        commandHistory.Clear();
    }
}