using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private SceneChanger _sceneChanager;

    [SerializeField] private string _nextSceneName;

    //Bgm
    [SerializeField] private AudioClip _bgmClip;

    private bool _isStart;

    private void Start()
    {
        SoundManager.Instance.SetLoopBGM(false);
        SoundManager.Instance.PlayeBGM(_bgmClip);
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
