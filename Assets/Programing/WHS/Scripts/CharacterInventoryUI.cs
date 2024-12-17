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
        Sprite tyranoSprite = Resources.Load<Sprite>("1");
        Sprite yranoSprite = Resources.Load<Sprite>("2");
        Sprite ranoSprite = Resources.Load<Sprite>("3");
        Sprite anoSprite = Resources.Load<Sprite>("4");
        Sprite noSprite = Resources.Load<Sprite>("5");
        Sprite oSprite = Resources.Load<Sprite>("6");

        AddCharacter(new Character { Name = "tyrano", level = 10, image = tyranoSprite });
        AddCharacter(new Character { Name = "yrano", level = 50, image = yranoSprite });
        AddCharacter(new Character { Name = "rano", level = 33, image = ranoSprite });
        AddCharacter(new Character { Name = "ano", level = 44, image = anoSprite });
        AddCharacter(new Character { Name = "no", level = 12, image = noSprite });
        AddCharacter(new Character { Name = "o", level = 21, image = oSprite });

        PopulateGrid();
    }

    // 캐릭터 추가하기
    public void AddCharacter(Character character)
    {
        characterList.Add(character);
    }

    // 그리드에 가진 캐릭터 정렬
    public void PopulateGrid()
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 캐릭터 정렬
        foreach(Character character in characterList)
        {
            GameObject slot = Instantiate(characterPrefab, content);
            CharacterSlotUI slotUI = slot.GetComponent<CharacterSlotUI>();

            // 가진 캐릭터 정보 세팅
            slotUI.SetCharacter(character);
        }
    }

    // 보유하지 않은 캐릭터도 정렬?

    // 보유하지 않은 캐릭터는 누르면 상점으로 이동하게?

    // 상점에서는 캐릭터를 구매하면 가진 목록에 추가?
}
