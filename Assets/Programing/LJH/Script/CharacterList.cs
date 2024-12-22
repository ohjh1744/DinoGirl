using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList : MonoBehaviour
{   // 캐릭터 목록 생성 및 정보 넣기 
    
    [SerializeField] string[] charaList; // 임시 데이터 , 나중에 캐릭터 정보를 받아와야 함 
    [SerializeField] GameObject slot; // 프리팹
    
    
    private void OnEnable()
    {
        for (int i = 0; i < charaList.Length; i++) 
        {
            GameObject obj =  Instantiate(slot, new Vector3(100+150 * i,125, 0),Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.GetComponent<TestSlot>().setTexttest(charaList[i]);
           
            
        }
    }
}
