using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendList : MonoBehaviour
{
    [SerializeField] private string _friendId;

    public string FriendId { get { return _friendId; } set { _friendId = value; } }

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    public void DeleteFriend()
    {
        SoundManager.Instance.PlaySFX(_buttonClip);

        PlayerDataManager.Instance.PlayerData.FriendIds.Remove(_friendId);

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);

        root.Child("_friendIds").SetValueAsync(PlayerDataManager.Instance.PlayerData.FriendIds);

        gameObject.SetActive(false);
    }
}
