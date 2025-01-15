using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUnitStatusBarController : UIBInder //MonoBehaviour임
{
    [SerializeField] private Slider _bossHpBar;
    [SerializeField] private Slider _bossManaBar;
    [SerializeField] private RaidBossUnitController _boss;
    private void Awake()
    {
        Bind();
        if(_bossHpBar == null)
            _bossHpBar = GetUI<Slider>("BossHPBar");
        if(_bossManaBar == null)
            _bossManaBar = GetUI<Slider>("BossMPBar");
    }

    private void OnEnable()
    {
        SubscribeEvents(); 
    }

    private void Start()
    {
        // Test용도
        //AllocateBoss(); 
        //SubscribeEvents();
    }

    private void Update()
    {
        if(_boss == null)
            return;

        _bossManaBar.value = -1 * _boss.CoolTimeCounter;
    }

    private void AllocateBoss()
    {
        foreach (var unit in BattleSceneManager.Instance.enemyUnits)
        {
            if (unit is not RaidBossUnitController boss) 
                continue;
            _boss = boss;
            break;
        }

        if (_boss == null)
        {
            Debug.Log("보스 유닛 미할당");
            return;
        }
        
        _boss.UnitModel.OnHpChanged += UpdateHealthBar;
        _boss.OnNextSkillSelected += HandleAllocatingNextSkill;
        _boss.UnitModel.OnDeath += HandleDeath;

        _bossHpBar.maxValue = _boss.UnitModel.MaxHp;
        _bossHpBar.value = _boss.UnitModel.Hp;
        _bossHpBar.minValue = 0;

        _bossManaBar.maxValue = 0;
        _bossManaBar.value = _boss.CoolTimeCounter;
        _bossManaBar.minValue = -1 * Mathf.RoundToInt(_boss.CurSkill.Cooltime); 

    }

    private void SubscribeEvents()
    {
        Spawner.OnSpawnCompleted += AllocateBoss;
    }
    
    private void UpdateHealthBar(int changedValue)
    {
        _bossHpBar.value = changedValue;
    }

    private void HandleAllocatingNextSkill(Skill nextSkill)
    {
        _bossManaBar.minValue = -1 * Mathf.RoundToInt(nextSkill.Cooltime);
    }

    private void UpdateManaBar(float changedValue)
    {
        // 쿨타임은 델타타임연산으로 변하는데 그때마다 이벤트 호출이면 너무 많다
        // 그냥 보스의 쿨타임값을 그대로 value에 적용하고싶은데
        // 델타타임은 float 연산인데 var의 value는 int로 하고싶다
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
