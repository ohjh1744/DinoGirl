using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPageBtn : MonoBehaviour
{
    [SerializeField] GameObject[] pages;
    [SerializeField] int num = 0;

    
    public void NextPageButton() 
    {
        for (int i = 0; i < pages.Length; i++) 
        {
            pages[i].SetActive(false);
        }
        pages[num].SetActive(true);
        num++;
        if (num == pages.Length) 
        {
            num = 0;
        }

    }
}
