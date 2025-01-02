using Firebase.Database;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaCheck : MonoBehaviour
{
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
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] + amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                     setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    // Test용
                    // setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] + amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                     setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    // Test용
                    // setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] + amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    // Test용
                    // setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] + amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    // Test용
                    // setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/4");
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
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/0");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/0");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoBlood":
                    result = playerData.Items[(int)E_Item.DinoBlood] - amount;
                    playerData.SetItem((int)E_Item.DinoBlood, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/1");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/1");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "BoneCrystal":
                    result = playerData.Items[(int)E_Item.BoneCrystal] - amount;
                    playerData.SetItem((int)E_Item.BoneCrystal, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/2");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/2");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "DinoStone":
                    result = playerData.Items[(int)E_Item.DinoStone] - amount;
                    playerData.SetItem((int)E_Item.DinoStone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/3");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                case "Stone":
                    result = playerData.Items[(int)E_Item.Stone] - amount;
                    playerData.SetItem((int)E_Item.Stone, result);
                    // 실제 빌드 시 사용 - UserId불러오기
                    setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                    // Test용
                    //setItemRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/4");
                    setItemRoot.SetValueAsync(result); // firebase 값 변경
                    break;
                default:
                    break;
            }
        }
        
    }
}
