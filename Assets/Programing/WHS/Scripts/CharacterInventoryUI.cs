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

        AddCharacter(new Character { name = "tyrano", level = 10, image = tyranoSprite });
        AddCharacter(new Character { name = "yrano", level = 50, image = yranoSprite });
        AddCharacter(new Character { name = "rano", level = 33, image = ranoSprite });
        AddCharacter(new Character { name = "ano", level = 44, image = anoSprite });
        AddCharacter(new Character { name = "no", level = 12, image = noSprite });
        AddCharacter(new Character { name = "o", level = 21, image = oSprite });

        PopulateGrid();
    }

    // 캐릭터 추가하기
    public void AddCharacter(Character character)
    {
        characterList.Add(character);
    }

    // 그리드에 가지고있는 캐릭터 정렬
    public void PopulateGrid()
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach(Character character in characterList)
        {
            GameObject slot = Instantiate(characterPrefab, content);
            CharacterSlotUI slotUI = slot.GetComponent<CharacterSlotUI>();

            slotUI.SetCharacter(character);
        }
    }
}
