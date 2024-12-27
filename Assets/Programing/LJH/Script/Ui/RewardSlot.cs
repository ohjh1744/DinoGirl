using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Image itemImage;

    public void setRewardData(string texts/*, Image image*/)
    {
        text.text = texts;
        //itemImage = image;
    }
   
}
