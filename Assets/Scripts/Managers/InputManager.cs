using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InputState
{
	IDLE,
	CLICKED,
	DRAGING
}

public class InputManager : MonoBehaviour
{
	public static InputManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<InputManager>();

			return _instance;
		}
	}

	private static InputManager _instance;

	[Header("클릭한 Card")]
	[SerializeField]
	private Card clickedCard;

	private LayerMask cardLayer;

	private Camera mainCam;

	private Vector3 inputOffset;

	[Header("현재 클릭 가능한가")]
	public bool canInput;

	private void Awake()
	{
		cardLayer = 1 << LayerMask.NameToLayer("Card");

		mainCam = Camera.main;
	}

    private void Update()
	{
		if (canInput == false)
		{
			if (clickedCard != null)
			{
				clickedCard.OnClickUp();
				clickedCard = null;
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			RaycastCard();
		}
		else if (Input.GetMouseButton(0))
		{
			Vector3 mousePos = GetMousePos();

			if (clickedCard != null)
				clickedCard.OnClicking(mousePos + inputOffset);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (clickedCard != null)
			{
				clickedCard.OnClickUp();
				clickedCard = null;
			}
		}
	}

	private void RaycastCard()
	{
		RaycastHit2D hit = Utils.RaycastMousePos(mainCam, cardLayer);

		if (hit)
		{
			clickedCard = hit.transform.GetComponent<Card>();

			if (CheckBlockingInput(clickedCard) == false)
			{
				clickedCard = null;
				return;
			}

			clickedCard.OnClickDown();

			inputOffset = clickedCard.transform.position - GetMousePos();
		}
	}

	private bool CheckBlockingInput(Card inputCard)
	{
		return (inputCard.cardState != ECardMoveState.MOVING &&
				inputCard.cardDirection == ECardDirection.FRONT &&
				inputCard.transform.GetSiblingIndex() == inputCard.curPoint.transform.childCount - 1);
	}
	
	private Vector3 GetMousePos()
	{
		return mainCam.ScreenToWorldPoint(Input.mousePosition);
	}
}
