using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaidRank : MonoBehaviour
{
    [SerializeField] List<RaidData> raidDatas;
    [SerializeField] GameObject[] rankingUi;
    [SerializeField] int BossWorldHP; // ���߿� ��Ʈ�� �ҷ��ͼ� �ؾ��ҵ�
    [SerializeField] int totalDamage;
    [SerializeField] private Slider  _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingText;


    [SerializeField] AudioClip buttonSfx;
    [SerializeField] AudioClip _bgmClip;



    private void OnEnable()
    {
        SoundManager.Instance.PlayeBGM(_bgmClip);
        raidDatas = new List<RaidData>();
        getRankingData();
    }
    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    {
        //while (snapshotChildren == null || snapshotChildren.Count == 0)
        //{
        //    Debug.Log("snapshot null����! �Ǵ� List�� 0����");
        //}
    }
    public void buttonSound()
    {
        SoundManager.Instance.PlaySFX(buttonSfx);
    }

    public void getRankingData() 
    {
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("RaidData");
        root.KeepSynced(true);
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ID���� �������� SnapShot�� List �߰����Դ��� Ȯ��.
            var RaidChildren = snapShot.Children.ToList();
            CheckSnapSHot(RaidChildren);


            System.Random random = new System.Random();
            RaidChildren = RaidChildren.OrderBy(item => random.Next()).ToList();

            for (int i = 0; i < RaidChildren.Count; i++)
            {
 
                RaidData data = new RaidData();
                data.Name = RaidChildren[i].Child("_name").Value.ToString();
                data.TotalDamage = int.Parse(RaidChildren[i].Child("_totalDamage").Value.ToString());

                Debug.Log($"{data.Name} :  {data.TotalDamage}");
                raidDatas.Add(data);
            }
            sortRanking();
        });
    }
    private void sortRanking() 
    {
        var sortedRaidDatas = raidDatas
        .OrderByDescending(data => data.TotalDamage)
        .ToList();

        for (int i = 0; i < sortedRaidDatas.Count; i++)
        {
            sortedRaidDatas[i].Rank = i; // ������ 0���� ����
        
        }

        // raidDatas�� ������Ʈ
        raidDatas = sortedRaidDatas;
        // ���� rankingUi�� raidDatas���� �� ũ�ٸ� ���� UI ������ ����
        for (int i = 0; i < rankingUi.Length; i++)
        {   
            
            rankingUi[i].GetComponent<RankingSlot>().setRankingData("", "");
        }
        for (int i = 0; i < raidDatas.Count; i++)
        {
            totalDamage += raidDatas[i].TotalDamage;
            if (i < 5) 
            {
                rankingUi[i].GetComponent<RankingSlot>().setRankingData(raidDatas[i].Name, raidDatas[i].TotalDamage.ToString());
            } // 5������� ����
  
        }

        float progress = Mathf.Clamp01((float)totalDamage / BossWorldHP);
        _loadingBar.value = progress;

        _loadingText.text = "�� ����� : " + totalDamage.ToString();
        


    }
}
