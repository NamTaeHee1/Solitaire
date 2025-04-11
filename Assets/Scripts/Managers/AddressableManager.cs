using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class AddressableManager : Singleton<AddressableManager>
{
    public AsyncOperationHandle Init(Action<AsyncOperationHandle> after)
    {
        AsyncOperationHandle handle = Addressables.InitializeAsync();

        handle.Completed += after;

        return handle;
    }

    public AsyncOperationHandle CheckDownload()
    {
        return new AsyncOperationHandle();
    }

    public AsyncOperationHandle Download()
    {
        return new AsyncOperationHandle();
    }
}
