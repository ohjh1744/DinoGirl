using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestSlot : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void setTexttest(string texts) 
    {
        text.text = texts; 
    }
}
