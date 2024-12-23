using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum E_Money {one, two, three, four, Five, Length }

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    [SerializeField] private string _playerName;

    public string PlayerName { get { return _playerName; } set { _playerName = value; } }

    [SerializeField] private string _playerId;

    public string PlayerId { get { return _playerId; }  set { _playerId = value; } }

    [SerializeField] private int[] _money;

    public int[] Money { get { return _money; } private set { } }

    [SerializeField] private List<PlayerUnitData> _unitDatas;

    public List<PlayerUnitData> UnitDatas { get { return _unitDatas; } private set { } }


}
