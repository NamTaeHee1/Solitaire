using System.Collections;
using System.Collections.Generic;

public static class Recorder
{
    private static Stack<ICommand> commandHistory = new Stack<ICommand>();

    public static bool IsEmpty { get { return commandHistory.Count == 0; } }

    public static void Push(ICommand command)
    {
        commandHistory.Push(command);
    }

    public static ICommand Pop()
    {
        return commandHistory.Pop();
    }

    public static void ClearHistory()
    {
        commandHistory.Clear();
    }
}