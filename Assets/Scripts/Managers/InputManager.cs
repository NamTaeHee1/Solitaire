using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
	[Header("클릭한 Card")][SerializeField]
	private Card clickedCard;

    [Header("Touch 허용 여부")]
    public ETOUCH_STATE touchState;

	private LayerMask cardLayer;

	private Camera mainCam;

	private Vector3 inputOffset;

    public bool IsBlocking { get { return BlockingInput != 0; } }

	public int BlockingInput
    {
        get { return blockingInput; }
        set
        {
            blockingInput = Mathf.Clamp(value, 0, value);
        }
    }
    private int blockingInput;

	private void Awake()
	{
		cardLayer = 1 << LayerMask.NameToLayer("Card");

		mainCam = Camera.main;
	}

    private void Update()
	{
		if (BlockingInput > 0)
		{
			if (clickedCard != null)
			{
				clickedCard.OnClickUp(GetMousePos());
				clickedCard = null;
			}

            return;
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
				clickedCard.OnClickUp(GetMousePos());
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

            Vector3 mousePos = GetMousePos();

			clickedCard.OnClickDown(mousePos);

			inputOffset = clickedCard.transform.position - mousePos;
		}
	}

	private bool CheckBlockingInput(Card inputCard)
	{
		return (inputCard.cardState != ECARD_MOVE_STATE.MOVING &&
				inputCard.cardDirection == ECARD_DIRECTION.FRONT &&
				inputCard.transform.GetSiblingIndex() == inputCard.curPoint.transform.childCount - 1);
	}
	
	private Vector3 GetMousePos()
	{
		return mainCam.ScreenToWorldPoint(Input.mousePosition);
	}
}
