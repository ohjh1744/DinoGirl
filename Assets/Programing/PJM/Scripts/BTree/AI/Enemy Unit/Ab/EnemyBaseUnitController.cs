using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseUnitController : BaseUnitController
{
    
    protected override BaseNode SetBTree()
    {
        Debug.LogWarning("자식에서 구현하세요 Btree");
        throw new System.NotImplementedException();
    }
    
    protected override BaseNode.ENodeState SetDetectedTarget()
    {
        if ((UnitModel.CurCc & CrowdControls.Taunt) != 0) // 걸린 상태이상 중 도발이 있을경우
        {
            if (UnitModel.CcCaster != null && UnitModel.CcCaster.gameObject.activeSelf) // 도발을 건 대상이 유효한 대상일 때
            {
                DetectedEnemy = UnitModel.CcCaster;
            }
        }
        // 이미 감지된 적이 있었을경우엔 수행할 필요 없음,  바로 chase로 전환
        if(DetectedEnemy != null && DetectedEnemy.gameObject.activeSelf)
            return BaseNode.ENodeState.Success;
        
        
        if (BattleSceneManager.Instance.myUnits.Count == 0)
            return BaseNode.ENodeState.Failure;
        
        float minDistance = float.MaxValue;
        float maxDistance = float.MinValue;
        BaseUnitController closetEnemy = null;
        BaseUnitController farthestEnemy = null;

        foreach (var unit in BattleSceneManager.Instance.myUnits)
        {
            if (unit == null || !unit.gameObject.activeSelf)
                continue;

            float distance = Vector2.Distance(transform.position, unit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closetEnemy = unit;
            }

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestEnemy = unit;
            }
        }

        if (UnitModel.IsPriorityTargetFar)
        {
            // 가장 먼 타겟을 DetectedEnemy 로 설정
            DetectedEnemy = farthestEnemy;
        }
        else
        {
            // 가장 가까운 타겟을 DetectedEnemy로 설정
            DetectedEnemy = closetEnemy;
        }

        if (DetectedEnemy == null) 
            return BaseNode.ENodeState.Failure;
        
        UnitViewer.CheckNeedFlip(transform, DetectedEnemy.transform);
        return BaseNode.ENodeState.Success;
    }
}
