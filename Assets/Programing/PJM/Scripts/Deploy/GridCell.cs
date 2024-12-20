using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    private Vector2Int _position;
    private UnitController _unit;
    public Vector2Int Position { get => _position; private set => _position = value; }
    private bool _isDeployed;
    
    public bool IsDeployed { get => _isDeployed; private set => _isDeployed = value; }

    public GridCell(Vector2Int position)
    {
        Position = position;
        IsDeployed = false;
    }
    
    public void ApplyBuff()
    {
        // Todo : 버프적용 로직
        Debug.Log("버프 적용됨");
    }
}
