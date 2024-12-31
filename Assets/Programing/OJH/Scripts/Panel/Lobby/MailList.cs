using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MailList : MonoBehaviour
{
    [SerializeField] private string _otherId;

    public string OtherId { get { return _otherId; } set { _otherId = value; } }

    public void CheckMail()
    {
        UpdateCoin();
        UpdateGift();

        gameObject.SetActive(false);
    }

    private void UpdateCoin()
    {
        //PlayerData Coin 수 Update.
        int getCoin = TypeCastManager.Instance.TryParseInt(PlayerDataManager.Instance.PlayerData.Gift[_otherId].ToString());
        int sum = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin] + getCoin;
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, sum);

        //Backend에서 Coin 수  변경
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_items");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic[$"/{(int)E_Item.Coin}"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin];
        root.UpdateChildrenAsync(dic);
        Debug.Log("코인업뎃 성공");

    }

    private void UpdateGift()
    {

        //PlayerData에서 gift Update
        PlayerDataManager.Instance.PlayerData.Gift.Remove(_otherId);

        //Backend에서 gift도 변경
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_gift");
        root.SetValueAsync(PlayerDataManager.Instance.PlayerData.Gift);
        Debug.Log("기프트업뎃 성공");
    }
}
