using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private PlayerData _playerData;

    public PlayerData PlayerData {  get { return _playerData; } private set { } }

    private int[] _housingIDs = new int[(int)E_Item.Length];

    private static int[] _itemIDs = { 500, 501, 502, 530, 504 };

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
    }
    private void Update()
    {
        //if (_instance == null)
            return;

        //UpdateItemValue();
    }

    // 재화별 1분당 증가하는 개수를 불러오기 위한 HOusing ID 찾은후 저장.
    // LobbyPanel Start에서 호출.
    public void LoadHousingIDs()
    {
        Dictionary<int, Dictionary<string, string>> itemDic = CsvDataManager.Instance.DataLists[(int)E_CsvData.Item];

        //Item별 HousingID 저장.
        for(int i = 0; i < (int)E_Item.Length; i++)
        {
            _housingIDs[i] = TypeCastManager.Instance.TryParseInt(itemDic[_itemIDs[i]]["HousingID"]);
        }
    }

    public void CheckCleaerStage()
    {

    }

    private void UpdateItemValue()
    {
        //첫 로딩씬과 로그인씬에서는 제외.
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            return;

        //Player stage클리어 여부 먼저 확인하기

       
        
    }



}
