using UnityEngine;

public enum EPointType
{
	Tableau = 0,
	Foundation,
	Waste,
	Stock,
	COUNT
}

public class Point : MonoBehaviour
{
	[Header("����Ʈ�� ī�尡 ���� �� Y���� ���� ������ �������°�")]
	public bool useCardYOffset;

	[Header("Point ����")]
	public EPointType pointType;

	public Card GetLastCard()
	{
		int lastCardIndex = base.transform.childCount - 1;

		if (lastCardIndex < 0)
			return null;

		return base.transform.GetChild(lastCardIndex).GetComponent<Card>();
	}

	/// <summary>
	/// ���� �˻��ϰ� �ִ� ī��� �� Point�� �����Ѱ� (�˻��� Point �������� ����)
	/// </summary>
	/// <param name="card"></param>
	/// <returns></returns>
	public virtual bool IsSuitablePoint(Card card) { return true; }
}
