
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestMakeStage : MonoBehaviour
{
    private string stage1 = "0,tutorial1,150,0,3,135";
    private string stage2 = "1,tutorial2,150,0,3,279";
    private string stage3 = "2,tutorial3,150,0,3,369";
    private string stage4 = "3,Well begun is half done,150,0,5,12345";
    private string stage5 = "4,Many Moons Ago,150,0,6,123789";
    private string stage6 = "5,Opportunities don’t happen. You create them,150,0,3,578";
    private string stage7 = "6,Knowledge is power,150,0,5,34569";
    private string stage8 = "7,No pain no gain,150,0,4,1238";
    private string stage9 = "8,Silence is golden,150,0,3,189";
    private string stage10 = "9,Rome wasn’t built in a day,150,0,7,1234567";
    private string stage11 = "10,A journey of a thousand miles begins with a single step,150,0,9,123456789";

    [SerializeField] TMP_Text stageNumText;
    [SerializeField] TMP_Text stageNameText;
    [SerializeField] Button[] mygrid;
    [SerializeField] Button[] enemygrid;


    [SerializeField] string[] stageDatas;

    [SerializeField] bool curStageCleared;
    [SerializeField] string curStageNum;
    [SerializeField] string curStageNames;
    [SerializeField] int curTimeLimit;
    [SerializeField] int curMobCount;
    

    [SerializeField] List<int> curMobPos;
    [SerializeField] List<int> curmyPos;

    [SerializeField] GameObject monster;
    [SerializeField] GameObject player;
    [SerializeField] Transform[] myPoss;
    [SerializeField] Transform[] enemyPoss;
    [SerializeField] Button[] buttons;
    [SerializeField] List<Dictionary<string, string>> stageDic;
    private void Start()
    {
        stageDatas[0] = stage1;
        stageDatas[1] = stage2;
        stageDatas[2] = stage3;
        stageDatas[3] = stage4;
        stageDatas[4] = stage5;
        stageDatas[5] = stage6; 
        stageDatas[6] = stage7;
        stageDatas[7] = stage8;
        stageDatas[8] = stage9;
        stageDatas[9] = stage10;
        stageDatas[10] = stage11;
        for (int i = 0; i < buttons.Length; i++) 
        {
           // buttons[i].interactable = false;
            //유저의 스테이지 클리어 데이터를 가지고 있어야 함 ! 
            // 추가로 db에 해당 데이터가 있어야 함
            // db에 
        }
       
    }

    public void setStageData(int stageNum)
    {
        stageDic = DataManager.Instance.DataLists[(int)E_CsvData.Stage]; //파싱한 순서(url 순서대로 들어감)
        curStageNum = stageDic[stageNum]["Id"];
        curStageNames = stageDic[stageNum]["StageName"];
        curTimeLimit = int.Parse(stageDic[stageNum]["TimeLimit"]);       
        curMobCount = int.Parse(stageDic[stageNum]["MonsterCount"]);

        for (int i = 0; i < enemygrid.Length; i++) // 색 설정 및 초기화
        {
            enemygrid[i].image.color = Color.black;
            mygrid[i].image.color = Color.black;
            curmyPos.Clear();
            curMobPos.Clear();
        }

        foreach (char val in stageDic[stageNum]["MonsterPos"])// 한 문자씩 숫자로 변환
        {
            curMobPos.Add(int.Parse(val.ToString()));
        }

        for (int i = 0; i < curMobPos.Count; i++)
        {
            
            enemygrid[curMobPos[i]-1].image.color= Color.red;
        }
        

        stageNumText.text = curStageNum;
        stageNameText.text = curStageNames;
    }

    public void setMyPos(int num)  // 버튼 눌러서 자리 세팅 , 추후엔 드래그 앤 드랍으로 바꿔야 함 
    {
        mygrid[num].image.color = Color.green;
        curmyPos.Add(num);
    }

    public void StartStage()
    {
        for (int i = 0; i < curMobCount; i++)
        {
           GameObject gameObject1 =  Instantiate(monster, enemyPoss[curMobPos[i]-1].position, Quaternion.identity);
           //Destroy(gameObject1,3f);
           
        }
        for (int i = 0; i < curmyPos.Count; i++)
        {
            GameObject gameObject2 = Instantiate(player, myPoss[curmyPos[i]].position, Quaternion.identity);
           // Destroy(gameObject2, 3f);
        }
    }
   
    public void stop() 
    {
        Time.timeScale = 0;
    }
    public void Unstop()
    {
        stageDic = DataManager.Instance.DataLists[(int)E_CsvData.Stage];
        Debug.Log(stageDic[0]["MonsterPos"]);
    }






}
