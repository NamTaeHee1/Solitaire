using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CardPackUI : MonoBehaviour
{
    [Header("CardPack 프로필")][SerializeField]
    private Image profile;

    [Header("CardPack Title")][SerializeField]
    private Text title;

    public Text downloadState;

    #region Info

    private CardPackInfo cardPackInfo;

    public void SetInfo(CardPackInfo info)
    {
        profile.sprite = info.cardPackProfile;

        Vector2 profileSize = new Vector2(info.cardPackProfile.rect.width,
                                          info.cardPackProfile.rect.height);

        profile.GetComponent<RectTransform>().sizeDelta = profileSize;

        title.text = info.cardPackName;

        cardPackInfo = info;

        StartCoroutine(CheckDownload());
    }

    #endregion

    #region Addressable

    private IEnumerator CheckDownload()
    {
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(cardPackInfo.cardPackSheet);

        yield return handle;

        downloadState.text = ((float)handle.Result).ToString();
    }

    #endregion
}
