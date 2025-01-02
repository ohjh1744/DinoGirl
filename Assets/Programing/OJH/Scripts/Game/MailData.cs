using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MailData 
{
    [SerializeField]private string _name; // ¿Ã∏ß+ID

    public string Name {get{ return _name;} set { _name = value; } }

    [SerializeField] private int _itemType;
    public int ItemType { get { return _itemType; } set { _itemType = value; } }

    [SerializeField] private int _itemNum;

    public int ItemNum { get { return _itemNum; } set { _itemNum = value; } }


}
