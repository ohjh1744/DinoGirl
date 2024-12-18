using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BuffShape
{
    FrontAll,
    BackAll,
    Size
}

public class BuffManager : MonoBehaviour
{
    // 그리드 만들기
    private GridCell[,] CreateGrid(int width, int height)
    {
        GridCell[,] grid= new GridCell[width, height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = new GridCell(new Vector2Int(x, y));
            }
        }
        return grid;
    }

    public void ApplyBuff(Vector2Int characterGridPosition, BuffShape buffShape, GridCell[,] grid)
    {
        Vector2Int[] buffPositions = GetRelativePositionToBuff(buffShape);

        foreach (Vector2Int relativePosition in buffPositions)
        {
            // 실제 버프를 줄 위치 계산
            Vector2Int targetPosition = characterGridPosition + relativePosition;

            if (!IsInGrid(targetPosition, grid) /* 상대 위치가 셀의 범위 (3x3) 범위를 벗어 날 경우*/)
            {
                // 넘어감
                continue;
            }
            
            
            GridCell targetCell = grid[targetPosition.x, targetPosition.y];
            
            if (targetCell.IsDeployed/* 해당 위치에 캐릭터가 있으면*/)
            {
                // 버프 적용
                targetCell.ApplyBuff();
            }
        }
    }

    private bool IsInGrid(Vector2Int targetPosition, GridCell[,] grid)
    {
        int xSize = grid.GetLength(0);
        int ySize = grid.GetLength(1);

        if (targetPosition.x < 0 || targetPosition.x >= xSize || targetPosition.y < 0 || targetPosition.y >= ySize)
            return false;

        return true;
        
    }

    private Vector2Int[] GetRelativePositionToBuff(BuffShape buffShpae)
    {
        switch (buffShpae)
        {
            /*
             * (0,0) (1,0) (2,0)
             * (0,1) (1,1) (2,1)
             * (0,2) (1,2) (2,2)
             */

            // 키패드 기준
            case BuffShape.FrontAll:
                // 전방 세 칸에 버프를 적용해야 할 경우
                return new Vector2Int[]
                {
                    new(1, -1),
                    new(1, 0),
                    new(1, 1)
                };

            case BuffShape.BackAll:
                return new Vector2Int[]
                {
                    new(-1, -1),
                    new(-1, 0),
                    new(-1, 1)
                };
            
            // Todo : 에러처리?
            default: return null;
        }
    }
}