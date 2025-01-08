using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;


public enum E_CsvData { Character, Element, Stat, CharacterSkill, CharacterLevelUp,
    Monster, Stages, StageReward, MontserGroup, Item, Gacha, GachaReturn, Raids, RaidReward, WeeklyReward, Housing}

public class CsvDataManager : MonoBehaviour
{
    private static CsvDataManager _instance;
    public static CsvDataManager Instance { get { return _instance; } set { _instance = value; } }

    //csv 링크들
    [SerializeField] private string[] _urls;

    //csvData들
    [SerializeField] private string[] _csvDatas;

    //csvData Parsing한 Data들
    public Dictionary<int, Dictionary<string, string>>[] DataLists { get; set; }

    UnityWebRequest _request;

    private Coroutine _downLoadRoutine;

    private bool _isLoad;

    public bool IsLoad { get { return _isLoad; } private set { } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _downLoadRoutine = StartCoroutine(DownloadRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DownloadRoutine()
    {
        // Data 초기화 
        DataLists = new Dictionary<int, Dictionary<string, string>>[_csvDatas.Length];
        for (int i = 0; i < DataLists.Length; i++)
        {
            DataLists[i] = new Dictionary<int, Dictionary<string, string>>();
        }

        for (int i = 0; i < _urls.Length; i++)
        {
            _request = UnityWebRequest.Get(_urls[i]);

            // 요청 후 파일다운로드 완료까지 대기
            yield return _request.SendWebRequest();

            //다운로드 완료 후 string에 저장.
            _csvDatas[i] = _request.downloadHandler.text;

            //Parsing에서 List에 저장.
            DataLists[i] = ChangeCsvToList(_csvDatas[i]);
        }

        _isLoad = true;
    }

    Dictionary< int, Dictionary<string, string>> ChangeCsvToList(string data)
    {
        Dictionary<int, Dictionary<string, string>> dataList = new Dictionary<int, Dictionary<string, string>>();

        string[] lines = data.Split('\n');

        // CSV 첫 줄은 헤더
        string[] headers = lines[0].Split(',');

        // CSV 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(",");
            Dictionary<string, string> dataDic = new Dictionary<string, string>();

            //id는 제외하고 다음속석부터를 위해서 1부터
            for (int j = 1; j < headers.Length; j++)
            {
                dataDic[headers[j].Trim()] = values[j].Trim();
            }

            //values[0]은 
            dataList[TypeCastManager.Instance.TryParseInt(values[0])] = dataDic;
        }

        return dataList;
    }


}
