using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPanel : MonoBehaviour
{
    #region Init

    private void Awake()
    {
        exitButton.onClick.AddListener(ExitButtonClick);

        downloadPopupCancel.onClick.AddListener(() => { downloadPopup.SetActive(false); });

        CreateCardPack();
    }

    public void OnEnable()
    {
        Time.timeScale = 0f;

        Managers.Input.BlockingInput++;
    }

    public void OnDisable()
    {
        Time.timeScale = 1f;

        Managers.Input.BlockingInput--;
    }

    #endregion

    #region Card Pack

    [Header("Card Pack ScrollView Content")][SerializeField]
    private Transform scrollViewContent;

    private Dictionary<ECARD_PACK_TYPE, CardPackUI> cardPackUIDict = new Dictionary<ECARD_PACK_TYPE, CardPackUI>();

    private void CreateCardPack()
    {
        GameObject cardPackPrefab = ResourcesCache<GameObject>.Load("Prefabs/CardPack");
        string[] packTypes = Enum.GetNames(typeof(ECARD_PACK_TYPE));

        for (int i = 0; i < packTypes.Length; i++)
        {
            CardPackUI pack = Instantiate(cardPackPrefab, scrollViewContent).GetComponent<CardPackUI>();
            CardPackInfo info = ResourcesCache<CardPackInfo>.Load($"SO/CardPack/{packTypes[i]}");

            pack.SetInfo(info);
            pack.collectionPanel = this;

            if(cardPackUIDict.ContainsKey(info.cardPackType) == false)
                cardPackUIDict.Add(info.cardPackType, pack);
        }
    }

    public CardPackUI GetCardPackUI(ECARD_PACK_TYPE type)
    {
        cardPackUIDict.TryGetValue(type, out CardPackUI ui);

        return ui;
    }

    #endregion

    #region Exit

    [Header("나가기 버튼")][SerializeField]
    private Button exitButton;

    private void ExitButtonClick()
    {
        Managers.Sound.Play("Press");

        gameObject.SetActive(false);
    }

    #endregion

    #region Download Popup

    [Header("Download Popup GameObject")][SerializeField]
    private GameObject downloadPopup;

    [Header("Download Popup 문구 Text")][SerializeField]
    private Text downloadPopupText;

    [Header("Download Popup 확인 버튼")][SerializeField]
    private Button downloadPopupConfirm;

    [Header("Download Popup 취소 버튼")][SerializeField]
    private Button downloadPopupCancel;

    public void ShowDownloadPopup(CardPackUI cardPackUI, string content)
    {
        downloadPopup.SetActive(true);

        downloadPopupText.text = content;

        downloadPopupConfirm.onClick.RemoveAllListeners();
        downloadPopupConfirm.onClick.AddListener(() =>
        {
            downloadPopup.SetActive(false);

            cardPackUI.DownloadCardPack();
        });
    }

    #endregion
}
