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



    [SerializeField] private DraggableUI[] Draggables;


    //BaseUnitController 로 전달이 가능해야 함 
    [SerializeField] public GameObject[] inGridObject; // 아군 정보 배열 나중에 타입을 바꾸면 될듯
    [SerializeField] public List<BaseUnitController> myUnits;
    [SerializeField] public List<BaseUnitController> enemyUnits;

    [SerializeField] public string[] enemyGridObject;// 적 정보 배열 배열의 인덱스가 위치임 , id 저장 

    [SerializeField] public int _timeLimit; // private 프로퍼티로 바꿀 예정

    [SerializeField] public Dictionary<int, int> curItemValues = new Dictionary<int, int>();// 클리어 보상
        
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
        myUnits = new List<BaseUnitController>();
        enemyUnits = new List<BaseUnitController>();
    }
    public UnityEvent startStage;
    public void StageStart()
    {
        // 1 ~ 5인만 출발 가능
        for (int i = 1; i < inGridObject.Length; i++) 
        {
            if (inGridObject[i] != null) 
            {
                UnitStat unitStat = inGridObject[i].GetComponent<UnitStat>();
                string x =" ";
                for (int j = 0; j < unitStat.buffs.Count; j++) // 적용 방식은 고민해봐야 할듯
                {
                    x += unitStat.buffs[j].y.ToString() + ":" + unitStat.buffs[j].y.ToString() + " , ";
                }
                Debug.Log($"위치 : {i} id :  {unitStat.Id} 레벨 {unitStat.Level} 적용된 버프종류 : {x}"); //나중에 적용된 버프들 ui에 뜨면 좋을듯 리스트는 실시간으로 적용중이니 
            }
        }
        for (int i = 0; i < enemyGridObject.Length; i++) 
        {
            if (enemyGridObject[i] != null) 
            {
                int id = int.Parse(enemyGridObject[i]);
                string name = CsvDataManager.Instance.DataLists[5][id]["MonsterName"];
                string damage = CsvDataManager.Instance.DataLists[5][id]["Damage"];
                string armor = CsvDataManager.Instance.DataLists[5][id]["Armor"];
                string hp = CsvDataManager.Instance.DataLists[5][id]["MonsterHP"];
                Debug.Log($"위치 :  {i+1} id : {id} 이름 : {name} 공 : {damage} 방 :{armor} 체 : {hp}");
            }
        }
        
    }
    public void BackStage()
    {   
         
        // 스테이지패널 , 배틀씬 매니저정보 초기화 해야함
        for (int i = 0; i < inGridObject.Length; i++)
        {
            
            if (inGridObject[i] != null) 
            {
               // inGridObject[i].GetComponent<DraggableUI>().ResetDragables();
            }
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
            Sprite sprite = Resources.Load<Sprite>("Portrait/portrait_"+id.ToString());
            Draggables[i].GetComponent<CharSlot>().setCharSlotData(id,name, level,sprite); // 이미지는 리소스 파일기준으로 사용하자
                                                                                           // 리소스 파일 이름에 id 같은거 포함 시키면 될듯함
            Draggables[i].GetComponent<UnitStat>().setStats(maxHp,atk,def,int.Parse(level),id,element);
            //지금 형태는 csv에서 바로 가져오는 형태임 ,이걸 여기서 하는게 맞나 ?   
        }

    }

    private void BattleSceneStart()
    {
        // 적 위치, 종류(id?)
        // 아군 위치, 종류 최종 스탯 
        // 스테이지 시간 

        Spawner spawner = FindAnyObjectByType<Spawner>();

        spawner.SpawnUnits();

    }


}