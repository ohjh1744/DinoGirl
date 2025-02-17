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
using UnityEngine.Events;

public class MailPanel : UIBInder
{
    [SerializeField] private GameObject _mailList;

    [SerializeField] private Transform _content; // content �ڽ����� �ֱ� ����.

    [SerializeField] private AutoFalseSetter _checkedImage;

    [SerializeField] private ListObjectPull _pull;

    [SerializeField] private Sprite[] _itemSprites; // ���� �̹����� item�������

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;

    private List<MailList> _mailLists;

    private List<int>[] _imagePosPerItems; //���� ���� ������ ���� �˾����� ������ ��ġ�� �ٸ�.

    StringBuilder _sb;

    private UnityAction _mailExitClickHandler;

    private UnityAction _mailAllCheckHandler;

    private void Awake()
    {
        BindAll();
    }
    private void Start()
    {
        _mailLists = new List<MailList>();
        _sb = new StringBuilder();
        _imagePosPerItems = new List<int>[(int)E_Item.Length + 1];
        // ���⼭�� Length+1 �ؼ� index������ 1�� . 1 -> ���� ������ Ÿ���� 1���ۿ� ���ٴ� �� �ǹ�.
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
        //Sound
        GetUI<Button>("MailExitButton").onClick.AddListener(_mailExitClickHandler = () => SoundManager.Instance.PlaySFX(_buttonClip));
        GetUI<Button>("MailAllCheckButton").onClick.AddListener(_mailAllCheckHandler = () => SoundManager.Instance.PlaySFX(_buttonClip));

        GetUI<Button>("MailAllCheckButton").onClick.AddListener(CheckAllMail);
        GetMailData();
    }

    private void OnDisable()
    {
        GetUI<Button>("MailExitButton").onClick.RemoveListener(_mailExitClickHandler);
        GetUI<Button>("MailAllCheckButton").onClick.RemoveListener(_mailAllCheckHandler);

        GetUI<Button>("MailAllCheckButton").onClick.RemoveListener(CheckAllMail);
        GetUI("MailCheckedImage").gameObject.SetActive(false);
        ResetMailLists();
    }

    private void GetMailData()
    {

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("MailData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);
        root.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapShot = task.Result;

            // ���ϵ� �������� 
            var mails = snapShot.Children.ToList();

            if(mails == null || mails.Count == 0)
            {
                Debug.Log("mail�� �����ϴ�");
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

                // MailList Text ��¥, �̸�, ����������, ���� ����.
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
            Debug.Log("snapshot null����! �Ǵ� List�� 0����");
        }
    }

    // �ϰ�����
    private void CheckAllMail()
    {
        if (_mailLists.Count == 0)
        {
            Debug.Log("������ �����ϴ�.");
            return;
        }
        UpdateAllItem();
        DeleteAllMail();
        ResetMailLists();
    }

    private void UpdateAllItem()
    {

        int[] items = new int[(int)E_Item.Length];

        bool isAllChecked = true;

        //��� ���� Ȯ���ϰ� �����ϱ�
        foreach(MailList mailList in _mailLists)
        {
            // ���ϼ������� �̹� Ȯ���� maillist�� ����.
            if(mailList.gameObject.activeSelf == false)
            {
                continue;
            }
            isAllChecked = false;
            items[mailList.ItemType] += mailList.ItemNum;
        }

        if(isAllChecked == true)
        {
            ResetMailLists();
            Debug.Log("�̹�üũ�ؼ� �� false�Ǿ��ִ� ����!");
            return;
        }

        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items");
        Dictionary<string, object> dic = new Dictionary<string, object>();

        for (int i = 0; i < (int)E_Item.Length; i++)
        {
            //PlayerData�� ���� Update ���ֱ�
            int sum = PlayerDataManager.Instance.PlayerData.Items[i] + items[i];
            PlayerDataManager.Instance.PlayerData.SetItem(i, sum);

            //�鿣�忡�� Update���ֱ�
            dic[$"/{i}"] = PlayerDataManager.Instance.PlayerData.Items[i];
        }
        root.UpdateChildrenAsync(dic);
        Debug.Log("���������� PlayerData, �鿣�� �ʿ� Items ������");

        //�˾� �˶� ����
        //0.�˾��ʱ�ȭ
        ResetCheckImage();

        //1. �� ������ �������� �����ߴ��� Ȯ��.
        List<int> itemTypes = new List<int>();

        for(int i = 0; i < items.Length; i++)
        {
            if (items[i] > 0)
            {
                itemTypes.Add(i);
            }
        }

        //2.���������� ���� Image �� text ��ġ �ٸ��� �ϱ�
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

    //checkImage �ʱ�ȭ
    private void ResetCheckImage()
    {
        for(int i = 1; i <= (int)E_Item.Length; i++)
        {
            if(i == 3)
            {
                continue;
            }
            GetUI<Image>($"MailItemImage{i}").gameObject.SetActive(false);
            GetUI<TextMeshProUGUI>($"MailItemText{i}").text = "";
            GetUI<TextMeshProUGUI>($"MailItemText{i}").gameObject.SetActive(false);
        }
    }

    // ���� �� �������ֱ�
    private void DeleteAllMail()
    {
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("MailData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);
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
