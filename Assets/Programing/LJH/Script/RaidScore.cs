using Firebase.Database;
using Firebase.Extensions;
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
        
        //Debug.Log(curBossHpScore);
    }

    public void setRankingData()
    {

        DatabaseReference root = BackendManager.Database.RootReference.Child("RaidData").Child(BackendManager.Auth.CurrentUser.UserId);

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase 데이터 가져오기 실패: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            int previousScore = 0;

            // Child("RaidData")가 없거나 데이터가 없는 경우 처리
            if (snapshot == null || !snapshot.Exists)
            {
                Debug.LogWarning("RaidData 데이터가 존재하지 않습니다. 새로운 데이터로 초기화합니다.");
            }
            else if (snapshot.Child("_totalDamage").Value != null)
            {
                previousScore = int.Parse(snapshot.Child("_totalDamage").Value.ToString());
            }

            Debug.Log($"이전 점수: {previousScore}, 현재 점수: {curBossHpScore}");

            // 새로운 점수가 더 높을 경우에만 갱신
            if (curBossHpScore > previousScore)
            {
                RaidData RaidData = new RaidData
                {
                    Name = PlayerDataManager.Instance.PlayerData.PlayerName,
                    TotalDamage = curBossHpScore
                };

                string json = JsonUtility.ToJson(RaidData);

                root.SetRawJsonValueAsync(json).ContinueWithOnMainThread(setTask =>
                {
                    if (setTask.IsCompleted)
                    {
                        Debug.Log("데이터 갱신 완료");
                    }
                    else
                    {
                        Debug.LogError("데이터 갱신 실패: " + setTask.Exception);
                    }
                });
            }
            else
            {
                Debug.Log("새로운 점수가 이전 점수보다 낮아서 갱신하지 않음.");
            }
        });
    }
    public void setTotalDamageData()
    {

    }
}
