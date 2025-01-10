using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ItemPanel : UIBInder
{
    [SerializeField] private SceneChanger _sceneChanger;

    private void Awake()
    {
        _sceneChanger = FindObjectOfType<SceneChanger>();
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
            PlayerDataManager.Instance.PlayerData.OnItemChanged = new UnityAction<int>[Enum.GetValues(typeof(E_Item)).Length];
        }

        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoinText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += UpdateDinoBloodText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += UpdateBoneCrystalText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] += UpdateDinoStoneText;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Stone] += UpdateStoneText;
    }

    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoinText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= UpdateDinoBloodText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= UpdateBoneCrystalText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] -= UpdateDinoStoneText;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Stone] -= UpdateStoneText;
        }
    }

    // 아이템 정보 초기화
    private void Init()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            UpdateCoinText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
            UpdateDinoBloodText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
            UpdateBoneCrystalText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
            UpdateDinoStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
            UpdateStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone]);
        }
        else
        {
            Debug.Log("PlayerData 찾을 수 없음");
        }

        /*
        LoadItemImage("CoinImage", E_Item.Coin);
        LoadItemImage("DinoBloodImage", E_Item.DinoBlood);
        LoadItemImage("BoneCrystalImage", E_Item.BoneCrystal);
        LoadItemImage("DinoStoneImage", E_Item.DinoStone);
        LoadItemImage("StoneImage", E_Item.Stone);
        */
    }

    // 아이템 숫자 갱신
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

    private void UpdateStoneText(int newValue)
    {
        GetUI<TextMeshProUGUI>("StoneText").text = newValue.ToString();
        UpdateItemsInDatabase();
    }

    // DataManager에 아이템 갱신
    public void UpdateItems()
    {
        UpdateCoinText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
        UpdateDinoBloodText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
        UpdateBoneCrystalText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
        UpdateDinoStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
        UpdateStoneText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone]);
    }

    // DB에 아이템 갱신
    private void UpdateItemsInDatabase()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            ["_items/3"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone],
            ["_items/4"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone]
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

    /*
    // 아이템 이미지 가져오기
    private void LoadItemImage(string imageName, E_Item itemType)
    {
        string itemPath = $"UI/item_{(int)itemType}";
        Sprite itemSprite = Resources.Load<Sprite>(itemPath);
        if(itemSprite != null)
        {
            GetUI<Image>(imageName).sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning($"이미지 찾을 수 없음 {itemPath}");
        }
    }
    */

    public void GoLobby(PointerEventData eventData)
    {
        _sceneChanger.CanChangeSceen = true;
        _sceneChanger.ChangeScene("Lobby_OJH");
    }
}
