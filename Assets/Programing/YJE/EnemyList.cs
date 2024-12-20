using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyList : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemyList = new List<GameObject>();

    private void Awake()
    {
        for(int i = 0; i <gameObject.transform.childCount; i++)
        {
            enemyList.Add(gameObject.transform.GetChild(i).gameObject);
        }
    }
}
