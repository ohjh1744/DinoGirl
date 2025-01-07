using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Transform[] myGrid;
    [SerializeField] Transform[] enemyGrid;
    public static event Action OnSpawnCompleted;
    public void SpawnUnits()
    {
        Time.timeScale = 0f;
        for (int i = 0; i < BattleSceneManager.Instance.myUnitData.Count; i++)
        {

            int id = BattleSceneManager.Instance.myUnitData[i].Id;
            int pos = BattleSceneManager.Instance.myUnitData[i].Pos;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Character_" + id.ToString()), myGrid[pos-1].position, Quaternion.identity);
            
            PlayableBaseUnitController unit = obj.GetComponent<PlayableBaseUnitController>();
            UnitModel model = obj.GetComponent<UnitModel>();
            model.AttackPoint = BattleSceneManager.Instance.myUnitData[i].Atk; 
            model.DefensePoint = BattleSceneManager.Instance.myUnitData[i].Def;
            model.MaxHp = BattleSceneManager.Instance.myUnitData[i].MaxHp;
            model.Hp = BattleSceneManager.Instance.myUnitData[i].MaxHp; 
            BattleSceneManager.Instance.myUnits.Add(unit);


        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnitData.Count; i++)
        {
            int id = BattleSceneManager.Instance.enemyUnitData[i].Id;
            int pos = BattleSceneManager.Instance.enemyUnitData[i].Pos;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Enemy_" + id.ToString()), enemyGrid[pos].position, Quaternion.identity);
            BaseUnitController unit = obj.GetComponent<BaseUnitController>();
            UnitModel model = obj.GetComponent<UnitModel>();
            model.AttackPoint = BattleSceneManager.Instance.enemyUnitData[i].Atk;
            model.DefensePoint = BattleSceneManager.Instance.enemyUnitData[i].Def;
            model.MaxHp = BattleSceneManager.Instance.enemyUnitData[i].MaxHp;
            model.Hp = BattleSceneManager.Instance.enemyUnitData[i].MaxHp;
            BattleSceneManager.Instance.enemyUnits.Add(unit);
        }
    
        BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Battle;
        OnSpawnCompleted?.Invoke();
        StartCoroutine(TimeDelaystart());
    }

    IEnumerator TimeDelaystart() 
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
    }
}
