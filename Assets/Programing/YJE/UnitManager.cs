using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private AtkSingleSkillSO skill;
    [SerializeField] private UnitSO unit;

    [SerializeField] EnemyList enemyList;
    [SerializeField] public List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        enemyList = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<EnemyList>(); // 임의의 태그 사용
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
