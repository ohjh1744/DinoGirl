using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private SceneChanger _sceneChanager;

    [SerializeField] private string _nextSceneName;

    void Start()
    {
        _sceneChanager.ChangeScene(_nextSceneName);
    }

}
