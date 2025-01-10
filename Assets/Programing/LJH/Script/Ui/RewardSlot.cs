using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Sprite[] itemImage;
    [SerializeField] Image Image;

    public void setRewardData(int id ,string texts)
    {   
        
        text.text = texts;
        Image.sprite = itemImage[id - 500];
    }
   
}
