using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;

public class MailPanel : MonoBehaviour
{
    [SerializeField] private GameObject _mailList;

    [SerializeField] private Transform _content; // content 자식으로 넣기 위함.


    private void Start()
    {
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

            Debug.Log(userIds.Count);
            List<string> ids = new List<string>(PlayerDataManager.Instance.PlayerData.Gift.Keys);

            Debug.Log(ids.Count);

            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                string name = snapShot.Child(id).Child("_playerName").Value.ToString();
                int coin = int.Parse(PlayerDataManager.Instance.PlayerData.Gift[id].ToString());

                GameObject friendInfo = Instantiate(_mailList, _content);
                TextMeshProUGUI nameText = friendInfo.GetComponentInChildren<TextMeshProUGUI>();
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


}
