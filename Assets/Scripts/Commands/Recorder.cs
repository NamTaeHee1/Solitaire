using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    public static Recorder Instance
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<Recorder>();

            return _instance;
        }
    }
    private static Recorder _instance;

    private Stack<ICommand> commandHistory = new Stack<ICommand>();

    public void Push(ICommand command)
    {
        commandHistory.Push(command);
    }

    public ICommand Pop()
    {
        return commandHistory.Pop();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            ICommand command = Pop();

            command.Undo();
        }
    }
}