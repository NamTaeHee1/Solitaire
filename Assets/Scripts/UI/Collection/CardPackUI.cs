using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class CardPackUI : MonoBehaviour
{
    #region Init

    private void Awake()
    {
        selectButton.onClick.AddListener(SelectButtonClick);
        selectButton.interactable = false;
    }

    #endregion

    #region Info

    private CardPackInfo cardPackInfo;

    [Header("CardPack 프로필")][SerializeField]
    private Image profile;

    [Header("CardPack Title")][SerializeField]
    private Text title;

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

    [HideInInspector]
    public float downloadSize;

    private IEnumerator CheckDownload()
    {
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(cardPackInfo.cardPackSheet);

        yield return handle;

        if(handle.Result > 0)
        {
            downloadSize = handle.Result / 1024f;

            SetState(ECARD_PACK_STATE.NOT_DOWNLOADED);
        }
        else
        {
            if(cardPackInfo.cardPackType == Data.CardPack)
            {
                SetState(ECARD_PACK_STATE.IN_USE);
            }
            else
            {
                SetState(ECARD_PACK_STATE.NOT_IN_USE);
            }
        }

        Addressables.Release(handle);
    }

    #endregion

    #region Select

    [Header("CardPack 선택 버튼")][SerializeField]
    private Button selectButton;

    [Header("선택 버튼에 들어갈 문구 Text")][SerializeField]
    private Text selectButtonText;

    [Header("CardPack Download Progress")][SerializeField]
    private Image downloadProgress;

    [HideInInspector]
    public CollectionPanel collectionPanel;

    private void SelectButtonClick()
    {
        if(currentState == ECARD_PACK_STATE.NOT_DOWNLOADED)
        {
            collectionPanel.ShowDownloadPopup(this,
                $"Card Pack {cardPackInfo.cardPackName} 을(를) 다운로드 ({downloadSize:F2} KB) 하시겠습니까?");
        }
        else if(currentState == ECARD_PACK_STATE.NOT_IN_USE)
        {
            AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetAsync<IList<Sprite>>(cardPackInfo.cardPackSheet);

            handle.Completed += (handle) =>
            {
                List<Sprite> list = new List<Sprite>();

                for (int i = 0; i < handle.Result.Count; i++)
                    list.Add(handle.Result[i]);

                collectionPanel.GetCardPackUI(Data.CardPack).SetState(ECARD_PACK_STATE.NOT_IN_USE);

                Data.ApplyCardPack(cardPackInfo.cardPackType);

                PlayerPrefs.SetInt("CardPack", (int)cardPackInfo.cardPackType);
                PlayerPrefs.Save();

                Managers.Game.SetCardPack(list);
                Managers.Point.stock.ApplyCardSheet();

                SetState(ECARD_PACK_STATE.IN_USE);
            };
        }
    }

    private IEnumerator ShowDownloadProgress(AsyncOperationHandle handle)
    {
        downloadProgress.fillAmount = 0f;

        while (handle.IsDone == false)
        {
            downloadProgress.fillAmount = handle.PercentComplete;

            yield return null;
        }

        downloadProgress.fillAmount = 1f;
    }

    #endregion

    #region State

    private readonly Color32 notDownloadColor = new Color32(127, 127, 127, 255);
    private readonly Color32 downloadColor = Color.white;

    private ECARD_PACK_STATE currentState;

    public void SetState(ECARD_PACK_STATE state)
    {
        currentState = state;

        profile.color = downloadColor;
        title.color = downloadColor;

        downloadProgress.gameObject.SetActive(false);
        selectButtonText.gameObject.SetActive(false);

        if (state == ECARD_PACK_STATE.NOT_DOWNLOADED)
        {
            profile.color = notDownloadColor;
            title.color = notDownloadColor;

            downloadProgress.gameObject.SetActive(true);

            selectButton.interactable = true;
        }
        else if(state == ECARD_PACK_STATE.NOT_IN_USE)
        {
            selectButtonText.gameObject.SetActive(true);
            selectButtonText.text = "사용하기";

            selectButton.interactable = true;
        }
        else
        {
            selectButtonText.gameObject.SetActive(true);
            selectButtonText.text = "사용 중";

            selectButton.interactable = false;
        }
    }

    #endregion

    #region Download

    public void DownloadCardPack()
    {
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(cardPackInfo.cardPackSheet);

        StartCoroutine(ShowDownloadProgress(handle));

        handle.Completed += (handle) =>
        {
            SetState(ECARD_PACK_STATE.NOT_IN_USE);
        };
    }

    #endregion
}
