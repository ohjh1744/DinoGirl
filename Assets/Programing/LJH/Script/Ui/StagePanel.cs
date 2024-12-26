using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    [SerializeField] TMP_Text stageNumText;  // 스테이지 번호 
    [SerializeField] TMP_Text stageNameText; // 스테이지 이름 
    [SerializeField] TMP_Text timeLimitText; // 시간제한

    [SerializeField] Dictionary<int,Dictionary<string, string>> stageDic;
    [SerializeField] Dictionary<int, Dictionary<string, string>> monsterGroupDic;
    [SerializeField] Dictionary<int, Dictionary<string, string>> monsterDic;
    [SerializeField] Dictionary<int, Dictionary<string, string>> stageRewardDic;
    [SerializeField] Dictionary<int, Dictionary<string, string>> itemDic;
     
    [SerializeField] List<string> curMobPos; // 현재 스테이지의 몬스터 위치
    [SerializeField] List<int> curmyPos;  // 현재 스테이지의 플레이어의 캐릭터 위치
   
    [SerializeField] int curStageID;   // 현재 스테이지의 정보들
    [SerializeField] string curStageNames;
    [SerializeField] string curTimeLimit;

    [SerializeField] int curMobGroup;
    [SerializeField] int curRewardGroup;


  
    [SerializeField] Image[] mygrid;
    [SerializeField] Image[] enemygrid;

    [SerializeField] Dictionary<int, int> itemValues = new Dictionary<int, int>();


    // Monster, Stages, MontserGroup, StageReward, Item  08 ~ 12 나중에 합칠때 리스트 순서 수정해야함 
    public void setStageData(int stageNum) //  stages csv의 스테이지 순서대로(0번부터)
    {
        stageDic = CsvDataManager.Instance.DataLists[1]; //파싱한 순서(url 순서대로 들어감)

       // curStageID = TypeCastManager.Instance.TryParseInt(stageDic[101]["StageID"]);// stageDic[stageNum]["StageID"];
        curStageNames = stageDic[stageNum]["StageName"];// stageDic[stageNum]["StageName"];
        curTimeLimit = stageDic[stageNum]["Limit"];
        BattleSceneManager.Instance._timeLimit = int.Parse(curTimeLimit);

        curMobGroup = TypeCastManager.Instance.TryParseInt(stageDic[stageNum]["MonsterGroupID"]);
        curRewardGroup = TypeCastManager.Instance.TryParseInt(stageDic[stageNum]["StageRewardID"]); 

        monsterGroupDic = CsvDataManager.Instance.DataLists[2];

        gridClearing();

        for (int i = 0; i < 9; i++)  // 현재 스테이지에 맞는 몬스터 그룹 찾아서 맞는 위치에 몬스터 넣기(몬스터 id)
        {
            curMobPos[i] = monsterGroupDic[curMobGroup]["MonsterLocation" + (i+1).ToString()];
        }

        monsterDic = CsvDataManager.Instance.DataLists[0];
        for (int i = 0; i < curMobPos.Count; i++)
        {
            if (curMobPos[i] != "0") 
            {
                enemygrid[i].color = Color.red;
                BattleSceneManager.Instance.enemyGridObject[i] = curMobPos[i]; // 몬스터 id로 instantiate가 안될거 같으면 몬스터 이름으로?
            }
        }

        stageRewardDic = CsvDataManager.Instance.DataLists[3]; // 스테이지 클리어시 받을 보상 불러오기
        itemDic = CsvDataManager.Instance.DataLists[4];
        foreach (string item in stageRewardDic[curRewardGroup].Keys) 
        {
            itemValues.Add(int.Parse(item),int.Parse(stageRewardDic[curRewardGroup][item]));
            BattleSceneManager.Instance.curItemValues.Add(int.Parse(item), int.Parse(stageRewardDic[curRewardGroup][item]));
        }

        stageNumText.text = curStageID.ToString();
        stageNameText.text = curStageNames;
        timeLimitText.text =  "Time Limit : "+curTimeLimit+" sec";
      
    }


    private void gridClearing() 
    {
        curmyPos.Clear(); 
        curMobPos.Clear();
        for (int i = 0; i < enemygrid.Length; i++) // 색 설정 및 초기화
        {
            enemygrid[i].color = Color.black;
            mygrid[i].color = Color.black;
            curMobPos.Add(null);
            curmyPos.Add(0);
        }
    }
 
}
