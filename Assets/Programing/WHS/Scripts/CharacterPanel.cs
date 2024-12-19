using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPanel : UIBInder
{
    private void Awake()
    {
        BindAll();
    }

    public void DisplayCharacterInfo(Character character)
    {
        GetUI<TextMeshProUGUI>("NameText").text = character.Name;
    }
}
