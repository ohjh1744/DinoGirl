using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomPanel : UIBInder
{
    private IdleReward _idleReward;
    [SerializeField] private GameObject _idleRewardPanel;
    [SerializeField] private ItemPanel _itemPanel;

    private Coroutine _updateIdleTimeCoroutine;

    private void Awake()
    {
        BindAll();

        AddEvent("ClaimButton", EventType.Click, ClaimIdleRewards);
        AddEvent("CheckRewardButton", EventType.Click, ShowIdleRewardPanel);
    }

    private void Start()
    {
        _idleReward = GetComponent<IdleReward>();
        if (_idleReward == null)
        {
            Debug.Log("idleReward없음");
        }

        _idleReward.CalculateIdleReward();

        if (_updateIdleTimeCoroutine == null)
        {
            _updateIdleTimeCoroutine = StartCoroutine(UpdateIdleTimeCoroutine());
        }
    }

    private void OnEnable()
    {


    }
    private void OnDisable()
    {
        if (_updateIdleTimeCoroutine != null)
        {
            StopCoroutine(_updateIdleTimeCoroutine);
            _updateIdleTimeCoroutine = null;
        }
    }

    // 보상 수령
    public void ClaimIdleRewards(PointerEventData eventData)
    {
        if (GetUI<Button>("ClaimButton").interactable == false)
            return;

        int storedGold = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.Coin];
        int storedDinoBlood = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.DinoBlood];
        int storedBoneCrystal = PlayerDataManager.Instance.PlayerData.StoredItems[(int)E_Item.BoneCrystal];

        // 방치형 보상을 플레이어의 아이템에 추가
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] + storedGold);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoBlood, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood] + storedDinoBlood);
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.BoneCrystal, PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal] + storedBoneCrystal);

        // 방치형 보상 비우기
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.Coin, 0);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.DinoBlood, 0);
        PlayerDataManager.Instance.PlayerData.SetStoredItem((int)E_Item.BoneCrystal, 0);

        ShowPopup(storedGold, storedDinoBlood, storedBoneCrystal);

        // Firebase에 업데이트된 아이템 정보 저장
        UpdateItemsInDatabase();

        // 방치형보상 수령 후 종료시간 저장
        _idleReward.SaveExitTime();

        _idleRewardPanel.SetActive(false);

        _itemPanel.UpdateItems();
    }

    // 데이터베이스에 아이템 저장
    private void UpdateItemsInDatabase()
    {
        string userId = BackendManager.Auth.CurrentUser.UserId;
        DatabaseReference userRef = BackendManager.Database.RootReference.Child("UserData").Child(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            ["_items/0"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin],
            ["_items/1"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood],
            ["_items/2"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal],
            ["_storedItems/0"] = 0,
            ["_storedItems/1"] = 0,
            ["_storedItems/2"] = 0
        };

        userRef.UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"방치형 아이템 갱신 실패 {task.Exception}");
            }
            if (task.IsCanceled)
            {
                Debug.LogError($"방치형 아이템 갱신 중단됨 {task.Exception}");
            }
            Debug.Log("방치형 보상 수령 성공");
        });
    }

    // 방치시간 타이머 텍스트
    private IEnumerator UpdateIdleTimeCoroutine()
    {
        DateTime lastTime = DateTime.Now;

        while (true)
        {
            TimeSpan idleTime = _idleReward.GetIdleTime();

            GetUI<TextMeshProUGUI>("IdleTimeText").text = $"idleTime {idleTime.Hours} : {idleTime.Minutes} : {idleTime.Seconds}";
            GetUI<Button>("ClaimButton").interactable = _idleReward.HasIdleReward();

            TimeSpan elapsedTime = DateTime.Now - lastTime;

            if (elapsedTime.TotalSeconds >= 10)
            {
                _idleReward.CalculateIdleReward();
                lastTime = DateTime.Now;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // 방치형보상 UI 패널
    private void ShowIdleRewardPanel(PointerEventData eventData)
    {
        GetUI<Button>("ClaimButton").interactable = _idleReward.HasIdleReward();

        _idleReward.CalculateIdleReward();

        _idleRewardPanel.SetActive(true);

        int goldReward = _idleReward.CalculateReward(1, (int)_idleReward.GetIdleTime().TotalSeconds);
        int dinoBloodReward = _idleReward.CalculateReward(2, (int)_idleReward.GetIdleTime().TotalSeconds);
        int boneCrystalReward = _idleReward.CalculateReward(3, (int)_idleReward.GetIdleTime().TotalSeconds);

        GetUI<TextMeshProUGUI>("CoinRewardText").text = $"Coin: {goldReward}";
        GetUI<TextMeshProUGUI>("DinoBloodRewardText").text = $"Dino Blood: {dinoBloodReward}";
        GetUI<TextMeshProUGUI>("BoneCrystalRewardText").text = $"Bone Crystal: {boneCrystalReward}";

        LoadItemImage("CoinRewardImage", E_Item.Coin);
        LoadItemImage("DinoBloodRewardImage", E_Item.DinoBlood);
        LoadItemImage("BoneCrystalRewardImage", E_Item.BoneCrystal);

        GetUI<Button>("ClaimButton").interactable = _idleReward.HasIdleReward();
    }

    private void ShowPopup(int gold, int dinoBlood, int boneCrystal)
    {
        GetUI<TextMeshProUGUI>("CoinClaimText").text = $"Coin : {gold}";
        GetUI<TextMeshProUGUI>("DinoBloodClaimText").text = $"Dino Blood : {dinoBlood}";
        GetUI<TextMeshProUGUI>("BoneCrystalClaimText").text = $"Bone Crystal : {boneCrystal}";

        LoadItemImage("CoinClaimImage", E_Item.Coin);
        LoadItemImage("DinoBloodClaimImage", E_Item.DinoBlood);
        LoadItemImage("BoneCrystalClaimImage", E_Item.BoneCrystal);
    }

    private void LoadItemImage(string imageName, E_Item itemType)
    {
        string imagePath = $"UI/item_{(int)itemType}";
        Sprite itemSprite = Resources.Load<Sprite>(imagePath);
        if (itemSprite != null)
        {
            GetUI<Image>(imageName).sprite = itemSprite;
        }
        else
        {
            Debug.LogWarning($"이미지 찾을 수 없음 {imagePath}");
        }
    }
}