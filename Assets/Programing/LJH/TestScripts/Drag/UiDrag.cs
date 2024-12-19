using UnityEngine;
using UnityEngine.EventSystems;

public class UiDrag: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // UI 요소의 RectTransform
    private Canvas canvas;               // UI가 속한 캔버스
    private CanvasGroup canvasGroup;     // 드래그 중 투명도 및 상호작용 제어

    private Vector2 basePos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>(); // 부모 캔버스 가져오기
        canvasGroup = GetComponent<CanvasGroup>(); // CanvasGroup은 선택 사항
        basePos = transform.position;   
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 상호작용 비활성화 (필요 시)
        canvasGroup.alpha = 0.3f;       // 투명도 조절
        canvasGroup.blocksRaycasts = false; // 드래그 중 다른 UI와 충돌 방지
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중 UI 위치 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 후 복구
        canvasGroup.alpha = 1.0f;       // 투명도 원래대로
        canvasGroup.blocksRaycasts = true; // 상호작용 복구
        //transform.position =  basePos;
    }

   
}