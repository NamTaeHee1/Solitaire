using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : Singleton<AddressablesManager>
{
    /// <summary>
    /// Addressables 기능들을 사용하기 위한 초기화 함수 (호출하지 않고 다른 기능을 호출하면 자동으로 초기화 함수가 호출)
    /// </summary>
    public AsyncOperationHandle Init(Action<AsyncOperationHandle> after = null)
    {
        AsyncOperationHandle handle = Addressables.InitializeAsync();

        if (after != null) handle.Completed += after;

        return handle;
    }

    /// <summary>
    /// sprite 다운로드 사이즈 반환, 완료하고 after 있으면 실행
    /// </summary>
    public AsyncOperationHandle<long> GetDownloadSize(AssetReferenceSprite sprite, Action<AsyncOperationHandle<long>> after = null)
    {
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(sprite);

        if (after != null) handle.Completed += after;

        return handle;
    }

    /// <summary>
    /// sprite 다운하고, 완료하고 after 있으면 실행
    /// </summary>
    public AsyncOperationHandle Download(AssetReferenceSprite sprite, Action<AsyncOperationHandle> after = null)
    {
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(sprite);

        if (after != null) handle.Completed += after;

        return handle;
    }

    /// <summary>
    /// IList<Sprite> 반환 후 List<Sprite>에 다시 담아야 함.
    /// </summary>
    public AsyncOperationHandle<IList<Sprite>> LoadAssetSpriteSheet(AssetReferenceSprite sprite, Action<AsyncOperationHandle<IList<Sprite>>> after = null)
    {
        AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(sprite);

        if(after != null) handle.Completed += after;

        return handle;
    }
}
