using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private SceneChanger _sceneChanager;

    [SerializeField] private string _nextSceneName;

    private bool _isStart;

    private void Start()
    {
        _sceneChanager.CanChangeSceen = false;
        _sceneChanager.ChangeScene(_nextSceneName);
    }

    private void Update()
    {
        if(CsvDataManager.Instance.IsLoad == true && _isStart == false)
        {
            _isStart = true;
            _sceneChanager.CanChangeSceen = true;
        }
    }

}
