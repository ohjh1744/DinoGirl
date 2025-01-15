using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum E_Item { Coin, DinoBlood, BoneCrystal, DinoStone, Stone, Length }
// 여러 컨텐츠쪽에서 직접적으로 바로 필요한 것들만 최소한으로 저장. 
[System.Serializable]
public class PlayerData 
{
    [SerializeField] private string _playerName;

    public string PlayerName { get { return _playerName; } set { _playerName = value; } }

    [SerializeField] private int _mainUnitID;

    public int MainUnitID { get { return _mainUnitID; } set { _mainUnitID = value; } }

    [SerializeField] private string _lastResetAddFriendTime; // 최근접속시간

    public string LastResetAddFriendTime { get { return _lastResetAddFriendTime; } set { _lastResetAddFriendTime = value; } }


    [SerializeField] private string _roomExitTime; // 나만의방에서 나간시간 -> 추후 방치형보상쪽에서 사용할 예정.

    public string RoomExitTime { get { return _roomExitTime; } set { _roomExitTime = value; } }

    [SerializeField] private int _canAddFriend;

    public int CanAddFriend { get { return _canAddFriend; } set { _canAddFriend = value; } }

    [SerializeField] private int[] _items;

    public int[] Items { get { return _items; } private set {} }

    public void SetItem(int index, int value)
    {
        if(index >= 0 && index < _items.Length)
        {
            _items[index] = value;
            OnItemChanged[index]?.Invoke(value);
        }
    }

    [SerializeField] private int[] _storedItems;

    public int[] StoredItems { get { return _storedItems; } private set { } }

    public void SetStoredItem(int index, int value)
    {
        if (index >= 0 && index < _storedItems.Length)
        {
            _storedItems[index] = value;
            OnStoredItemChanged[index]?.Invoke(value);
        }
    }

    [SerializeField] private bool[] _isStageClear;   //스테이지 클리어 여부

    public bool[] IsStageClear { get { return _isStageClear; } private set { } }

    [SerializeField] private List<string> _friendIds;

    public List<string> FriendIds { get { return _friendIds; } private set { } }

    [SerializeField] private List<PlayerUnitData> _unitDatas;

    public List<PlayerUnitData> UnitDatas { get { return _unitDatas; } private set { } }

    public UnityAction<int>[] OnItemChanged = new UnityAction<int>[(int)E_Item.Length];

    public UnityAction<int>[] OnStoredItemChanged = new UnityAction<int>[(int)E_Item.Length];
}
