using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 임의의 테스트를 위해서 제작한 스크립트
/// Behavior Tree에서 이와 유사하게 사용 가능
/// </summary>
public class UnitManager : MonoBehaviour
{
    [SerializeField] private AtkSingleSkillSO skill;
    [SerializeField] private UnitSO unit;

    [SerializeField] EnemyList enemyList;
    [SerializeField] public List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        enemyList = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<EnemyList>(); // 임의의 태그 사용하여 적군 리스트 참조
    }

    private void Start()
    {
        enemies = enemyList.enemyList;
    }

    private void Update()
    {
        if (enemyList.isChanged)
        {
            enemies = enemyList.enemyList;
        }
        else return;
    }

    /// <summary>
    /// 임의로 버튼을 누르면 스킬이 발사되는 함수를 제작함
    /// - Behavior Tree에서 스킬을 사용하는 경우 이 함수를 사용하여 사용 가능
    /// </summary>
    public void OnSkillBtn()
    {
        Debug.Log("버튼입력");
        skill.DoSkill(unit.damage, enemies, gameObject);
        skill.DoAnimationSkill();
        skill.DoSoundSkill();
        //skill.OnSkill(unit.damage, enemies, gameObject);
        //skill.OnMotion();
    }
}
