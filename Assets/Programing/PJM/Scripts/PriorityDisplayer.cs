using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityDisplayer : MonoBehaviour
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] private bool checker;
    
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        checker = GetComponentInParent<BaseUnitController>().IsPriorityTargetFar;
        if (checker)
        {
            renderer.color = Color.red;
        }
        else
        {
            renderer.color = Color.blue;
        }
    }
}
