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
    /// Item의 증감에 따라 PlayerData를 갱신
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
        if (add) // 증가하는 경우
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] + amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] + amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] + amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] + amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] + amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                default:
                    break;
            }
        }
        else if (!add) // 감소하는 경우
        {
            switch (itemName)
            {
                case "Coin":
                    result = playerData.Items[(int)E_Item.Coin] - amount;
                    playerData.SetItem((int)E_Item.Coin, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] - amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] - amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] - amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] - amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                default:
                    break;
            }
        }
        
    }

    /// <summary>
    /// Character의 중복에 따라 PlayerData의 Units를 갱신하는 함수
    /// CharId를 통해서 확인하고 동일한 Character의 여부에 따라 알맞은 정보를 갱신
    //  - GachaBtn.cs에서 사용
    /// </summary>
    public void CheckCharId(List<GameObject> resultList, DatabaseReference root, PlayerData playerData)
    {
        // 전체 가챠 결과를 확인하면서 Item인지 Character인지 확인
        foreach (GameObject result in resultList) 
        {
            if (result.GetComponent<GachaItem>()) // 아이템인 경우
            {
                // 아이템 내용 갱신
                SendChangeValue(result.GetComponent<GachaItem>().ItemName,
                                result.GetComponent<GachaItem>().Amount, true,
                                root, playerData);
            }
            else if (result.GetComponent<GachaChar>()) // 캐릭터인 경우
            {
                bool isChecked = false; // 중복 : true, 미중복 : false

                foreach(PlayerUnitData unit in playerData.UnitDatas) // 모든 playerData의 UnitDatas를 확인하면서 중복여부 확인
                {
                    if(result.GetComponent<GachaChar>().CharId == unit.UnitId)
                    {
                        isChecked = true; // 중복
                    }
                }

                if (isChecked == false) // 소유 캐릭터 미중복
                {
                    // 새로운 Unit을 저장
                    PlayerUnitData newUnit = new PlayerUnitData();
                    newUnit.UnitId = result.GetComponent<GachaChar>().CharId;
                    newUnit.UnitLevel = 1;
                    playerData.UnitDatas.Add(newUnit);
                    // 실제 빌드 시 사용 - UserId불러오기 
                    DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");

                    // 모든 playerData.UnitDatas의 정보를 DB서버에 갱신
                    for (int num = 0; num < playerData.UnitDatas.Count; num++)
                    {
                        // nowData로 PlayerUnitData 생성
                        PlayerUnitData nowData = new PlayerUnitData();
                        nowData.UnitId = playerData.UnitDatas[num].UnitId;
                        nowData.UnitLevel = playerData.UnitDatas[num].UnitLevel;

                        // 지정된 위치에 순서대로 서버에 저장
                        unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                        unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                    }
                }
                else // 소유 캐릭터 중복
                {
                    // 중복 시 아이템으로 변환하여 환산
                    GameObject resultItem = gachaSceneController.CharReturnItem(result.GetComponent<GachaChar>().CharId, result);
                    // 변동 값 서버에 저장
                    SendChangeValue(resultItem.GetComponent<GachaItem>().ItemName,
                                    resultItem.GetComponent<GachaItem>().Amount, true,
                                    root, playerData);
                }
            }
        }
    }
}
