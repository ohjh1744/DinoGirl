using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class FriendsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _friendList;

    [SerializeField] private Transform _content; // content 자식으로 넣기 위함.

    [SerializeField] private ListObjectPull _pull;

    private List<GameObject> _infoLists;

    private void Awake()
    {
        _infoLists = new List<GameObject>();
    }
    private void OnEnable()
    {
        GetFriendData();
    }

    private void OnDisable()
    {
        Clear();
    }

    private void GetFriendData()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ID값들 가져오고 SnapShot의 List 잘가져왔는지 확인.
            var userIds = snapShot.Children.ToList();

            for (int i = 0; i < PlayerDataManager.Instance.PlayerData.FriendIds.Count; i++)
            {
                string friendId = PlayerDataManager.Instance.PlayerData.FriendIds[i];
                string name = snapShot.Child(friendId).Child("_playerName").Value.ToString();

                GameObject friendInfo = _pull.Get((int)E_List.Friend, _content);
                _infoLists.Add(friendInfo);
                TextMeshProUGUI nameText = friendInfo.GetComponentInChildren<TextMeshProUGUI>();
                FriendList friendList = friendInfo.GetComponent<FriendList>();
                friendList.FriendId = friendId;
                SetNameTag(name, friendId, nameText);
            }

        });

    }

    private void Clear()
    {
        for(int i = 0; i < _infoLists.Count; i++)
        {
            _infoLists[i].SetActive(false);
        }
        _infoLists.Clear();
    }

    private void SetNameTag(string name, string id, TextMeshProUGUI text)
    {
        StringBuilder nameSb = new StringBuilder();
        nameSb.Append(name);
        nameSb.Append("#");
        nameSb.Append(id.Substring(0, 4));
        text.SetText(nameSb);
    }

}
