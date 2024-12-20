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

    private List<Character> characterList = new List<Character>();

    private void Start()
    {
        // 캐릭터는 데이터베이스에서 받아와서 쓸것,  스크롤뷰 확인용
        AddCharacter(new Character { ID = 10, Name = "tyrano", level = 10 });
        AddCharacter(new Character { ID = 11, Name = "yrano", level = 50 });
        AddCharacter(new Character { ID = 12, Name = "rano", level = 33 });
        AddCharacter(new Character { ID = 13, Name = "ano", level = 44 });
        AddCharacter(new Character { ID = 14, Name = "no", level = 12 });
        AddCharacter(new Character { ID = 15, Name = "o", level = 21 });
        AddCharacter(new Character { ID = 16, Name = "tyrano", level = 10 });
        AddCharacter(new Character { ID = 17, Name = "yrano", level = 50 });
        AddCharacter(new Character { ID = 18, Name = "rano", level = 33 });
        AddCharacter(new Character { ID = 19, Name = "ano", level = 44 });
        AddCharacter(new Character { ID = 20, Name = "no", level = 12 });
        AddCharacter(new Character { ID = 21, Name = "o", level = 21 });

        PopulateGrid();
    }

    // 캐릭터 추가하기
    private void AddCharacter(Character character)
    {
        characterList.Add(character);
    }

    // 그리드에 가진 캐릭터 정렬
    private void PopulateGrid()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 스크롤뷰에 캐릭터 정렬
        foreach (Character character in characterList)
        {
            GameObject slot = Instantiate(characterPrefab, content);
            CharacterSlotUI slotUI = slot.GetComponent<CharacterSlotUI>();

            // 가진 캐릭터 정보 세팅
            slotUI.SetCharacter(character);
        }
    }

    // 바뀐 캐릭터 갱신
    public void UpdateCharacterUI(Character updatedCharacter)
    {
        foreach (Transform child in content)
        {
            CharacterSlotUI slotUI = child.GetComponent<CharacterSlotUI>();
            if (slotUI.GetCharacter().ID == updatedCharacter.ID)
            {
                slotUI.SetCharacter(updatedCharacter);
                break;
            }
        }
    }
}
