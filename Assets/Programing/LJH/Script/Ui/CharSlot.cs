using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    [SerializeField] public int charId;
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Level;
    [SerializeField] Image gridimage;
    [SerializeField] Image charimage;
    

    public void setCharSlotData(int id ,string name , string level /*Image gridImage*/, Sprite charImage) 
    {   
        charId = id;
        Name.text = name;
        Level.text = level;
        //gridimage = gridImage;
        charimage.sprite = charImage;
    }
}
