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
            int level = BattleSceneManager.Instance.myUnitData[i].Level;
            int increase = BattleSceneManager.Instance.myUnitData[i].Increase;

            int buffAtk = 0;
            int buffDef = 0;
            int buffHp = 0;
            int buffCool = 0;
            if (BattleSceneManager.Instance.myUnitData[i].buffs.Count != 0) 
            {
                for (int j = 0; j < BattleSceneManager.Instance.myUnitData[i].buffs.Count; j++)
                {
                    switch (BattleSceneManager.Instance.myUnitData[i].buffs[j].y) 
                    {
                        case 1:
                            buffAtk += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 2:
                            buffDef += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 3:
                            buffHp += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 4:
                            buffCool += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                    }
                    Debug.Log($"id{id}¿¡ {BattleSceneManager.Instance.myUnitData[i].buffs[j].y}½ºÅÈ {BattleSceneManager.Instance.myUnitData[i].buffs[j].z} ¹öÇÁ");
                }
               
            }  // 6* (2+2)
            double incAtk =1+((level * (increase+buffAtk))/100.0);    // »ó½Â·® 
            double incDef =1+((level * (increase+buffDef))/100.0);    // »ó½Â·® 
            double incHp = 1+((level * (increase+buffHp))/100.0);    // »ó½Â·® 
            double incCool = 1+(buffCool / 100.0);
            Debug.Log($"°ø {incAtk} ¹æ {incDef} Ã¼ {incHp} Äð {incCool}");
            GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Character_" + id.ToString()), myGrid[pos-1].position, Quaternion.identity);
            
            PlayableBaseUnitController unit = obj.GetComponent<PlayableBaseUnitController>();
            UnitModel model = obj.GetComponent<UnitModel>();
            model.AttackPoint = (int)(BattleSceneManager.Instance.myUnitData[i].Atk*incAtk); 
            model.DefensePoint = (int)(BattleSceneManager.Instance.myUnitData[i].Def*incDef);
            model.MaxHp = (int)(BattleSceneManager.Instance.myUnitData[i].MaxHp*incHp);
            model.Hp = model.MaxHp;
            model.CoolDownAcc = (float)(BattleSceneManager.Instance.myUnitData[i].Cool * incCool);
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
