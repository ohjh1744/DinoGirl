using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 현재는 EnemyController.cs이나 추후 EnemyUnit등으로 수정할 예정
// 적 유닛이 전체적으로 가질 수 있는 함수를 포함하는 기본 스크립트로 제작할 예정
// - 플레이어의 Unit과 유사한 점이 존재할 것 같아서 한번에 제작할 수 있는지 확인 필요
public class EnemyController : MonoBehaviour
{
    [SerializeField] UnitSO unit;
    int curHp;

    private void Awake()
    {
        curHp = unit.hp;
    }


    public void TakeDamage(int damage)
    {
        Debug.Log("몬스터 체력 감소 이벤트 발생" + damage);
        curHp -= damage;
        Debug.Log("몬스터 체력 : " + curHp);
        if (curHp <= 0)
        {
            Debug.Log("몬스터 사망여부 판단");
            Die();
        }
        else return;
    }
    private void Die()
    {
        Debug.Log("몬스터 체력 : " + curHp);
        Debug.Log("몬스터 사망모션출력");
        Destroy(gameObject);
    }
}
