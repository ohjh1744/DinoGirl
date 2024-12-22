using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList = new List<GameObject>();
    public bool isChanged = false;
    private void Awake()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            enemyList.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        isChanged = false;
    }
    /*
        private void Update()
        {
            // 리스트의 개수가 자식오브젝트의 개수와 다르면(변동이있는경우면)
            if(enemyList.Count != gameObject.transform.childCount)
            {
                enemyList.Clear(); // 리스트 변경
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    enemyList.Add(gameObject.transform.GetChild(i).gameObject);
                }
                isChanged = true;
            }
            else
            {
                isChanged = false;
            }
        }
    */
    /// <summary>
    /// 자식의 수가 변경될 때마다 호출되는 함수
    /// </summary>
    private void OnTransformChildrenChanged()
    {

        if (transform.childCount == 0)
        {
            Debug.Log("적 전멸");
        }
        else if (transform.childCount >= 1)
        {
            enemyList.Clear(); // 리스트 변경
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                enemyList.Add(gameObject.transform.GetChild(i).gameObject);
            }
            isChanged = true;
        }

    }
}
