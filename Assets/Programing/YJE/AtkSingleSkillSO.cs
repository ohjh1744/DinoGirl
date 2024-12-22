using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "AtkSingleSkill")]
public class AtkSingleSkillSO : SkillBaseSO
{
    public EnemyController enemyController;

    public override void DoSkill(int damage, List<GameObject> targetList, GameObject unit)
    {
        List<float> list = new List<float>();
        Debug.Log("가장 가까운 타겟 설정");
        for (int i = 0; i < targetList.Count; i++)
        {
            list.Add(Vector2.Distance(unit.transform.position, targetList[i].transform.position));
        }
        float value = list.Min();
        int index = list.IndexOf(value); // 최소값의 인덱스 번호

        GameObject target = targetList[index]; // 공격 할 타겟
        enemyController = target.GetComponent<EnemyController>();
        Debug.Log("스킬 실행" + target);
        Debug.Log("스킬 실행 - " + damage);
        enemyController.TakeDamage(damage);
    }
    public override void DoAnimationSkill()
    {
        Debug.Log("애니메이션 재생");
    }
    public override void DoSoundSkill()
    {
        Debug.Log("소리 재생");
    }
}
