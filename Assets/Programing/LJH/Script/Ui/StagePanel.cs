//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class StagePanel : MonoBehaviour
//{
//    [SerializeField] TMP_Text stageNumText;  // 스테이지 번호 
//    [SerializeField] TMP_Text stageNameText; // 스테이지 이름 
//    [SerializeField] TMP_Text timeLimitText; // 시간제한

//    [SerializeField] List<Dictionary<string, string>> stageDic;
//    [SerializeField] List<Dictionary<string, string>> monsterGroupDic;
//    [SerializeField] List<Dictionary<string, string>> monsterDic;
//    [SerializeField] List<Dictionary<string, string>> stageRewardDic;
//    [SerializeField] List<Dictionary<string, string>> itemDic;

//    [SerializeField] List<string> curMobPos; // 현재 스테이지의 몬스터 위치
//    [SerializeField] List<int> curmyPos;  // 현재 스테이지의 플레이어의 캐릭터 위치
     
//    [SerializeField] string curStageID;   // 현재 스테이지의 정보들
//    [SerializeField] string curStageNames;
//    [SerializeField] string curTimeLimit;

//    [SerializeField] string curMobGroup;
//    [SerializeField] string curRewardGroup;


    
//    [SerializeField] Image[] mygrid;
//    [SerializeField] Image[] enemygrid;

//    [SerializeField] List<string> itemIds;
//    [SerializeField] List<string> itemCounts;



//    // Monster, Stages, MontserGroup, StageReward, Item  08 ~ 12 나중에 합칠때 리스트 순서 수정해야함 
//    public void setStageData(int stageNum) //  stages csv의 스테이지 순서대로(0번부터)
//    {
//        stageDic = CsvDataManager.Instance.DataLists[1]; //파싱한 순서(url 순서대로 들어감)
//        curStageID = stageDic[stageNum]["StageID"];
//        curStageNames = stageDic[stageNum]["StageName"];
//        curTimeLimit = stageDic[stageNum]["Limit"];
//        BattleSceneManager.Instance._timeLimit = int.Parse(curTimeLimit);

//        curMobGroup = stageDic[stageNum]["MonsterGroupID"];
//        curRewardGroup = stageDic[stageNum]["StageRewardID"];
//        monsterGroupDic = CsvDataManager.Instance.DataLists[2];

//        gridClearing();

//        for (int i = 0; i < monsterGroupDic.Count; i++)  // 현재 스테이지에 맞는 몬스터 그룹 찾아서 맞는 위치에 몬스터 넣기(몬스터 id)
//        {
//            if (monsterGroupDic[i]["MonsterGroupID"] == curMobGroup) 
//            {
//                curMobPos[int.Parse(monsterGroupDic[i]["MonsterLocation"])] = monsterGroupDic[i]["MonsterID"];
//            }
//        }

//        monsterDic = CsvDataManager.Instance.DataLists[0];
//        for (int i = 0; i < curMobPos.Count; i++)
//        {
//            if (curMobPos[i] != null) 
//            {
//                enemygrid[i].color = Color.red;
//                BattleSceneManager.Instance.enemyGridObject[i] = curMobPos[i]; // 몬스터 id로 instantiate가 안될거 같으면 몬스터 이름으로?
//            }
//        }

//        stageRewardDic = CsvDataManager.Instance.DataLists[3];
//        itemDic = CsvDataManager.Instance.DataLists[4];
//        for (int i = 0; i < stageRewardDic.Count; i++)   // 스테이지 클리어시 받을 아이템 정보  
//        {
//            if (stageRewardDic[i]["StageRewardID"] == curRewardGroup) 
//            {
//                itemIds.Add(stageRewardDic[i]["ItemID"]);
//                itemCounts.Add(stageRewardDic[i]["Count"]);
//                BattleSceneManager.Instance.curItemIDs.Add(stageRewardDic[i]["ItemID"]);
//                BattleSceneManager.Instance.curItemCounts.Add(stageRewardDic[i]["Count"]);
                
//            }
//        }
        
//        //BattleSceneManager.Instance.curRewardValue = Rewards;


//        stageNumText.text = curStageID;
//        stageNameText.text = curStageNames;
//        timeLimitText.text =  "Time Limit : "+curTimeLimit+" sec";
        
//    }
  

//    private void gridClearing() 
//    {
//        curmyPos.Clear(); 
//        curMobPos.Clear();
//        for (int i = 0; i < enemygrid.Length; i++) // 색 설정 및 초기화
//        {
//            enemygrid[i].color = Color.black;
//            mygrid[i].color = Color.black;
//            curMobPos.Add(null);
//            curmyPos.Add(0);
//        }
//    }
   
//}
