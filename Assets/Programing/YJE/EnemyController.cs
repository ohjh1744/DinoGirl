using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
