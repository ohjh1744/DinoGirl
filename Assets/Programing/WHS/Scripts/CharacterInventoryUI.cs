using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventoryUI : MonoBehaviour
{
    public GameObject characterPrefab;  // 캐릭터 칸 프리팹
    public Transform content;           // 스크롤뷰의 content

    private List<PlayerUnitData> characterList = new List<PlayerUnitData>();

    private void Start()
    {
        StartCoroutine(WaitForFirebaseInitialization());
    }

    // 로그인을 생략해서 데이터베이스 연동 데스트용
    private IEnumerator WaitForFirebaseInitialization()
    {
        while (BackendManager.Database == null || BackendManager.Database.RootReference == null)
        {
            yield return null;
        }
        LoadCharacter();
    }

    // 파이어베이스에서 캐릭터 받아오기
    private void LoadCharacter()
    {
        // 로그인을 생략해 임시로 userID 지정
        string userID = "poZb90DRTiczkoC5TpHOpaJ5AXR2";
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(userID).Child("_unitDatas");

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("캐릭터 데이터 로딩 취소됨");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("캐릭터 데이터 로딩중 오류 발생 " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            characterList.Clear();

            foreach(DataSnapshot childSnapshot in snapshot.Children)
            {
                string json = childSnapshot.GetRawJsonValue();
                PlayerUnitData character = JsonUtility.FromJson<PlayerUnitData>(json);
                characterList.Add(character);
            }

            PopulateGrid();

        });
    }

    // 그리드에 가진 캐릭터 정렬
    private void PopulateGrid()
    {
        // 목록 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 스크롤뷰에 캐릭터 정렬
        foreach (PlayerUnitData unitData in characterList)
        {
            GameObject slot = Instantiate(characterPrefab, content);
            CharacterSlotUI slotUI = slot.GetComponent<CharacterSlotUI>();

            // 가진 캐릭터 정보 세팅
            slotUI.SetCharacter(unitData);
        }
    }

    // 바뀐 캐릭터 갱신
    public void UpdateCharacterUI(PlayerUnitData updatedCharacter)
    {
        foreach (Transform child in content)
        {
            CharacterSlotUI slotUI = child.GetComponent<CharacterSlotUI>();
            if (slotUI.GetCharacter().Name == updatedCharacter.Name)
            {
                slotUI.SetCharacter(updatedCharacter);
                break;
            }
        }
    }
}
