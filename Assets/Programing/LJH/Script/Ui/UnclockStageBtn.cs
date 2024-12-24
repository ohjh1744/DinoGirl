using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnclockStageBtn : MonoBehaviour
{
    // 파이어 베이스 정보 불러와서 버튼 인터랙터블 조정 
    [SerializeField] Button[] buttons;
    [SerializeField] bool[] stageClears;
    private void OnEnable()
    {
        for (int i = 1; i < buttons.Length; i++) 
        {
            if (stageClears[i] == false) 
            {
                buttons[i].interactable = false;
            }
        }
    }
}
