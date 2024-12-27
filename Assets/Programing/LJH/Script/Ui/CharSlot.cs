using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Level;
    [SerializeField] Image gridimage;
    [SerializeField] Image charimage;

    public void setCharSlotData(string name , string level /*Image gridImage*/, Sprite charImage) 
    {
        Name.text = name;
        Level.text = level;
        //gridimage = gridImage;
        charimage.sprite = charImage;
    }
}
