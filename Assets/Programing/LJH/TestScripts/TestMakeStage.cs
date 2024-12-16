
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
    [SerializeField] int[] curMobPos;

    [SerializeField] List<int> curmyPos;

    [SerializeField] GameObject monster;
    [SerializeField] GameObject player;
    [SerializeField] Transform[] myPoss;
    [SerializeField] Transform[] enemyPoss;

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
    }

    public void setStageData(int stageNum)
    {

        string[] values = stageDatas[stageNum].Split(',');

        curStageNum = values[0];
        curStageNames = values[1];
        curTimeLimit = int.Parse(values[2]);
        if (values[3] == "0")
        {
            curStageCleared = false;
        }
        else 
        {
            curStageCleared = true; 
        }
        curMobCount = int.Parse(values[4]);
        string[] val = values[5].Split("");

        string mobPosString = values[5]; // 예: "135"
        curMobPos = new int[mobPosString.Length]; // 배열 크기 설정
        for (int i = 0; i < enemygrid.Length; i++) 
        {
            enemygrid[i].image.color = Color.black;
            mygrid[i].image.color = Color.black;
            curmyPos.Clear();

        }

        for (int i = 0; i < mobPosString.Length; i++)
        {
            curMobPos[i] = int.Parse(mobPosString[i].ToString()); // 한 문자씩 숫자로 변환
            enemygrid[curMobPos[i]-1].image.color= Color.red;
        }
        

        stageNumText.text = curStageNum;
        stageNameText.text = curStageNames;
    }

    public void setMyPos(int num) 
    {
        mygrid[num].image.color = Color.green;
        curmyPos.Add(num);
    }

    public void StartStage()
    {
        for (int i = 0; i < curMobCount; i++)
        {
           GameObject gameObject1 =  Instantiate(monster, enemyPoss[curMobPos[i]-1].position, Quaternion.identity);
           Destroy(gameObject1,3f);
           
        }
        for (int i = 0; i < curmyPos.Count; i++)
        {
            GameObject gameObject2 = Instantiate(player, myPoss[curmyPos[i]].position, Quaternion.identity);
            Destroy(gameObject2, 3f);
        }
    }





}
