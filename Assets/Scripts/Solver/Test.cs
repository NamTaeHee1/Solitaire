using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    [Serializable]
    public struct Info
    {
        public string name;
        public int uid;

        public Info(string _name, int _uid)
        {
            name = _name;
            uid = _uid;
        }
    }

    [Serializable]
    public struct InfoState
    {
        public List<Info> infoList;
    }

    public List<Info> tableauList = new List<Info>();
    public List<Info> foundationsList = new List<Info>();

    public Action testAction;

    List<int> nums = new List<int>();

    private void Start()
    {
        nums.Add(1);
        nums.Add(1);
        nums.Add(3);
        nums.Add(9);
        nums.Add(55);

        testAction = () => { Debug.Log(nums[2]); };

        testAction.Invoke();
    }

    public void InitAction()
    {
        nums[2] = 99999;

        testAction.Invoke();

        nums.Insert(2, 4124124);

        testAction.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            List<Info> list = GetList().infoList;

            Info info = list[0];

            info.uid = 3;

            list[0] = info;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            InitAction();

            testAction.Invoke();
        }
    }

    private InfoState GetList()
    {
        InfoState state = new InfoState();

        state.infoList = tableauList;

        return state;
    }

    private void Swap(int info1, int info2)
    {
        Info temp = tableauList[info1];
        tableauList[info1] = tableauList[info2];
        tableauList[info2] = temp;
    }
}
