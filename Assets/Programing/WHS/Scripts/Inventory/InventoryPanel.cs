using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public GameObject characterPrefab;  // 캐릭터 칸 프리팹
    public Transform content;           // 스크롤뷰의 content

    private void Start()
    {
        StartCoroutine(WaitForPlayerData());
    }
    
    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() => PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

        PopulateGrid();
    }
    
    // 그리드에 가진 캐릭터 정렬
    private void PopulateGrid()
    {
        // 목록 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Firebase에서 UnitDatas를 가져와 스크롤뷰에 캐릭터 정렬
        string userID = BackendManager.Auth.CurrentUser.UserId;

        DatabaseReference unitDatasRef = BackendManager.Database.RootReference
            .Child("UserData").Child(userID).Child("_unitDatas");

        unitDatasRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("유닛 데이터 로딩 중 오류 발생: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            foreach (var childSnapshot in snapshot.Children)
            {
                PlayerUnitData unitData = new PlayerUnitData
                {
                    UnitId = int.Parse(childSnapshot.Child("_unitId").Value.ToString()),
                    UnitLevel = int.Parse(childSnapshot.Child("_unitLevel").Value.ToString())
                    // TODO : 슬롯에 캐릭터 이미지도 띄워야할것
                };

                GameObject slot = Instantiate(characterPrefab, content);
                CharacterSlot slotUI = slot.GetComponent<CharacterSlot>();
                slotUI.SetCharacter(unitData);
            }
        });
    }

    // 바뀐 캐릭터 갱신
    public void UpdateCharacterUI(PlayerUnitData updatedCharacter)
    {
        foreach (Transform child in content)
        {
            CharacterSlot slotUI = child.GetComponent<CharacterSlot>();
            if (slotUI.GetCharacter().UnitId == updatedCharacter.UnitId)
            {
                slotUI.SetCharacter(updatedCharacter);
                break;
            }
        }
    }    
}
