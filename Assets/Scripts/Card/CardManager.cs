using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject CardPrefab;

    public void CreateCard()
	{
        List<Sprite> CardImgList = new List<Sprite>(Resources.LoadAll<Sprite>("Cards"));
        Debug.Log(CardImgList.Count);
        int RandomNum;
        Sprite CurCardTexture;
        GameObject _Card;
        for (int i = 0; i < 52; i++)
		{
            RandomNum = Random.Range(0, CardImgList.Count);
            CurCardTexture = CardImgList[RandomNum];

            _Card = Instantiate(CardPrefab, this.transform);
            _Card.GetComponent<Card>().SetCardInfo(CurCardTexture, CurCardTexture.name);
            CardImgList.RemoveAt(RandomNum);
		}
	}

	private void Start()
	{
        CreateCard();
	}
}
