using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private PlayerData _playerData;

    public PlayerData PlayerData {  get { return _playerData; } set { _playerData = value; } }

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

}
