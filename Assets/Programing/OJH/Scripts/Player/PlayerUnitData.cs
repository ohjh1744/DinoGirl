using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnitData 
{
    [SerializeField]private int _unitId;

    public int UnitId {  get { return _unitId; } set { _unitId = value; } }

    [SerializeField] private int _unitLevel;

    public int UnitLevel {get { return _unitLevel; } set { _unitLevel = value; } }

    [SerializeField] private string _type; //class 종류? 유형?

    public string Type { get { return _type; } set { _type = value; } }

    [SerializeField] private string _elementName;

    public string ElementName {  get { return _elementName;  } set { _elementName = value; } }

    [SerializeField] private int _hp;

    public int Hp { get { return _hp; } set { _hp = value; } }

    [SerializeField] private int _atk;

    public int Atk { get { return _atk; } set { _atk = value; } }

    [SerializeField] private int _def;

    public int Def { get { return _def; } set { _def = value; } }

    [SerializeField] private string _grid;

    public string Grid { get { return _grid; } set { _grid = value; } }

    [SerializeField] private int _statId;

    public int StatId {  get { return _statId; } set { _statId = value; } }

    [SerializeField] private int _percentIncrease;

    public int PercentIncrease {  get { return _percentIncrease;} set { _percentIncrease = value; } }


}
