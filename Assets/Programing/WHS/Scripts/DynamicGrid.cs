using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour
{
    private int _characterCount;
    private RectTransform _parent;
    private GridLayoutGroup _grid;

    private void Start()
    {
        _parent = gameObject.GetComponent<RectTransform>();
        _grid = gameObject.GetComponent<GridLayoutGroup>();

        _characterCount = GetCharacterCount();
        SetDynamicGrid();
    }


    private void OnRectTransformDimensionsChange()
    {
        /*
        #if UNITY_ANDROID
        SetDynamicGrid();
        #endif
        */

        if (Application.platform == RuntimePlatform.Android)
        {
            SetDynamicGrid();
        }
    }


    private void SetDynamicGrid()
    {
        if (_grid == null || _parent == null)
        {
            Debug.LogWarning("Grid or parent is null in SetDynamicGrid");
            return;
        }

        int cols = 5;
        int rows = Mathf.CeilToInt((float)_characterCount / cols);

        // 셀 크기 계산
        float availableWidth = _parent.rect.width - (_grid.spacing.x * (cols + 1));
        float cellWidth = availableWidth / cols;
        float cellHeight = cellWidth;
        _grid.cellSize = new Vector2(cellWidth, cellHeight);

        // grid layout group의 padding을 spacing과 동일하게 설정
        int padding = Mathf.RoundToInt(_grid.spacing.x);
        _grid.padding = new RectOffset(padding, padding, padding, padding);

        _grid.constraintCount = cols;
    }

    private int GetCharacterCount()
    {
        InventoryPanel inventoryPanel = FindObjectOfType<InventoryPanel>();
        if (inventoryPanel != null)
        {
            return inventoryPanel.GetCharacterCount();
        }
        return 0;
    }
}
