using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase.Database;
using System.Linq;
public class OthersInfoPanel : UIBInder
{
    [SerializeField] private GameObject _othersInfoImage;

    [SerializeField] private Dictionary<string, int> _othersDinoStoneCounts;

    private void Awake()
    {
        BindAll();
    }

    private void GetOthersData()
    {

        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");

        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

          
        });
    }

    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    { 
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null°ªÀÓ!");
        }
    }

    
}
