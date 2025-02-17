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

public class DataLoader : MonoBehaviour
{

    [SerializeField] private string _uID;

    [ContextMenu("LoadTest")]
    public void Test()
    {
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(_uID);

        Debug.Log(root);

        root.KeepSynced(true);
  
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            string json = snapShot.GetRawJsonValue();
            Debug.Log(json);
            PlayerDataManager.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(json);

        });

    }

    //Snapshot�� ����� �ҷ��������� üũ�ϴ� �Լ� -> snapshot�� �ҷ������µ� �����ð��� �ణ �ִ°����� ������ ��.
    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    {
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null����!");
        }

        for(int i = 0; i < snapshotChildren.Count; i++)
        {
            Debug.Log(snapshotChildren[i].Key.ToString());
        }
    }
}
