using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_List {User, Friend };
public class ListObjectPull : MonoBehaviour
{
    [SerializeField] private  GameObject[] _lists;
    
    private List<GameObject>[] _pools;

    void Awake()
    {
        _pools = new List<GameObject>[_lists.Length];

        for (int i = 0; i < _pools.Length; i++)
        {
            _pools[i] = new List<GameObject>();
        }
    }

    public GameObject Get(int index, Transform parent)
    {
        GameObject select = null;

        foreach (GameObject list in _pools[index])
        {
            if (!list.activeSelf)
            {
                select = list;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(_lists[index], parent);
            _pools[index].Add(select);
        }

        return select;
    }
}
