using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MailPanel : UIBInder
{
    [SerializeField] private GameObject _mailList;

    [SerializeField] private Transform _content; // content 자식으로 넣기 위함.

    [SerializeField] private AutoFalseSetter _getCoinImage;

    private List<MailList> _mailLists;

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        _mailLists = new List<MailList>();
        GetUI<Button>("MailAllCheckButton").onClick.AddListener(CheckAllMail);

        GetGiftData();
    }


    private void GetGiftData()
    {
        Debug.Log("hi");
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
            CheckSnapSHot(userIds);

            List<string> ids = new List<string>(PlayerDataManager.Instance.PlayerData.Gift.Keys);
            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                string name = snapShot.Child(id).Child("_playerName").Value.ToString();
                int coin = int.Parse(PlayerDataManager.Instance.PlayerData.Gift[id].ToString());

                GameObject friendInfo = Instantiate(_mailList, _content);
                TextMeshProUGUI nameText = friendInfo.GetComponentInChildren<TextMeshProUGUI>();
                MailList mailList = friendInfo.GetComponentInChildren<MailList>();
                _mailLists.Add(mailList);
                mailList.OtherId = id;
                SetNameTag(name, coin, id, nameText);
            }

        });

    }
    private void SetNameTag(string name, int coin, string id, TextMeshProUGUI text)
    {
        StringBuilder nameSb = new StringBuilder();
        nameSb.Append(name);
        nameSb.Append("#");
        nameSb.Append(id.Substring(0, 4));
        nameSb.Append($" Give {coin}coins");
        text.SetText(nameSb);
    }


    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    {
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null값임!");
        }
    }

    // 일괄수령
    private void CheckAllMail()
    {
        UpdateAllCoin();
        UpdateGift();

        foreach(MailList mailList in _mailLists)
        {
            mailList.gameObject.SetActive(false);
        }
    }

    private void UpdateAllCoin()
    {
        // 모든 메일 수령 후 Coin합계 해서 계산.
        int getCoin = 0;
        int sum = 0;
        foreach (object value in PlayerDataManager.Instance.PlayerData.Gift.Values)
        {
            getCoin = TypeCastManager.Instance.TryParseInt(value.ToString());
            sum += getCoin;
        }

        sum += PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin];
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, sum);

        //Coin 백엔드 Update
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_items");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic[$"/{(int)E_Item.Coin}"] = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin];
        root.UpdateChildrenAsync(dic);
        Debug.Log("코인업뎃 성공");
    }

    private void UpdateGift()
    {

        //Gift 백엔드 Update
        PlayerDataManager.Instance.PlayerData.Gift.Clear();
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_gift");
        root.SetValueAsync(PlayerDataManager.Instance.PlayerData.Gift);
        Debug.Log("기프트업뎃 성공");
    }




}
