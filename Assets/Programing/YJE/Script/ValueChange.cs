using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChange : MonoBehaviour
{
    ShopBtnManager shopBtnManager;

    private void Awake()
    {
        shopBtnManager = gameObject.GetComponent<ShopBtnManager>();
    }

    /// <summary>
    /// Item�� ������ ���� PlayerData�� ����
    /// itemName�� ���� switch������ �б��Ͽ� �˸��� root��ġ�� playerData�� �����ϴ� �Լ�
    /// - add : false �̸� ���� / ture�̸� ����
    //  - GachaBtn.cs���� ���
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="amount"></param>
    /// <param name="root"></param>
    /// <param name="playerData"></param>
    public void SendChangeValue(string itemName, int amount, bool add, DatabaseReference root, PlayerData playerData)
    {
        int result = 0;
        DatabaseReference setItemRoot;
        if (add) // �����ϴ� ���
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] + amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] + amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] + amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] + amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] + amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                default:
                    break;
            }
        }
        else if (!add) // �����ϴ� ���
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] - amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] - amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] - amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] - amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] - amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // ���� ���� �� ��� - UserId�ҷ�����
                    setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase �� ����
                    break;
                default:
                    break;
            }
        }

    }

    /// <summary>
    /// Character�� �ߺ��� ���� PlayerData�� Units�� �����ϴ� �Լ�
    /// CharId�� ���ؼ� Ȯ���ϰ� ������ Character�� ���ο� ���� �˸��� ������ ����
    //  - GachaBtn.cs���� ���
    /// </summary>
    public void CheckCharId(List<GameObject> resultList, DatabaseReference root, PlayerData playerData)
    {
        // ��ü ��í ����� Ȯ���ϸ鼭 Item���� Character���� Ȯ��
        foreach (GameObject result in resultList)
        {
            if (result.GetComponent<Item>()) // �������� ���
            {
                // ������ ���� ����
                SendChangeValue(result.GetComponent<Item>().ItemName,
                                result.GetComponent<Item>().Amount, true,
                                root, playerData);
            }
            else if (result.GetComponent<ShopChar>()) // ĳ������ ���
            {
                bool isChecked = false; // �ߺ� : true, ���ߺ� : false

                foreach (PlayerUnitData unit in playerData.UnitDatas) // ��� playerData�� UnitDatas�� Ȯ���ϸ鼭 �ߺ����� Ȯ��
                {
                    if (result.GetComponent<ShopChar>().CharId == unit.UnitId)
                    {
                        isChecked = true; // �ߺ�
                    }
                }

                if (isChecked == false) // ���� ĳ���� ���ߺ�
                {
                    // ���ο� Unit�� ����
                    PlayerUnitData newUnit = new PlayerUnitData();
                    newUnit.UnitId = result.GetComponent<ShopChar>().CharId;
                    newUnit.UnitLevel = 1;
                    playerData.UnitDatas.Add(newUnit);
                    // ���� ���� �� ��� - UserId�ҷ����� 
                    DatabaseReference unitRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_unitDatas");

                    // ��� playerData.UnitDatas�� ������ DB������ ����
                    for (int num = 0; num < playerData.UnitDatas.Count; num++)
                    {
                        // nowData�� PlayerUnitData ����
                        PlayerUnitData nowData = new PlayerUnitData();
                        nowData.UnitId = playerData.UnitDatas[num].UnitId;
                        nowData.UnitLevel = playerData.UnitDatas[num].UnitLevel;

                        // ������ ��ġ�� ������� ������ ����
                        unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                        unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                    }
                }
                else // ���� ĳ���� �ߺ�
                {
                    // �ߺ� �� ���������� ��ȯ�Ͽ� ȯ��
                    GameObject resultItem = shopBtnManager.CharReturnItem(result.GetComponent<ShopChar>().CharId, result);
                    // ���� �� ������ ����
                    SendChangeValue(resultItem.GetComponent<Item>().ItemName,
                                    resultItem.GetComponent<Item>().Amount, true,
                                    root, playerData);
                }
            }
        }
    }
}
