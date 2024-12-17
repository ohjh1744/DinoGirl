using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Billboard : MonoBehaviour, IComparable<Billboard>
{
    public static List<Billboard> billboards = new List<Billboard>();
    public static bool sorted;
    [SerializeField] SpriteRenderer[] render;
    [SerializeField] int order;
    private void Awake()
    {
        if (render != null)
            return;
        else if (render.Length != 0)
            return;
        render = GetComponentsInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        billboards.Add(this);
    }
    private void OnDisable()
    {
        billboards.Remove(this);
    }
    private void Update()
    {
        if (sorted == true)
            return;
        billboards.Sort();
        for (int i = 0; i < billboards.Count; i++)
        {
            billboards[i].order = i;
        }
        sorted = true;
    }
    private void LateUpdate()
    {
        BillBoard();
        sorted = false;
    }
    private void BillBoard()
    {
        float yAngle = Camera.main.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yAngle, 0);
        for (int i = 0; i < render.Length; i++)
        {
            render[i].sortingOrder = (order * 1000 + i);
        }
    }
    public int CompareTo(Billboard other)
    {
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float xDistance = plane.GetDistanceToPoint(transform.position);
        float yDistance = plane.GetDistanceToPoint(other.transform.position);
        if (xDistance == yDistance)
            return 0;
        return xDistance > yDistance ? -1 : 1;
    }
}