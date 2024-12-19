using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// 알아두기
// inspector 창에서 넣는 _urls의 형식은 마지막 /edit~ 부분을 /export?=format=csv 형식으로 변환하여 사용
// ex)
// https://docs.google.com/spreadsheets/d/1KXfqH_i8RFvsEWVfqmSbnVuuLuMm6GvW3kF6nt5iMI4/edit?gid=0#gid=0 이 url을
// https://docs.google.com/spreadsheets/d/1KXfqH_i8RFvsEWVfqmSbnVuuLuMm6GvW3kF6nt5iMI4/export?=format=csv 로 변경

public class RnDDataManager : MonoBehaviour
{
    private static RnDDataManager _instance;
    public static RnDDataManager Instance { get { return _instance; } set { _instance = value; } }

    //csv 링크들
    [SerializeField] private string[] _urls;

    //csvData들
    [SerializeField] private string[] _csvDatas;

    //csvData Parsing한 Data들
    public List<Dictionary<string, string>>[] DataLists { get; set; }

    UnityWebRequest _request;

    private Coroutine _downLoadRoutine;

    private bool _isLoad;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            if (_isLoad == false)
            {
                _downLoadRoutine = StartCoroutine(DownloadRoutine());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DownloadRoutine()
    {
        _isLoad = true;

        // Data 초기화 
        DataLists = new List<Dictionary<string, string>>[_csvDatas.Length];
        for (int i = 0; i < DataLists.Length; i++)
        {
            DataLists[i] = new List<Dictionary<string, string>>();
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
    }

    List<Dictionary<string, string>> ChangeCsvToList(string data)
    {
        List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

        string[] lines = data.Split('\n');

        Debug.Log(lines.Length);

        // CSV 첫 줄은 헤더
        string[] headers = lines[0].Split(',');

        // CSV 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            Dictionary<string, string> dataDic = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length; j++)
            {
                dataDic[headers[j].Trim()] = values[j].Trim();
            }

            dataList.Add(dataDic);
        }

        //foreach (var expando in dataList)
        //{
        //    foreach (var pair in expando)
        //    {
        //        Debug.Log($"{pair.Key}: {pair.Value}");
        //    }
        //}

        return dataList;
    }




}
