using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void setText(string texts)
    {
        text.text = texts;
    }
}
