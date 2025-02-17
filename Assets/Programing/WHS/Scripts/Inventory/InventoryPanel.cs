using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPanel : UIBInder
{
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private Transform _content;

    private List<PlayerUnitData> _allCharacters = new List<PlayerUnitData>();

    private Dictionary<int, Dictionary<string, string>> _characterData;

    [SerializeField] private AudioClip _bgmClip;

    private void Awake()
    {
        BindAll();
        AddEvent("AllElementButton", EventType.Click, AllElementButtonClicked);
        AddEvent("FireElementButton", EventType.Click, FireElementButtonClicked);
        AddEvent("WaterElementButton", EventType.Click, WaterElementButtonClicked);
        AddEvent("GroundElementButton", EventType.Click, GroundElementButtonClicked);
        AddEvent("GrassElementButton", EventType.Click, GrassElementButtonClicked);

        SoundManager.Instance.PlayeBGM(_bgmClip);
    }

    private void Start()
    {
        _characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        PopulateGrid();
    }

    private void OnDisable()
    {
        SoundManager.Instance.StopBGM();
    }

    /*
    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() => PlayerDataManager.Instance.PlayerData.UnitDatas != null && PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        _characterData = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];

        PopulateGrid();
    }
    */

    // 그리드에 가진 캐릭터 정렬
    private void PopulateGrid()
    {
        // 목록 초기화
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        // Firebase에서 UnitDatas를 가져와 스크롤뷰에 캐릭터 정렬
        string userID = BackendManager.Instance.Auth.CurrentUser.UserId;

        DatabaseReference unitDatasRef = BackendManager.Instance.Database.RootReference
            .Child("UserData").Child(userID).Child("_unitDatas");

        unitDatasRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("유닛 데이터 로딩 중 오류 발생: " + task.Exception);
                return;
            }

            _allCharacters.Clear();
            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                PlayerUnitData unitData = new PlayerUnitData
                {
                    UnitId = int.Parse(childSnapshot.Child("_unitId").Value.ToString()),
                    UnitLevel = int.Parse(childSnapshot.Child("_unitLevel").Value.ToString())
                };

                _allCharacters.Add(unitData);
            }

            Debug.Log($"캐릭터 {_allCharacters.Count}개 로드");

            _allCharacters = PlayerDataManager.Instance.PlayerData.UnitDatas;
            DisplayCharacters(_allCharacters);
        });
    }

    // 바뀐 캐릭터 갱신
    public void UpdateCharacterUI(PlayerUnitData updatedCharacter)
    {
        foreach (Transform child in _content)
        {
            CharacterSlot slotUI = child.GetComponent<CharacterSlot>();
            if (slotUI.GetCharacter().UnitId == updatedCharacter.UnitId)
            {
                slotUI.SetCharacter(updatedCharacter);
                break;
            }
        }
    }

    // 보유한 캐릭터 출력
    private void DisplayCharacters(List<PlayerUnitData> characters)
    {
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerUnitData unitData in characters)
        {
            GameObject slot = Instantiate(_characterPrefab, _content);
            CharacterSlot slotUI = slot.GetComponent<CharacterSlot>();
            slotUI.SetCharacter(unitData);
        }
    }

    // 원소 ID 받아오기
    private int GetElementId(int unitId)
    {
        if (_characterData.TryGetValue(unitId, out var data))
        {
            int elementID = int.Parse(data["ElementID"]);
            Debug.Log($"UnitID: {unitId}, ElementID: {elementID}");
            return elementID;
        }
        Debug.LogWarning($"No data found for UnitID: {unitId}");
        return 0;
    }

    // ElementID에 따라 정렬 1땅 2불 3물 4풀
    private void AllElementButtonClicked(PointerEventData eventData)
    {
        Debug.Log("All");
        DisplayCharacters(_allCharacters);
    }

    private void FireElementButtonClicked(PointerEventData eventData)
    {
        Debug.Log("Fire");
        DisplayCharacters(_allCharacters.Where(c => GetElementId(c.UnitId) == 2).ToList());
    }

    private void WaterElementButtonClicked(PointerEventData eventData)
    {
        Debug.Log("Water");
        DisplayCharacters(_allCharacters.Where(c => GetElementId(c.UnitId) == 3).ToList());
    }

    private void GroundElementButtonClicked(PointerEventData eventData)
    {
        Debug.Log("Ground");
        DisplayCharacters(_allCharacters.Where(c => GetElementId(c.UnitId) == 1).ToList());
    }

    private void GrassElementButtonClicked(PointerEventData eventData)
    {
        Debug.Log("grass");
        DisplayCharacters(_allCharacters.Where(c => GetElementId(c.UnitId) == 4).ToList());
    }
}
