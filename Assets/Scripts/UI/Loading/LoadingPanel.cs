using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    private IEnumerator Start()
    {
#if UNITY_EDITOR
        Caching.ClearCache();
#endif
        Data.Load();

        yield return Addressables.InitializeAsync();

        CardPackInfo info = ResourcesCache<CardPackInfo>.Load($"SO/CardPack/{Data.CardPack}");
        AsyncOperationHandle<long> getSizeHandle = Addressables.GetDownloadSizeAsync(info.cardPackSheet);

        StartCoroutine(SetProgressBar(getSizeHandle, "Card Pack 다운로드 확인 중..."));

        yield return getSizeHandle;


        if (getSizeHandle.Status == AsyncOperationStatus.Succeeded && getSizeHandle.Result > 0)
        {
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(info.cardPackSheet);

            StartCoroutine(SetProgressBar(downloadHandle, "Card Pack 다운로드 중..."));

            yield return downloadHandle;

            Addressables.Release(downloadHandle);
        }

        Addressables.Release(getSizeHandle);

        AsyncOperationHandle<IList<Sprite>> loadHandle = Addressables.LoadAssetAsync<IList<Sprite>>(info.cardPackSheet);
        List<Sprite> cardPackSheet = new List<Sprite>();

        StartCoroutine(SetProgressBar(loadHandle, "Card Pack 불러오는 중..."));

        yield return loadHandle;

        for (int i = 0; i < loadHandle.Result.Count; i++)
        {
            cardPackSheet.Add(loadHandle.Result[i]);
        }

        Managers.Game.SetCardPack(cardPackSheet);
        Managers.Game.StartGame(false, true);

        gameObject.SetActive(false);
    }

    [Header("현재 진행 Progress Bar")][SerializeField]
    private Slider progressBar;

    [Header("현재 진행 중인 내용 Text")][SerializeField]
    private Text progressContentText;

    private IEnumerator SetProgressBar(AsyncOperationHandle handle, string progressContent)
    {
        string text = $"{progressContent} {(handle.PercentComplete * 100f):F1}%";
        
        progressBar.value = 0f;
        progressContentText.text = text;

        while (handle.IsDone == false)
        {
            progressBar.value = handle.PercentComplete;

            progressContentText.text = text;

            yield return null;
        }

        progressBar.value = 1f;
    }
}
