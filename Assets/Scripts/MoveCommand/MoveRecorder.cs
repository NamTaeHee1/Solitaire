using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRecorder : MonoBehaviour
{
    public static MoveRecorder Instance
    {
        get
        {
            if(_instance == null)
                _instance = FindObjectOfType<MoveRecorder>();

            return _instance;
        }
    }
    private static MoveRecorder _instance;

    private Stack<MoveCardCommand> moveHistory = new Stack<MoveCardCommand>();

    public void PushMove(MoveCardCommand command)
    {
        moveHistory.Push(command);
    }

    public MoveCardCommand PopMove()
    {
        return moveHistory.Pop();
    }
}