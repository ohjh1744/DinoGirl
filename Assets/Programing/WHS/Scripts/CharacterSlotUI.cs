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

    public void SetCharacter(Character newCharacter)
    {
        character = newCharacter;
        iconImage.sprite = character.image;
        nameText.text = $"{character.name}";
        levelText.text = $"{character.level}";
    }

    public void OnClick()
    {
        Debug.Log($"{character.name}");
    }
}
