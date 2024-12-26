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

    // private List<PlayerUnitData> characterList = new List<PlayerUnitData>();

    private void Start()
    {
        StartCoroutine(WaitForPlayerData());
    }

    private IEnumerator WaitForPlayerData()
    {
        // PlayerDataManager가 초기화되고 PlayerData가 로드될 때까지 대기
        yield return new WaitUntil(() =>
            PlayerDataManager.Instance != null &&
            PlayerDataManager.Instance.PlayerData != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas != null &&
            PlayerDataManager.Instance.PlayerData.UnitDatas.Count > 0);

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

        // PlayerDataManager에서 UnitDatas를 가져와 스크롤뷰에 캐릭터 정렬
        foreach (PlayerUnitData unitData in PlayerDataManager.Instance.PlayerData.UnitDatas)
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
            if (slotUI.GetCharacter().UnitId == updatedCharacter.UnitId)
            {
                slotUI.SetCharacter(updatedCharacter);
                break;
            }
        }
    }    
}
