using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemPanel : UIBInder
{
    private static ItemPanel _instance;
    public static ItemPanel Instance { get { return _instance; } set { _instance = value; } }

    [SerializeField] private SceneChanger _sceneChanger;

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

    private void Start()
    {
        BindAll();
        StartCoroutine(WaitForPlayerData());

        AddEvent("HomeButton", EventType.Click, GoLobby);
    }

    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() => PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        Init();
    }

    private void OnEnable()
    {
        if (PlayerDataManager.Instance.PlayerData.OnItemChanged == null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged = new UnityAction<int>[System.Enum.GetValues(typeof(E_Item)).Length];
        }

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoinText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += UpdateDinoBloodText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += UpdateBoneCrystalText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] += UpdateDinoStoneText;
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoinText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= UpdateDinoBloodText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= UpdateBoneCrystalText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] -= UpdateDinoStoneText;
        }
    }

    private void Init()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            UpdateCoinText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
            UpdateDinoBloodText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
            UpdateBoneCrystalText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
            UpdateDinoStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
        }
        else
        {
            Debug.Log("PlayerData 찾을 수 없음");
        }
    }

    private void UpdateCoinText(int newValue)
    {
        GetUI<TextMeshProUGUI>("CoinText").text = newValue.ToString();
        UpdateItemsInDatabase();
    }

    private void UpdateDinoBloodText(int newValue)
    {
        GetUI<TextMeshProUGUI>("DinoBloodText").text = newValue.ToString();
        UpdateItemsInDatabase();
    }

    private void UpdateBoneCrystalText(int newValue)
    {
        GetUI<TextMeshProUGUI>("BoneCrystalText").text = newValue.ToString();
        UpdateItemsInDatabase();
    }

    private void UpdateDinoStoneText(int newValue)
    {
        GetUI<TextMeshProUGUI>("DinoStoneText").text = newValue.ToString();
        UpdateItemsInDatabase();
    }

    public void UpdateItems()
    {
        UpdateCoinText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
        UpdateDinoBloodText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
        UpdateBoneCrystalText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
        UpdateDinoStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
    }

    private void UpdateItemsInDatabase()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            ["_items/3"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"아이템 업데이트 실패: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("아이템이 성공적으로 업데이트되었습니다.");
            }
        });
    }

    public void GoLobby(PointerEventData eventData)
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Lobby_OJH");
    }
}
