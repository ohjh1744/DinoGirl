using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;    // 캐릭터 이름 텍스트
    public TextMeshProUGUI levelText;   // 캐릭터 이름 텍스트
    public Image iconImage;             // 캐릭터 아이콘 이미지

    private Character character;

    // 인벤토리에 캐릭터 세팅 - 이미지, 이름, 레벨 ( 레어도, 포지션 등 )
    public void SetCharacter(Character newCharacter)
    {
        character = newCharacter;
        iconImage.sprite = character.image;
        nameText.text = $"{character.Name}";
        levelText.text = $"{character.level}";
    }

    // 클릭 시 ( 캐릭터 정보 출력, 추가 UI )
    public void OnClick()
    {
        Debug.Log($"{character.Name}");
        Debug.Log($"{character.ID}");
    }
}
