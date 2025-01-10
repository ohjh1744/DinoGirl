using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Cinemachine.DocumentationSortingAttribute;

public class BattleSceneManager : MonoBehaviour
{
    private static BattleSceneManager _instance;
    public static BattleSceneManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private SceneChanger _sceneChanger;

    [SerializeField] private DraggableUI[] Draggables;
    [SerializeField] public int curChapterNum { get; set; }

    public bool isAutoOn { get; set; }
    public bool isGamePaused { get; set; }


    [SerializeField] public GameObject[] inGridObject;
    [SerializeField] public string[] enemyGridObject;
    [SerializeField] public int curStageNum;

    [SerializeField] public List<PlayableBaseUnitController> myUnits;
    [SerializeField] public List<BaseUnitController> enemyUnits;
    [SerializeField] public List<UnitsDatas> myUnitData;
    [SerializeField] public List<UnitsDatas> enemyUnitData;

    [SerializeField] public float _timeLimit;

    [SerializeField] TMP_Text BattlePower;
    [SerializeField] TMP_Text tacPower;
    [SerializeField] public int Power;
    [SerializeField] public int incBuffsPowers;

    [SerializeField] public Dictionary<int, int> curItemValues = new Dictionary<int, int>();

    [SerializeField] public BattleState curBattleState;
    public enum BattleState
    {
        Ready, Battle, Win, Lose, Stop, size
    }
    public int inGridObjectCount { get; set; }
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
        curBattleState = BattleState.Ready;

        inGridObject = new GameObject[10];
        enemyGridObject = new string[9];
        /*myUnits = new List<PlayableBaseUnitController>();
        enemyUnits = new List<BaseUnitController>();*/
        myUnitData = new List<UnitsDatas>();
        enemyUnitData = new List<UnitsDatas>();
    }

    public UnityEvent startStage;
    public void StageStart()
    {

        inGridObjectCount = inGridObject.Count(inGridObject => inGridObject != null); // 출발 인원 체크
        Debug.Log($"시작버튼 눌림{inGridObjectCount}");
        if (inGridObjectCount >= 1 && inGridObjectCount <= 5)
        {
            Debug.Log($"출발 인원{inGridObjectCount}");


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
                    unit.Increase = inGridObject[i].GetComponent<UnitStat>().Increase;
                    unit.Cool = inGridObject[i].GetComponent<UnitStat>().Cool;
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
            switch (curChapterNum)
            {
                case 0:
                    _sceneChanger.ChangeScene("StageBattleScene_LJH");
                    Debug.Log($"0번으로 이동");
                    BattleSceneStart();
                    break;
                case 7:
                    _sceneChanger.ChangeScene("StageBattleScene1_LJH");
                    Debug.Log($"1번으로 이동");
                    BattleSceneStart();
                    break;
                case 14:
                    _sceneChanger.ChangeScene("StageBattleScene2_LJH");
                    Debug.Log($"2번으로 이동");
                    BattleSceneStart();
                    break;
            }

        }
        else
        {
            Debug.Log($"출발 인원 초과 or 부족{inGridObjectCount}");
        }
    }
    public void BackStage()
    {

        for (int i = 0; i < inGridObject.Length; i++)
        {
            inGridObject[i] = null;
        }
        for (int i = 0; i < enemyGridObject.Length; i++)
        {
            Debug.Log(enemyGridObject[i]);
            enemyGridObject[i] = null;
        }
    }
    public void GetDraggables()
    {
        BattlePower.text = "Power :";
        tacPower.text = "Grid :";
        Power = 0;
        incBuffsPowers = 0;
        for (int i = 0; i < 13; i++)
        {
            if (Draggables[i] == null)
            {

                Draggables[i] = GameObject.Find("Slot" + i.ToString()).GetComponent<DraggableUI>();
                Draggables[i].gameObject.SetActive(false);

            }

        }
        Debug.Log(PlayerDataManager.Instance.PlayerData.UnitDatas.Count);
        for (int i = 0; i < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; i++) // db ���� ���� ������ ��ŭ�� ���̰� 
        {

            Draggables[i].gameObject.SetActive(true);
            int id = PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitId;
            int maxHp = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseHp"]);
            int atk = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseATK"]);
            int def = int.Parse(CsvDataManager.Instance.DataLists[0][id]["BaseDef"]);
            int inc = int.Parse(CsvDataManager.Instance.DataLists[0][id]["Increase"]);
            string element = CsvDataManager.Instance.DataLists[0][id]["ElementID"];
            string name = CsvDataManager.Instance.DataLists[0][id]["Name"];
            string level = PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitLevel.ToString();

            Sprite sprite = Resources.Load<Sprite>("Portrait/portrait_" + id.ToString());
            Draggables[i].GetComponent<CharSlot>().setCharSlotData(id, name, level, sprite); // �̹����� ���ҽ� ���ϱ������� �������
                                                                                             // ���ҽ� ���� �̸��� id ������ ���� ��Ű�� �ɵ���
            Draggables[i].GetComponent<UnitStat>().setStats(0, maxHp, atk, def, int.Parse(level), id, element, inc, 1);
        }
    }
    private void BattleSceneStart()
    {
        StartCoroutine(BattleSceneStartDelaying());
    }
    IEnumerator BattleSceneStartDelaying()
    {
        yield return new WaitForSeconds(5f);
        Spawner spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        spawner.SpawnUnits();
    }

    public void GoLobby()
    {
        _sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Lobby_OJH");
        Destroy(gameObject);
    }
    public void GoChapter()
    {
        _sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("ChapterSelect_LJH");
        Destroy(gameObject);
    }
    public void ClearBuffs()
    {
        for (int i = 0; i < Draggables.Length; i++)
        {
            Draggables[i].GetComponent<CharSlot>().buffCount = Draggables[i].gameObject.GetComponent<UnitStat>().buffs.Count;
        }
    }


    IEnumerator delayingPowerCheck()
    {
        Power = 0;
        incBuffsPowers = 0;
        yield return new WaitForSeconds(0.1f);
        for (int i = 1; i < inGridObject.Length; i++)
        {
            if (inGridObject[i] != null)
            {
                double inc = 1 + (((inGridObject[i].GetComponent<UnitStat>().Level - 1) * (inGridObject[i].GetComponent<UnitStat>().Increase)) / 100.0);
                int atk = (int)(inc * inGridObject[i].GetComponent<UnitStat>().Atk);
                int incBuffs = 0;
                for (int j = 0; j < inGridObject[i].GetComponent<UnitStat>().buffs.Count; j++)
                {
                    incBuffs += inGridObject[i].GetComponent<UnitStat>().buffs[j].z;
                }
                Power += atk;
                incBuffsPowers += incBuffs;
            }
        }
        BattlePower.text = "Power :" + Power.ToString();
        tacPower.text = "Grid :" + incBuffsPowers.ToString();

    }
    public void TotalPowerUpdate()
    {
        StartCoroutine(delayingPowerCheck());
    }

}