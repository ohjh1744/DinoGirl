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
        curBossHpScore = maxHp - score;
        ResultScoreTxt.text = curBossHpScore.ToString();
        curScoreTxt.text = curBossHpScore.ToString();

        //Debug.Log(curBossHpScore);
    }

    public void setRankingData()
    {

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("RaidData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase ������ �������� ����: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            int previousScore = 0;

            // Child("RaidData")�� ���ų� �����Ͱ� ���� ��� ó��
            if (snapshot == null || !snapshot.Exists)
            {
                Debug.LogWarning("RaidData �����Ͱ� �������� �ʽ��ϴ�. ���ο� �����ͷ� �ʱ�ȭ�մϴ�.");
            }
            else if (snapshot.Child("_totalDamage").Value != null)
            {
                previousScore = int.Parse(snapshot.Child("_totalDamage").Value.ToString());
            }

            Debug.Log($"���� ����: {previousScore}, ���� ����: {curBossHpScore}");

            // ���ο� ������ �� ���� ��쿡�� ����
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
                        Debug.Log("������ ���� �Ϸ�");
                    }
                    else
                    {
                        Debug.LogError("������ ���� ����: " + setTask.Exception);
                    }
                });
            }
            else
            {
                Debug.Log("���ο� ������ ���� �������� ���Ƽ� �������� ����.");
            }
        });
    }
    public void setTotalDamageData()
    {

    }
}
