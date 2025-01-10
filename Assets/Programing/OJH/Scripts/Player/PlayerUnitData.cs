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

}
