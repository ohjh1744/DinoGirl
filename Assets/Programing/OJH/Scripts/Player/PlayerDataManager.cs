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

}
