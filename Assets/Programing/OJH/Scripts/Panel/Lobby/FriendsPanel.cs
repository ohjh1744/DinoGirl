using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FriendsPanel : UIBInder
{
    [SerializeField] private GameObject _friendList;

    [SerializeField] private Transform _content; // content �ڽ����� �ֱ� ����.

    [SerializeField] private ListObjectPull _pull;

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private List<GameObject> _infoLists;

    private UnityAction _friendExitClickHandler;
    private void Awake()
    {
        _infoLists = new List<GameObject>();
        BindAll();
    }

    private void OnEnable()
    {
        //Sound
        GetUI<Button>("FriendsExitButton").onClick.AddListener(_friendExitClickHandler = () => SoundManager.Instance.PlaySFX(_buttonClip));
        GetFriendData();
    }

    private void OnDisable()
    {
        Clear();
        GetUI<Button>("FriendsExitButton").onClick.RemoveListener(_friendExitClickHandler);
    }

    private void GetFriendData()
    {
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData");
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ID���� �������� SnapShot�� List �߰����Դ��� Ȯ��.
            var userIds = snapShot.Children.ToList();
            CheckSnapSHot(userIds);

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

    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    {
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null����!");
        }
    }

}
