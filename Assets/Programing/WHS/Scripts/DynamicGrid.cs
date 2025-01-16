using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour
{
    private RectTransform _parent;
    private GridLayoutGroup _grid;
    [SerializeField] private int _cols; // 세로 열 개수
    // [SerializeField] private int _itemCount = 0; // 셀 개수
    [SerializeField] private float _heightRate = 1;

    private void Awake()
    {
        _parent = gameObject.GetComponent<RectTransform>();
        _grid = gameObject.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        SetDynamicGrid();
    }

    private void OnRectTransformDimensionsChange()
    {
        /*
        #if UNITY_ANDROID
        SetDynamicGrid();
        #endif
        */

        // 화면의 크기가 바뀔때 그리드 크기 조정하려고 했음
        if (Application.platform == RuntimePlatform.Android)
        {
            SetDynamicGrid();
        }
    }

    private void SetDynamicGrid()
    {
        // int rows = Mathf.CeilToInt((float)_itemCount / _cols);

        // 셀 크기 계산
        float availableWidth = _parent.rect.width - (_grid.spacing.x * (_cols + 1));
        float cellWidth = availableWidth / _cols;
        float cellHeight = cellWidth;
        _grid.cellSize = new Vector2(cellWidth, cellHeight * _heightRate);

        // grid layout group의 padding을 spacing과 동일하게 설정
        int padding = Mathf.RoundToInt(_grid.spacing.x);
        _grid.padding = new RectOffset(padding, padding, padding, padding);

        _grid.constraintCount = _cols;
    }

    /* // 셀 개수 받아오기
    public void SetItemCount(int count)
    {
        _itemCount = count;
        SetDynamicGrid();
    }
    */
}
