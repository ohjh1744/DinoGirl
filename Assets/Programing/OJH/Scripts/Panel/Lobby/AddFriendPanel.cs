using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddFriendPanel : MonoBehaviour
{
    [SerializeField] private GameObject _userList;

    [SerializeField] private Transform _content; // content 자식으로 넣기 위함.

    [SerializeField] private int _listNum; //LIst에 보이는 User 수

    private void Start()
    {
        GetUserData();
    }

    private void GetUserData()
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
            var userChildren = snapShot.Children.ToList();
            CheckSnapSHot(userChildren);

            // 랜덤으로 정렬.
            System.Random random = new System.Random();
            userChildren = userChildren.OrderBy(item => random.Next()).ToList();

            for (int i = 0; i < userChildren.Count; i++)
            {
                if(i == _listNum)
                {
                    break;
                }

                // 본인은 제외.
                if (userChildren[i].Key.ToString() == user.UserId)
                {
                    continue;
                }

                // 이미 친구면 제외.
                foreach(string friendId in PlayerDataManager.Instance.PlayerData.FriendIds)
                {
                    if(userChildren[i].Key.ToString() == friendId)
                    {
                        continue;
                    }
                }

                GameObject userInfo = Instantiate(_userList, _content);

                TextMeshProUGUI nameText = userInfo.GetComponentInChildren<TextMeshProUGUI>();

                string name = snapShot.Child(userChildren[i].Key.ToString()).Child("_playerName").Value.ToString();

                while (name == null) { }
   
                SetNameTag(name, userChildren[i].Key.ToString(), nameText);
            }



        });
    }

    private void SetNameTag(string name, string id, TextMeshProUGUI text)
    {
        StringBuilder nameSb = new StringBuilder();
        nameSb.Append(name);
        nameSb.Append("#");
        nameSb.Append(id.Substring(0, 4));
        text.SetText(nameSb);
    }

    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    { 
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null값임!");
        }
    }

    
}
