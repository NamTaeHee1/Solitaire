using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : Singleton<AddressableManager>
{
    public AsyncOperationHandle Init(Action<AsyncOperationHandle> after)
    {
        AsyncOperationHandle handle = Addressables.InitializeAsync();

        handle.Completed += after;

        return handle;
    }

    public AsyncOperationHandle GetDownloadSize(AssetReferenceSprite sprite, Action<AsyncOperationHandle<long>> after = null)
    {
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(sprite);

        handle.Completed += after;

        return handle;
    }

    public AsyncOperationHandle Download(AssetReferenceSprite sprite, Action<AsyncOperationHandle> after = null)
    {
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(sprite);
        
        handle.Completed += after;

        return handle;
    }

    /// <summary>
    /// IList<Sprite> 반환 후 List<Sprite>에 다시 담아야 함.
    /// </summary>
    public AsyncOperationHandle<IList<Sprite>> LoadAssetSpriteSheet(AssetReferenceSprite sprite, Action<AsyncOperationHandle<IList<Sprite>>> after = null)
    {
        AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(sprite);

        handle.Completed += after;

        return handle;
    }
}
