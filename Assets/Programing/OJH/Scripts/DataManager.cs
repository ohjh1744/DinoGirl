using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;


public class DataManager : MonoBehaviour
{
    private DataManager _instance;
    public DataManager Instance { get { return _instance; } set { _instance = value; } }

    // 불러올 csvData들
    // data개수만큼 인스펙터창에서 생성필요.
    [SerializeField] private string[] _datas;
    public string[] Datas { get { return _datas; } private set { } }

    // 추후 Url 추가예정.
    private string[] _urls = { "https://docs.google.com/spreadsheets/d/1KXfqH_i8RFvsEWVfqmSbnVuuLuMm6GvW3kF6nt5iMI4/export?format=csv"};

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
        for (int i = 0; i < _urls.Length; i++)
        {
            _request = UnityWebRequest.Get(_urls[i]);

            // 요청 후 파일다운로드 완료까지 대기
            yield return _request.SendWebRequest();

            //다운로드 완료 후 string에 저장.
            _datas[i] = _request.downloadHandler.text;

            Debug.Log(_datas[i]);
        }
    }


}
