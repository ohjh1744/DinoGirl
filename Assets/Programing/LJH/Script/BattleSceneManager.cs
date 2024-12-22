using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    private static BattleSceneManager _instance;
    public static BattleSceneManager Instance { get { return _instance; } set { _instance = value; } }


    
    private DraggableUI[] Draggables;

    [SerializeField] public GameObject[] inGridObject; // 아군 정보 배열 나중에 타입을 바꾸면 될듯
    [SerializeField] public string[] enemyGridObject;// 적 정보 배열
    [SerializeField] public int _timeLimit; // private 프로퍼티로 바꿀 예정 , 
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inGridObject = new GameObject[10]; // 캐릭터 목록이 0번 
        enemyGridObject = new string[9];
       
    }

    public void StageStart()
    {
        string datas = "";
        int count = 0; 
        Debug.Log("스테이지 시작");
        // 씬만 넘어가게 해도 될듯 이미 매니저에 데이터가 있으니 
        for (int i = 0; i < inGridObject.Length; i++) 
        {
            if (inGridObject[i] != null) 
            {   
                datas += i.ToString();
                count++;
            }
        }
        Debug.Log(datas);
        if (count > 5) 
        {
            Debug.Log("5인 초과 출발 불가");
        }
    }
    public void BackStage() 
    {
        // 스테이지패널 , 배틀씬 매니저정보 초기화 해야함
        for (int i = 0; i < inGridObject.Length; i++) 
        {
            inGridObject[i] = null; 
            enemyGridObject[i] = null;
        }
        for (int i = 0; i < Draggables.Length; i++) 
        {
            Destroy(Draggables[i].gameObject);
        }
    } 

    public void getDraggables() 
    {
        StartCoroutine(delay());
    }

    IEnumerator delay() 
    {
        yield return 0.1f;

        Draggables = FindObjectsOfType<DraggableUI>();
    }


    private void BattleSceneStart() 
    {
        // 적 위치, 종류(id?)

        // 아군 위치, 종류 최종 스탯 
        // 스테이지 시간 

    }
}