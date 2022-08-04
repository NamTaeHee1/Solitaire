using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;
    private const int CARD_MAX_SIZE = 52;

    private void CreateCards()
	{
        List<Sprite> CardImgList = new List<Sprite>(Resources.LoadAll<Sprite>("Cards"));
        int RandomNum;
        Sprite CurCardTexture;
        GameObject _Card;

        for (int i = 0; i < CARD_MAX_SIZE; i++)
		{
            RandomNum = Random.Range(0, CardImgList.Count);
            CurCardTexture = CardImgList[RandomNum];

            _Card = Instantiate(CardPrefab, transform);
            _Card.transform.GetComponent<Card>().SetCardInfo(CurCardTexture, CurCardTexture.name);
            CardImgList.RemoveAt(RandomNum);
		}
	}

    private void MoveCardToPoints()
	{

	}

    private void Start()
	{
        CreateCards();
     }
}