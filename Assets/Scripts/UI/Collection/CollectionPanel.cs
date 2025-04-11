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

    private void CreateCardPack()
    {
        GameObject cardPackPrefab = ResourcesCache<GameObject>.Load("Prefabs/CardPack");
        string[] packTypes = Enum.GetNames(typeof(ECARD_PACK_TYPE));

        for (int i = 0; i < packTypes.Length; i++)
        {
            CardPackUI pack = Instantiate(cardPackPrefab, scrollViewContent).GetComponent<CardPackUI>();
            CardPackInfo info = ResourcesCache<CardPackInfo>.Load($"SO/CardPack/{packTypes[i]}");

            pack.SetInfo(info);
        }
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
}
