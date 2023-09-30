using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    private GameObject cardPrefab;
    private const int CARD_MAX_SIZE = 52;

    private void Start()
     {
        CreateCards();
        MoveCardToPoints();
     }

    private void CreateCards()
	{
        List<Sprite> cardImgList = new List<Sprite>(Resources.LoadAll<Sprite>("Cards"));
        int randomNum;
        Sprite curCardTexture;
        GameObject _card;

        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");

        for (int i = 0; i < CARD_MAX_SIZE; i++)
		{
            randomNum = Random.Range(0, cardImgList.Count);
            curCardTexture = cardImgList[randomNum];

            _card = Instantiate(cardPrefab, transform);
            _card.transform.GetComponent<Card>().SetCardInfo(curCardTexture, curCardTexture.name);
            cardImgList.RemoveAt(randomNum);
		}
	}

    private void MoveCardToPoints()
	{
        float waitTime = 0;
        Point[] kPoints = PointManager.Instance.K;

        for (int i = 0; i < kPoints.Length; i++)
		{
            for (int j = i; j < kPoints.Length; j++)
			{
                Card _Card = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
                _Card.Move(kPoints[j], waitTime += 0.05f);
                _Card.StartCoroutine(_Card.Show(j == i ? CardEnum.ECardDirection.FRONT : CardEnum.ECardDirection.BACK, waitTime));
               }
		}
	}
}