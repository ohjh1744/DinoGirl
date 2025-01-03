using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 빌드 시 BackendManager.Auth.CurrentUser.UserId 주석 해제하고 테스트 코드 주석처리 필수

public class GachaCheck : MonoBehaviour
{
    GachaSceneController gachaSceneController;

    private void Awake()
    {
        gachaSceneController = gameObject.GetComponent<GachaSceneController>();
    }

    /// <summary>
    /// itemName에 따라 switch문으로 분기하여 알맞은 root위치에 playerData를 갱신하는 함수
    /// - add : false 이면 감소 / ture이면 증가
    //  - GachaBtn.cs에서 사용
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="amount"></param>
    /// <param name="root"></param>
    /// <param name="playerData"></param>
    public void SendChangeValue(string itemName, int amount, bool add, DatabaseReference root, PlayerData playerData)
    {
        int result = 0;
        DatabaseReference setItemRoot;
        if (add)
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] + amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] + amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    // Test용
                     setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] + amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] + amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] + amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                default:
                    break;
            }
        }
        else if (!add)
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] - amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] - amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] - amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] - amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] - amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    // setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    // Test용
                    setItemRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                default:
                    break;
            }
        }
        
    }

    /// <summary>
    /// Character의 중복을 CharId를 통해서 확인하고 동일한 Character의 여부에 따라 알맞은 정보를 갱신
    //  - GachaBtn.cs에서 사용
    /// </summary>
    public void CheckCharId(List<GameObject> resultList, DatabaseReference root, PlayerData playerData)
    {
        for (int i = 0; i < resultList.Count; i++)
        {
            if (resultList[i].GetComponent<GachaItem>()) // GachaItem이 존재하는 Item인 경우
            {
                SendChangeValue(resultList[i].gameObject.GetComponent<GachaItem>().ItemName,
                                           resultList[i].gameObject.GetComponent<GachaItem>().Amount, true,
                                           root, playerData);
            }
            else if (resultList[i].GetComponent<GachaChar>()) // GachaChar가 존재하는 캐릭터인 경우
            {

                int index = -1;

                for (int j = 0; j < playerData.UnitDatas.Count; j++)
                {
                    if (resultList[i].GetComponent<GachaChar>().CharId == playerData.UnitDatas[j].UnitId)
                    {
                        index = j;
                    }
                }
                // PlayerData의 UnitDatas에 동일한 캐릭터 아이디가 있는지 여부를 확인
                if (index == -1)
                {
                    Debug.Log("없는 캐릭터");
                    // 새로운 Unit을 저장
                    PlayerUnitData newUnit = new PlayerUnitData();
                    newUnit.UnitId = resultList[i].GetComponent<GachaChar>().CharId;
                    newUnit.UnitLevel = 1;
                    playerData.UnitDatas.Add(newUnit);
                    // 실제 빌드 시 사용 - UserId불러오기 
                    // DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");
                    // Test 용
                     DatabaseReference unitRoot = root.Child("CHSmbrwghYNzZb7AIkdLRtvpHaW2").Child("_unitDatas");

                    for (int num = 0; num < playerData.UnitDatas.Count; num++)
                    {
                        PlayerUnitData nowData = new PlayerUnitData();
                        nowData.UnitId = playerData.UnitDatas[num].UnitId;
                        nowData.UnitLevel = playerData.UnitDatas[num].UnitLevel;
                        unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                        unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                    }
                }
                else
                {
                    Debug.Log("이미 소유한 캐릭터");
                    GameObject resultItem = gachaSceneController.CharReturnItem(resultList[i].gameObject.GetComponent<GachaChar>().CharId, resultList[i].gameObject);
                    SendChangeValue(resultItem.gameObject.GetComponent<GachaItem>().ItemName,
                                               resultItem.gameObject.GetComponent<GachaItem>().Amount, true,
                                               root, playerData);
                }
            }

        }
    }
}
