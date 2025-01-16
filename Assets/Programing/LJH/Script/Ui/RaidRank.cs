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
    [SerializeField] int BossWorldHP; // 나중엔 시트로 불러와서 해야할듯
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
        //    Debug.Log("snapshot null값임! 또는 List값 0개임");
        //}
    }
    public void buttonSound()
    {
        SoundManager.Instance.PlaySFX(buttonSfx);
    }

    public void getRankingData() 
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        DatabaseReference root = BackendManager.Database.RootReference.Child("RaidData");
        root.KeepSynced(true);
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ID값들 가져오고 SnapShot의 List 잘가져왔는지 확인.
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
            sortedRaidDatas[i].Rank = i; // 순위는 0부터 시작
        
        }

        // raidDatas를 업데이트
        raidDatas = sortedRaidDatas;
        // 만약 rankingUi가 raidDatas보다 더 크다면 남은 UI 슬롯을 비우기
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
            } // 5등까지만 노출
  
        }

        float progress = Mathf.Clamp01((float)totalDamage / BossWorldHP);
        _loadingBar.value = progress;

        _loadingText.text = "총 대미지 : " + totalDamage.ToString();
        


    }
}
