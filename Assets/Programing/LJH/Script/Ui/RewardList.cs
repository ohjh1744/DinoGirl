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
        foreach (int item in BattleSceneManager.Instance.curItemValues.Keys)
        {   
            count++;
            if (BattleSceneManager.Instance.curItemValues[item] != 0) 
            {
                GameObject obj = Instantiate(RewardSlot);
                RectTransform pos = obj.GetComponent<RectTransform>();
                obj.transform.SetParent(transform);
                pos.anchoredPosition = new Vector2(1000f, 240f - 100 * count);
                obj.GetComponent<RewardSlot>().setText(BattleSceneManager.Instance.curItemValues[item].ToString());
            }
            
        }
    }

}
