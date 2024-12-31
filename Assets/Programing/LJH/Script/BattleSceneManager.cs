using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BattleSceneManager : MonoBehaviour
{
    private static BattleSceneManager _instance;
    public static BattleSceneManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private DraggableUI[] Draggables;
     

    //BaseUnitController 로 전달이 가능해야 함 
    [SerializeField] public GameObject[] inGridObject; // 아군 정보 배열 나중에 타입을 바꾸면 될듯
    [SerializeField] public string[] enemyGridObject;// 적 정보 배열 배열의 인덱스가 위치임 , id 저장 

    [SerializeField] public List<PlayableBaseUnitController> myUnits;
    [SerializeField] public List<BaseUnitController> enemyUnits;
    [SerializeField] public List<UnitsDatas> myUnitData;
    [SerializeField] public List<UnitsDatas> enemyUnitData;

    [SerializeField] public int _timeLimit; // private 프로퍼티로 바꿀 예정

    [SerializeField] public Dictionary<int, int> curItemValues = new Dictionary<int, int>();// 클리어 보상

   
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        inGridObject = new GameObject[10]; // 캐릭터 목록이 0번 
        enemyGridObject = new string[9];
        myUnits = new List<PlayableBaseUnitController>();
        enemyUnits = new List<BaseUnitController>();
        myUnitData = new List<UnitsDatas>();
        enemyUnitData = new List<UnitsDatas>(); 
    }

    public UnityEvent startStage;
    public void StageStart()
    {
        // 1 ~ 5인만 출발 가능
        for (int i = 1; i < inGridObject.Length; i++)
        {
            if (inGridObject[i] != null)
            {   
                 inGridObject[i].GetComponent<UnitStat>().Pos = i;
                 UnitsDatas unit = new UnitsDatas();
                 unit.Pos = i;
                 unit.Id = inGridObject[i].GetComponent<UnitStat>().Id;
                 unit.Level = inGridObject[i].GetComponent<UnitStat>().Level;
                 unit.MaxHp = inGridObject[i].GetComponent<UnitStat>().MaxHp;
                 unit.Atk = inGridObject[i].GetComponent<UnitStat>().Atk;
                 unit.Def = inGridObject[i].GetComponent<UnitStat>().Def;
                 unit.buffs = inGridObject[i].GetComponent<UnitStat>().buffs;
                 myUnitData.Add(unit);
            }
        }
        for (int i = 0; i < enemyGridObject.Length; i++)
        {
            if (enemyGridObject[i] != null)
            {
               int id = int.Parse(enemyGridObject[i]);              
               UnitsDatas unit = new UnitsDatas();
               unit.Pos = i;
               unit.Id = id;
               unit.Atk = int.Parse(CsvDataManager.Instance.DataLists[5][id]["Damage"]);
               unit.Def = int.Parse(CsvDataManager.Instance.DataLists[5][id]["Armor"]);
               unit.MaxHp = int.Parse(CsvDataManager.Instance.DataLists[5][id]["MonsterHP"]);          
               enemyUnitData.Add(unit); 
            }
        }
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("StageBattleScene");
        BattleSceneStart();
    }
    public void BackStage()
    {
        // 스테이지패널 , 배틀씬 매니저정보 초기화 해야함
        for (int i = 0; i < inGridObject.Length; i++)
        {
            inGridObject[i] = null;
        }
        for (int i = 0; i < enemyGridObject.Length; i++)
        {
            enemyGridObject[i] = null;
        }
    }
    public void GetDraggables()
    {

        for (int i = 0; i < 13; i++)
        {
            if (Draggables[i] == null)
            {

                Draggables[i] = GameObject.Find("Slot" + i.ToString()).GetComponent<DraggableUI>();
                Draggables[i].gameObject.SetActive(false);

            }

        }
        Debug.Log(PlayerDataManager.Instance.PlayerData.UnitDatas.Count);
        for (int i = 0; i < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; i++) // db 에서 유닛 보유수 만큼만 보이게 
        {
            Draggables[i].gameObject.SetActive(true);
            int id = PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitId;
            int maxHp = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseHp"]);
            int atk = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseATK"]);
            int def = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseDef"]);
            string element = CsvDataManager.Instance.DataLists[0][id]["ElementID"];
            string name = CsvDataManager.Instance.DataLists[0][id]["Name"];
            string level = PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitLevel.ToString();
            Sprite sprite = Resources.Load<Sprite>("Portrait/portrait_" + id.ToString());
            Draggables[i].GetComponent<CharSlot>().setCharSlotData(id, name, level, sprite); // 이미지는 리소스 파일기준으로 사용하자
                                                                                             // 리소스 파일 이름에 id 같은거 포함 시키면 될듯함
            Draggables[i].GetComponent<UnitStat>().setStats(0, maxHp, atk, def, int.Parse(level), id, element);
        }
    }
    private void BattleSceneStart()
    {
        StartCoroutine(BattleSceneStartDelaying()); 
    }
    IEnumerator BattleSceneStartDelaying() 
    {
        yield return new WaitForSeconds(2f);
        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        spawner.SpawnUnits();

        foreach (int i in curItemValues.Keys) 
        {
            Debug.Log($"아이디 : {i} 수량 : {curItemValues[i]}");
        }
        
    }


}