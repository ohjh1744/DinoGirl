using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingSlot : MonoBehaviour
{
    [SerializeField] TMP_Text ScoreTxt;
    [SerializeField] TMP_Text nameTxt;



    public void setRankingData(string name, string score ) 
    {
        
        nameTxt.text = name;
        ScoreTxt.text = score;
    }
}
