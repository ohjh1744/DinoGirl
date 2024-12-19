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

    [SerializeField] List<Dictionary<string, string>> stageDic;

    [SerializeField] List<int> curMobPos; // 현재 스테이지의 몬스터 위치
    [SerializeField] List<int> curmyPos;  // 현재 스테이지의 플레이어의 캐릭터 위치
     
    [SerializeField] string curStageNum;   // 현재 스테이지의 정보들
    [SerializeField] string curStageNames;
    [SerializeField] string curTimeLimit;
    [SerializeField] int curMobCount;
    [SerializeField] Image[] mygrid;
    [SerializeField] Image[] enemygrid;

    public void setStageData(int stageNum) //  stages csv의 스테이지 순서대로(0번부터)
    {
        stageDic = DataManager.Instance.DataLists[(int)E_CsvData.Stage]; //파싱한 순서(url 순서대로 들어감)
        curStageNum = stageDic[stageNum]["Id"];
        curStageNames = stageDic[stageNum]["StageName"];
        curTimeLimit = stageDic[stageNum]["TimeLimit"];
        curMobCount = int.Parse(stageDic[stageNum]["MonsterCount"]);

        for (int i = 0; i < enemygrid.Length; i++) // 색 설정 및 초기화
        {
            enemygrid[i].color = Color.black;
            mygrid[i].color = Color.black;
            curmyPos.Clear();
            curMobPos.Clear();
        }

        foreach (char val in stageDic[stageNum]["MonsterPos"])// 한 문자씩 숫자로 변환
        {
            curMobPos.Add(int.Parse(val.ToString()));
        }

        for (int i = 0; i < curMobPos.Count; i++)
        {

            enemygrid[curMobPos[i] - 1].color = Color.red;
        }


        stageNumText.text = curStageNum;
        stageNameText.text = curStageNames;
        timeLimitText.text =  "Time Limit : "+curTimeLimit+" sec";
    }

    public void setMyGrid() 
    {
        // Todo : 드랍하면 해당 위치에 올라가면서 정보 세팅 asdasd
    }


    public void StageStart()
    {
        Debug.Log("스테이지 시작");
        // Todo :  캐릭터의 배치 위치 정보 + 배치순서? + 버프가 적용된 기준의 스탯을 가지고 배틀씬으로 넘어가야 함 , + 몬스터의 배치 정보 
    }
}
