using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillRuntimeData
{
    private float[] _damageThresholds;  // ex) [0.3, 0.5, 0.7, 0.9]
    private int _currentIndex;
    

    public BossSkillRuntimeData(float startUpRatio, float recoveryRatio, float tickNumber)
    {
        InitTickDamageThresholds(startUpRatio, recoveryRatio, tickNumber);
        _currentIndex = 0;
    }

    private void InitTickDamageThresholds(float startUp, float recovery, float tickCount)
    {
        if (tickCount <= 0)
        {
            Debug.LogWarning("틱은 최소 1회 이상 주어야 함");
            return;
        }
        
        // tickCount=4, startUp=0.1, recovery=0.9 → (0.3,0.5,0.7,0.9) 같은 배열
        // (recoveryRatio - startUpRatio) / tickNumber : Done까지 normalizedTime이 해당 수치만큼 지날때마다 데미지 적용
        _damageThresholds = new float[(int)tickCount];
        float interval = (recovery - startUp) / tickCount;
        for (int i = 0; i < tickCount; i++)
        {
            _damageThresholds[i] = startUp + interval * (i + 1);
        }
    }

    /// <summary>
    /// 현재 normalizedTime이 각 threshold에 도달했는지 체크, 아직 데미지를 주지 않은 틱이면 데미지 적용
    /// </summary>
    public void CheckAndDealDamage(float normalizedTime, RaidBossUnitController raidBossCaster, BaseUnitController target, float skillRatio)
    {
        if (_damageThresholds == null || _damageThresholds.Length == 0)
            return;
        if (target == null || !target.gameObject.activeSelf)
            return;

        
        while (_currentIndex < _damageThresholds.Length && normalizedTime >= _damageThresholds[_currentIndex])
        {
            // 실제 데미지 주기
            float damage = raidBossCaster.UnitModel.AttackPoint * skillRatio; // skillRatio를 어떻게 쓰냐에 따라 /tickNum을 할수도 있음
            target.UnitModel.TakeDamage(Mathf.RoundToInt(damage)); 

            Debug.Log($" 틱 데미지 {_currentIndex+1}회 : {target.gameObject.name}에 데미지 {damage}");
            _currentIndex++;
        }
    }

    /// <summary>
    /// 남은 틱이 있는지 여부 등 필요하다면 추가 메서드
    /// </summary>
    public bool AllTicksDone()
    {
        return _currentIndex >= _damageThresholds.Length;
    }
}
