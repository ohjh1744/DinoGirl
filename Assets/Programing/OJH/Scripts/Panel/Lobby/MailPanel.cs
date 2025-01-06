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

    [SerializeField] private AutoFalseSetter _checkedImage;

    [SerializeField] private ListObjectPull _pull;

    [SerializeField] private Sprite[] _itemSprites; // 보상 이미지들 item순서대로

    private List<MailList> _mailLists;

    private List<int>[] _imagePosPerItems; //보상 종류 개수에 따라 팝업내부 아이템 위치가 다름.

    StringBuilder _sb;

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        _mailLists = new List<MailList>();
        _sb = new StringBuilder();
        _imagePosPerItems = new List<int>[(int)E_Item.Length + 1];
        for (int i = 0; i < _imagePosPerItems.Length; i++)
        {
            _imagePosPerItems[i] = new List<int>();
        }
        _imagePosPerItems[1].Add(3);
        _imagePosPerItems[2].Add(2);
        _imagePosPerItems[2].Add(4);
        _imagePosPerItems[3].Add(2);
        _imagePosPerItems[3].Add(3);
        _imagePosPerItems[3].Add(4);
        _imagePosPerItems[4].Add(1);
        _imagePosPerItems[4].Add(2);
        _imagePosPerItems[4].Add(4);
        _imagePosPerItems[4].Add(5);
        _imagePosPerItems[5].Add(1);
        _imagePosPerItems[5].Add(2);
        _imagePosPerItems[5].Add(3);
        _imagePosPerItems[5].Add(4);
        _imagePosPerItems[5].Add(5);
    }

    private void OnEnable()
    {
        GetUI<Button>("MailAllCheckButton").onClick.AddListener(CheckAllMail);
        GetMailData();
    }

    private void OnDisable()
    {
        GetUI<Button>("MailAllCheckButton").onClick.RemoveListener(CheckAllMail);
        ResetMailLists();
    }

    private void GetMailData()
    {

        DatabaseReference root = BackendManager.Database.RootReference.Child("MailData").Child(BackendManager.Auth.CurrentUser.UserId);
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // 메일들 가져오기 
            var mails = snapShot.Children.ToList();

            if(mails == null || mails.Count == 0)
            {
                Debug.Log("mail이 없습니다");
                return;
            }
            CheckSnapSHot(mails);

            mails = mails.OrderByDescending(mail => mail.Key.ToString()).ToList();

            for(int i = 0; i < mails.Count; i++)
            {
                GameObject mailListObject = _pull.Get((int)E_List.Mail, _content);
                MailList mailList = mailListObject.GetComponent<MailList>();
                _mailLists.Add(mailList);
                mailList.MailTime = mails[i].Key.ToString();
                mailList.ItemType = TypeCastManager.Instance.TryParseInt(mails[i].Child("_itemType").Value.ToString());
                mailList.ItemNum = TypeCastManager.Instance.TryParseInt(mails[i].Child("_itemNum").Value.ToString());
                mailList.CheckedImage = _checkedImage;
                mailList.CheckedImageSprite = _itemSprites[mailList.ItemType];
                mailList.CheckedItemText = GetUI<TextMeshProUGUI>($"MailItemText3");
                mailList.CheckedItemImage = GetUI<Image>($"MailItemImage3");

                // MailList Text 날짜, 이름, 아이템종류, 개수 설정.
                StringBuilder sb = new StringBuilder();
                FindTime(mailList.MailTime, sb);
                sb.Append("  ");
                sb.Append(mails[i].Child("_name").Value.ToString());
                sb.Append("  ");
                sb.Append(((E_Item)mailList.ItemType).ToString());
                sb.Append("  ");
                sb.Append(mailList.ItemNum.ToString());

                mailList.MailText.SetText(sb);
            }
            
        });

    }

    private void FindTime(string time,StringBuilder sb)
    {
        // Time 
        string year = time.Substring(0, 4);
        string month = time.Substring(4, 2);
        string day = time.Substring(6, 2);
        string hour = time.Substring(9, 2);
        string minute = time.Substring(11, 2);

        string formattedTime = $"{year}/{month}/{day}/{hour}:{minute}";
        sb.Append(formattedTime);
        sb.Append("\n");
    }

    private void CheckSnapSHot(List<DataSnapshot> snapshotChildren)
    {
        while (snapshotChildren == null || snapshotChildren.Count == 0)
        {
            Debug.Log("snapshot null값임! 또는 List값 0개임");
        }
    }

    // 일괄수령
    private void CheckAllMail()
    {
        if (_mailLists.Count == 0)
        {
            Debug.Log("메일이 없습니다.");
            return;
        }
        UpdateAllItem();
        DeleteAllMail();
        ResetMailLists();
    }

    private void UpdateAllItem()
    {

        int[] items = new int[(int)E_Item.Length];

        //모든 보상 확인하고 저장하기
        foreach(MailList mailList in _mailLists)
        {
            // 단일수령으로 이미 확인한 maillist는 제외.
            if(mailList.gameObject.activeSelf == false)
            {
                continue;
            }
            items[mailList.ItemType] += mailList.ItemNum;
        }

        //PlayerData의 보상 Update 해주기
        for(int i = 0; i < (int)E_Item.Length; i++)
        {
            int sum = PlayerDataManager.Instance.PlayerData.Items[i] + items[i];
            PlayerDataManager.Instance.PlayerData.SetItem(i, sum);
        }

        //백엔드에도 Update해주기
        DatabaseReference root = BackendManager.Database.RootReference.Child("UserData").Child(BackendManager.Auth.CurrentUser.UserId).Child("_items");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        for (int i = 0; i < (int)E_Item.Length; i++)
        {
            dic[$"/{i}"] = PlayerDataManager.Instance.PlayerData.Items[i];
        }
        root.UpdateChildrenAsync(dic);
        Debug.Log("성공적으로 백엔드에도 items 업뎃됨");

        //팝업 알람 띄우기
        //0.팝업초기화
        ResetCheckImage();

        //1. 몇 종류의 아이템을 수령했는지 확인.
        List<int> itemTypes = new List<int>();

        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] > 0)
            {
                itemTypes.Add(i);
            }
        }

        //2.종류개수에 따라 Image 및 text 위치 다르게 하기
        for (int i = 0; i < itemTypes.Count; i++)
        {
            GetUI<Image>($"MailItemImage{_imagePosPerItems[itemTypes.Count][i]}").sprite = _itemSprites[itemTypes[i]];
            GetUI<Image>($"MailItemImage{_imagePosPerItems[itemTypes.Count][i]}").gameObject.SetActive(true);
            _sb.Clear();
            _sb.Append("+");
            _sb.Append(items[itemTypes[i]]);
            GetUI<TextMeshProUGUI>($"MailItemText{_imagePosPerItems[itemTypes.Count][i]}").SetText(_sb);
            GetUI<TextMeshProUGUI>($"MailItemText{_imagePosPerItems[itemTypes.Count][i]}").gameObject.SetActive(true);
        }

        _checkedImage.ResetCurrentTime();
        _checkedImage.gameObject.SetActive(true);

    }

    //checkImage 초기화
    private void ResetCheckImage()
    {
        for(int i = 1; i <= (int)E_Item.Length; i++)
        {
            if(i == 3)
            {
                continue;
            }
            GetUI<Image>($"MailItemImage{i}").gameObject.SetActive(false);
            GetUI<TextMeshProUGUI>($"MailItemText{i}").gameObject.SetActive(false);
        }
    }

    // 선물 다 제거해주기
    private void DeleteAllMail()
    {
        DatabaseReference root = BackendManager.Database.RootReference.Child("MailData").Child(BackendManager.Auth.CurrentUser.UserId);
        root.RemoveValueAsync();
    }

    private void ResetMailLists()
    {
        foreach (MailList mailList in _mailLists)
        {
            mailList.gameObject.SetActive(false);
        }
        _mailLists.Clear();
    }


}
