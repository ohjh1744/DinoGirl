using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserList : MonoBehaviour
{
    [SerializeField] private string _otherId;

    public string OtherId { get { return _otherId; } set { _otherId = value; } }

    [SerializeField] private int _giveCoin;

    public void AddFriend()
    {
        //이미 친구인 경우 return
        foreach(string id in PlayerDataManager.Instance.PlayerData.FriendIds)
        {
            if(id == _otherId)
            {
                Debug.Log("이미 친구 추가 신청");
                return;
            }
        }

        //PlayerData List에 추가
        PlayerDataManager.Instance.PlayerData.FriendIds.Add(_otherId);

        //Firebase에 추가
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_friendIds");
        root.SetValueAsync(PlayerDataManager.Instance.PlayerData.FriendIds);


        Debug.Log("친구추가!");
        DecreaseCanFollow();
        GiveCoin();
    }

    //canFollow 변수 감소 후 Firebase에 Update
    private void DecreaseCanFollow()
    {
        PlayerDataManager.Instance.PlayerData.CanAddFriend--;
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_canAddFriend");
        root.SetValueAsync(PlayerDataManager.Instance.PlayerData.CanAddFriend);
    }

    // 친구 추가하면서 Coin 선물
    private void GiveCoin()
    {
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(_otherId).Child("_gift");

        root.RunTransaction(mutableData =>
        {
            //_gift 딕셔너리가 없는 경우 
            if(mutableData.Value == null)
            {
                Dictionary<string, object> newGift = new Dictionary<string, object>();
                newGift.Add(BackendManager.Auth.CurrentUser.UserId, _giveCoin);
                Debug.Log(newGift[BackendManager.Auth.CurrentUser.UserId]);
                mutableData.Value = newGift;
                return TransactionResult.Success(mutableData);
            }

            Dictionary<string, object> gift = mutableData.Value as Dictionary<string, object>;
            // Id가존재하다는 것은 기존에 팔로워가 준 선물을 안받은 상태.
            if (gift.ContainsKey(BackendManager.Auth.CurrentUser.UserId))
            {
                gift[BackendManager.Auth.CurrentUser.UserId] = (int)gift[BackendManager.Auth.CurrentUser.UserId] + _giveCoin;
            }
            else
            {
                gift[BackendManager.Auth.CurrentUser.UserId] = _giveCoin;
            }

            mutableData.Value = gift;
            Debug.Log("bbb");
            return TransactionResult.Success(mutableData);

        });
    }
}
