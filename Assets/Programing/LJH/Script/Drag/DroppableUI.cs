using ExitGames.Client.Photon;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DroppableUI : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
	[SerializeField] int gridNum; // 현재 그리드 번호 
	private	Image			image;
	private	RectTransform	rect;
	private bool isFull; // 현재 그리드에 오브젝트가 있는지 없는지

	private void Awake()
	{
		image	= GetComponent<Image>();
		rect	= GetComponent<RectTransform>();
		isFull = false;
		if (transform.childCount >= 1) 
		{
			isFull = true;
		}

    }

	/// <summary>
	/// 마우스 포인트가 현재 아이템 슬롯 영역 내부로 들어갈 때 1회 호출
	/// </summary>
	public void OnPointerEnter(PointerEventData eventData)
	{
		// 아이템 슬롯의 색상을 노란색으로 변경
		//image.color = Color.green;
	}

	/// <summary>
	/// 마우스 포인트가 현재 아이템 슬롯 영역을 빠져나갈 때 1회 호출
	/// </summary>
	public void OnPointerExit(PointerEventData eventData)
	{
		// 아이템 슬롯의 색상을 하얀색으로 변경
		//image.color = Color.black;
	}

	/// <summary>
	/// 현재 아이템 슬롯 영역 내부에서 드롭을 했을 때 1회 호출
	/// </summary>
	public void OnDrop(PointerEventData eventData)
	{
		if (isFull == true)  
		{
			return;
		}
		if (gridNum == 0) 
		{
			return;
		}
		// pointerDrag는 현재 드래그하고 있는 대상(=아이템)
		if ( eventData.pointerDrag != null )
		{
			// 드래그하고 있는 대상의 부모를 현재 오브젝트로 설정하고, 위치를 현재 오브젝트 위치와 동일하게 설정
			eventData.pointerDrag.transform.SetParent(transform);
			eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;

			BattleSceneManager.Instance.inGridObject[gridNum] = eventData.pointerDrag;
			isFull = true;
		}
	}


    public void OnTransformChildrenChanged() // 자식의 수가 변경될때마다 호출
    {
		if (transform.childCount == 0)
		{			
            BattleSceneManager.Instance.inGridObject[gridNum] = null; //그리드에서 빠지면 빼기
			isFull=false;
        }
		else if (transform.childCount >= 1) 
		{
            isFull = true;
        }
    }

}

