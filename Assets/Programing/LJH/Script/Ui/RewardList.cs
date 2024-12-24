using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardList : MonoBehaviour
{
    [SerializeField] GameObject RewardSlot;


    private void OnEnable()
    { 
        StartCoroutine(delaying());
    }

    IEnumerator delaying() 
    {
        yield return new WaitForSeconds(0.1f);
        int count = 0;
        for (int i = 0; i < BattleSceneManager.Instance.curItemCounts.Count; i++)
        {
            if (BattleSceneManager.Instance.curItemCounts[i] != "") 
            {   
                count++;
                GameObject obj = Instantiate(RewardSlot);
                RectTransform pos = obj.GetComponent<RectTransform>();
                obj.transform.SetParent(transform);
                pos.anchoredPosition =  new Vector2(1000f, 140f-100*count);
                obj.GetComponent<RewardSlot>().setText(BattleSceneManager.Instance.curItemCounts[i]);
            }
            
        }
    }
  
}
