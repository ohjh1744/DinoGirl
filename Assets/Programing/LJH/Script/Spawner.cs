using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] Transform[] myGrid;
    [SerializeField] Transform[] enemyGrid;
    public void SpawnUnits()
    {
        
        for (int i = 0; i < BattleSceneManager.Instance.myUnitData.Count; i++)
        {

            int id = BattleSceneManager.Instance.myUnitData[i].Id;
            int pos = BattleSceneManager.Instance.myUnitData[i].Pos;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Character_" + id.ToString()), myGrid[pos-1].position, Quaternion.identity);
            
            PlayableBaseUnitController unit = obj.GetComponent<PlayableBaseUnitController>();
            UnitModel model = obj.GetComponent<UnitModel>();
            //model.AttackPoint = BattleSceneManager.Instance.myUnitData[i].Atk; //이 부분에서 스탯계산 들어가면 될듯
            BattleSceneManager.Instance.myUnits.Add(unit);


        }
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnitData.Count; i++)
        {
            int id = BattleSceneManager.Instance.enemyUnitData[i].Id;
            int pos = BattleSceneManager.Instance.enemyUnitData[i].Pos;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Characters/Enemy_" + id.ToString()), enemyGrid[pos-1].position, Quaternion.identity);
            BaseUnitController unit = obj.GetComponent<BaseUnitController>();
            BattleSceneManager.Instance.enemyUnits.Add(unit);
        }
    }
}
