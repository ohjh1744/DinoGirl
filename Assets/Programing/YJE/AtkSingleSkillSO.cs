using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 가까운 적군에게 단일 딜을 넣은 Scriptable Object
/// - SkillBaseSO를 상속받아 생성하는 함수
/// - 현재는 에셋화 시킨 후 UnitManager와 함께 Unit에 넣어 참조한 후 
/// - 각 수치를 UnitManager에서 받아서 사용하는 방식으로 구현되어있음
/// </summary>
[CreateAssetMenu(menuName = "AtkSingleSkill")]
public class AtkSingleSkillSO : SkillBaseSO
{
    private EnemyController enemyController;

    public override void DoSkill(int damage, List<GameObject> targetList, GameObject unit)
    {
        List<float> list = new List<float>();
        Debug.Log("가장 가까운 타겟 설정");
        // 스킬을 사용하는 unit오브젝트와 적군 리스트 중 가장 가까히 존재하는 적군을 타겟으로 설정
        for (int i = 0; i < targetList.Count; i++)
        {
            list.Add(Vector2.Distance(unit.transform.position, targetList[i].transform.position));
        }
        float value = list.Min();
        int index = list.IndexOf(value); // 최소값의 인덱스 번호
        GameObject target = targetList[index]; // 공격 할 타겟

        // 공격할 타겟의 TakeDamage() 함수가 있는 스크립트를 참조
        // TODO : 현재는 EnemyController.cs이나 추후 EnemyUnit등으로 수정할 예정
        enemyController = target.GetComponent<EnemyController>(); 
        Debug.Log("스킬 실행" + target);
        Debug.Log("스킬 실행 - " + damage);

        // 공격받은 타겟의 TakeDamge() 실행
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
