using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Transform[] myGrid;
    [SerializeField] Transform[] enemyGrid;
    public void SpawnUnits()
    {
        for (int i = 1; i < BattleSceneManager.Instance.inGridObject.Length; i++)
        {
            if (BattleSceneManager.Instance.inGridObject[i] != null)
            {
                int id = BattleSceneManager.Instance.inGridObject[i].GetComponent<UnitStat>().Id;
                float atk = BattleSceneManager.Instance.inGridObject[i].GetComponent<UnitStat>().Atk;
                GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Characters_" + id.ToString()), myGrid[i].position, Quaternion.identity);
                UnitModel unit = obj.GetComponent<UnitModel>();
                //unit.AttackPoint = BattleSceneManager.Instance.inGridObject[i].GetComponent<UnitStat>().Atk;
                //BattleSceneManager.Instance.myUnits.Add()
            }

        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyGridObject.Length; i++)
        {
            if (BattleSceneManager.Instance.enemyGridObject[i] != null)
            {   
                GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Enemy_" + BattleSceneManager.Instance.enemyGridObject[i]), enemyGrid[i].position, Quaternion.identity);
                
            }
            

        }
    }
}
