using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RaidScore : MonoBehaviour
{
    [SerializeField] int curBossHp;




    private void OnEnable()
    {
        StartCoroutine(DelayingSubsScore());
    }
    IEnumerator DelayingSubsScore() 
    {
        yield return new WaitForSeconds(3f);
        BattleSceneManager.Instance.enemyUnits[0].UnitModel.OnHpChanged += UpdataScore;
    }

    public void UpdataScore(int score) 
    {
        curBossHp = score;
        Debug.Log(score);
    }
}
