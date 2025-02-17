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
using UnityEngine.Events;
using UnityEngine.UI;

public class AddFriendPanel : UIBInder
{
    [SerializeField] private GameObject _userList;

    [SerializeField] private Transform _content; // content �ڽ����� �ֱ� ����.

    [SerializeField] private int _listNum; //LIst�� ���̴� User ��

    [SerializeField] private AutoFalseSetter _getCoinImage; // userList�� �������� Image, ģ���߰� �� ��� �˾�

    [SerializeField] private AutoFalseSetter _cantAddImage; //userList�� �������� Image, ģ���߰� Ƚ�� �̹� �Ѿ����� ��� �˾�

    [SerializeField] private AutoFalseSetter _maxFriendImage; //userList�� �������� Image, �ִ�ģ���� ��� ��� �˾�

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private bool _isFriend;

    private int _curUserListNum;

    private UnityAction _addFriendExitClickHandler;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        //Sound
        GetUI<Button>("AddFriendExitButton").onClick.AddListener(_addFriendExitClickHandler = () => SoundManager.Instance.PlaySFX(_buttonClip));
    }
    private void Start()
    {
        GetUserData();
    }

    private void OnDisable()
    {
        //Sound
        GetUI<Button>("AddFriendExitButton").onClick.RemoveListener(_addFriendExitClickHandler);

        GetUI("AddFriendGetCoinImage").gameObject.SetActive(false);
        GetUI("AddFriendCantAddImage").gameObject.SetActive(false);
        GetUI("AddFriendMaxFriendImage").gameObject.SetActive(false);
    }

    private void GetUserData()
    {
        FirebaseUser user = BackendManager.Instance.Auth.CurrentUser;

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData");

        root.KeepSynced(true);

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ID���� �������� SnapShot�� List �߰����Դ��� Ȯ��.
            var userChildren = snapShot.Children.ToList();
            CheckSnapSHot(userChildren);

            // �������� ����.
            System.Random random = new System.Random();
            userChildren = userChildren.OrderBy(item => random.Next()).ToList();

            for (int i = 0; i < userChildren.Count; i++)
            {
                // ���� User �˻��� �ʱ�ȭ 
                _isFriend = false;

                if(_curUserListNum == _listNum)
                {
                    break;
                }

                // ������ ����.
                if (userChildren[i].Key.ToString() == user.UserId)
                {
                    continue;
                }

                // �̹� ģ���� ����.
                foreach(string friendId in PlayerDataManager.Instance.PlayerData.FriendIds)
                {
                    if(userChildren[i].Key.ToString() == friendId)
                    {
                        _isFriend = true;
                        break;
                    }
                }

                if(_isFriend == true)
                {
                    continue;
                }

                _curUserListNum++;

                GameObject userInfo = Instantiate(_userList, _content);

                UserList userList = userInfo.GetComponent<UserList>();

                userList.OtherId = userChildren[i].Key.ToString();

                userList.GetCoinImage = _getCoinImage;

                userList.CantAddImage = _cantAddImage;

                userList.MaxFriendImage = _maxFriendImage;

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
            Debug.Log("snapshot null����!");
        }
    }

    
}
