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

public class AddFriendPanel : UIBInder
{
    [SerializeField] private GameObject _userList;

    [SerializeField] private Transform _content; // content 자식으로 넣기 위함.

    [SerializeField] private int _listNum; //LIst에 보이는 User 수

    [SerializeField] private AutoFalseSetter _getCoinImage; // userList에 참조해줄 Image, 친구추가 시 띄울 팝업

    [SerializeField] private AutoFalseSetter _cantAddImage; //userList에 참조해줄 Image, 친구추가 횟수 이미 넘었따면 띄울 팝업

    [SerializeField] private AutoFalseSetter _maxFriendImage; //userList에 참조해줄 Image, 최대친구인 경우 띄울 팝업

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private bool _isFriend;

    private int _curUserListNum;

    private void Awake()
    {
        BindAll();
    }

    private void OnEnable()
    {
        //Sound
        GetUI<Button>("AddFriendExitButton").onClick.AddListener(() => SoundManager.Instance.PlaySFX(_buttonClip));
    }
    private void Start()
    {
        GetUserData();
    }

    private void OnDisable()
    {
        //Sound
        GetUI<Button>("AddFriendExitButton").onClick.RemoveListener(() => SoundManager.Instance.PlaySFX(_buttonClip));

        GetUI("AddFriendGetCoinImage").gameObject.SetActive(false);
        GetUI("AddFriendCantAddImage").gameObject.SetActive(false);
        GetUI("AddFriendMaxFriendImage").gameObject.SetActive(false);
    }

    private void GetUserData()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");

        root.KeepSynced(true);

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
                // 다음 User 검색시 초기화 
                _isFriend = false;

                if(_curUserListNum == _listNum)
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
            Debug.Log("snapshot null값임!");
        }
    }

    
}
