using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDisplayer : MonoBehaviour
{
    public AoESkill skill;
    public Transform flipStandard;
    public bool flipToRight;


    private void Update()
    {
        if (flipStandard.localScale.x < 0)
        {
            flipToRight = true;
        }
        else
        {
            flipToRight = false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 centerPos = transform.position;

        Vector2 dir = flipToRight ? Vector2.right : Vector2.left;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPos, skill.SkillRangeRadius);
        Gizmos.color = Color.red;

        float halfAngle = skill.areaAngle * 0.5f;

        int segmentCount = 3; // 부채꼴을 보이게 하도록 나눌 횟수
        float segmentDelta = (skill.areaAngle / segmentCount);

        for (int i = 0; i <= segmentCount; i++)
        {
            float angleStep = -halfAngle + (segmentDelta * i);
            float rad = angleStep * Mathf.Deg2Rad;

            if (dir == Vector2.left)
                rad += Mathf.PI;
            Vector2 rotatedDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Vector2 endPos = centerPos + rotatedDir * skill.SkillRangeRadius;
            Gizmos.DrawLine(centerPos, endPos);
        }
    }
}
