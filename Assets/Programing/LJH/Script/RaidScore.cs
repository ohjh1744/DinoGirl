using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class RaidScore : MonoBehaviour
{
    [SerializeField] public int curBossHpScore;
    [SerializeField] TMP_Text ResultScoreTxt;
    [SerializeField] TMP_Text curScoreTxt;

    private int maxHp;

    private void OnEnable()
    {
        StartCoroutine(DelayingSubsScore());
    }
    IEnumerator DelayingSubsScore() 
    {
        yield return new WaitForSeconds(3f);
        BattleSceneManager.Instance.enemyUnits[0].UnitModel.OnHpChanged += UpdataScore;
        maxHp = BattleSceneManager.Instance.enemyUnits[0].UnitModel.MaxHp;
    }

    public void UpdataScore(int score) 
    {
        curBossHpScore =maxHp-score;
        ResultScoreTxt.text = curBossHpScore.ToString();
        curScoreTxt.text = curBossHpScore.ToString();
        
        Debug.Log(curBossHpScore);
    }

    public void setRankingData()
    {
        
        DatabaseReference root = BackendManager.Database.RootReference.Child("RaidData").Child(BackendManager.Auth.CurrentUser.UserId);

        RaidData RaidData = new RaidData();

        RaidData.Name = PlayerDataManager.Instance.PlayerData.PlayerName;

        RaidData.TotalDamage = curBossHpScore;
        Debug.Log($"{name} : {curBossHpScore}");
        string json = JsonUtility.ToJson(RaidData);

        root.SetRawJsonValueAsync(json);
    }
}
