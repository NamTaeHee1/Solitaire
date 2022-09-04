using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private Point[] KPoints;
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
        float WaitTime = 0;

        for (int i = 0; i < KPoints.Length; i++)
		{
            for (int j = i; j < KPoints.Length; j++)
			{
                Card _Card = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
                _Card.Move(KPoints[j], WaitTime += 0.05f);
                _Card.StartCoroutine(_Card.Show(j == i ? CardEnum.CardDirection.FRONT : CardEnum.CardDirection.BACK, WaitTime));
               }
		}
	}

    private void Start()
	{
        CreateCards();
        MoveCardToPoints();
     }
}