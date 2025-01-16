using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{

    [SerializeField] Transform[] myGrid;
    [SerializeField] Transform[] enemyGrid;

    [SerializeField] GameObject image;
    [SerializeField] GameObject ready;
    [SerializeField] GameObject start;

    [SerializeField] Button[] buttons;

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
                            buffHp += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 2:
                            buffAtk += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 3:
                            buffDef += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                        case 4:
                            buffCool += BattleSceneManager.Instance.myUnitData[i].buffs[j].z;
                            break;
                    }
                    Debug.Log($"id{id}에 {BattleSceneManager.Instance.myUnitData[i].buffs[j].y}스탯 {BattleSceneManager.Instance.myUnitData[i].buffs[j].z} 버프");
                }
               
            }  // 6* (2+2)
            double incAtk =1+(((level - 1) * (increase + buffAtk)) / 100.0);    // 상승량 
            double incDef =1+(((level - 1) * (increase + buffDef)) / 100.0);    // 상승량 
            double incHp = 1+(((level - 1) * (increase + buffHp)) / 100.0);    // 상승량 
            double incCool = 1+(buffCool / 100.0);
            Debug.Log($"공 {incAtk} 방 {incDef} 체 {incHp} 쿨 {incCool}");
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
        int stageid = BattleSceneManager.Instance.curStageNum+101;
        double Aper =int.Parse(CsvDataManager.Instance.DataLists[6][stageid]["ATKPercent"])/ 100.0 ;
        double Hper =int.Parse(CsvDataManager.Instance.DataLists[6][stageid]["HpPercent"])/ 100.0 ;
        double Dper =int.Parse(CsvDataManager.Instance.DataLists[6][stageid]["DefPercent"]) / 100.0 ;
        Debug.Log($"스테이지 배율 공{Aper} 체 {Hper} 방 {Dper}");
        for (int i = 0; i < BattleSceneManager.Instance.enemyUnitData.Count; i++)
        {
            int id = BattleSceneManager.Instance.enemyUnitData[i].Id;
            int pos = BattleSceneManager.Instance.enemyUnitData[i].Pos;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Enemies/Enemy_" + id.ToString()), enemyGrid[pos].position, Quaternion.identity);
            BaseUnitController unit = obj.GetComponent<BaseUnitController>();
            UnitModel model = obj.GetComponent<UnitModel>();
            model.AttackPoint = (int)(BattleSceneManager.Instance.enemyUnitData[i].Atk*Aper);
            model.DefensePoint = (int)(BattleSceneManager.Instance.enemyUnitData[i].Def*Dper);
            model.MaxHp = (int)(BattleSceneManager.Instance.enemyUnitData[i].MaxHp*Hper);
            model.Hp = model.MaxHp;
            BattleSceneManager.Instance.enemyUnits.Add(unit);
        }
    
        BattleSceneManager.Instance.curBattleState = BattleSceneManager.BattleState.Battle;
        OnSpawnCompleted?.Invoke();
        StartCoroutine(TimeDelaystart());
        image.SetActive(false);
        }

    IEnumerator TimeDelaystart() 
    {
        for (int i = 0; i < buttons.Length; i++) 
        {
            buttons[i].interactable = false;
        }
        ready.SetActive(true);
        yield return new WaitForSecondsRealtime(2.5f);
        ready.SetActive(false); 
        start.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        start.SetActive(false);
        Time.timeScale = 1f;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }

    }
}
