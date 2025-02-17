using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MailList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mailText;

    public TextMeshProUGUI MailText {  get { return _mailText; } set { _mailText = value; } }

    [SerializeField] private string _mailTime;

    public string MailTime { get { return _mailTime; } set { _mailTime = value; } }

    [SerializeField] private int _itemType;

    public int ItemType { get { return _itemType; } set { _itemType = value; } }

    [SerializeField] private int _itemNum;

    public int ItemNum {  get { return _itemNum; } set { _itemNum = value; } }

    [SerializeField] private AutoFalseSetter _checkedImage;

    public AutoFalseSetter CheckedImage { get { return _checkedImage; } set { _checkedImage = value; } }

    [SerializeField] private Sprite _checkedImageSprite;
    public Sprite CheckedImageSprite { get { return _checkedImageSprite; } set { _checkedImageSprite = value; } }

    [SerializeField] private Image _checkcedItemImage;
    public Image CheckedItemImage { get { return _checkcedItemImage; } set { _checkcedItemImage = value; } }

    [SerializeField] private TextMeshProUGUI _checkcedItemText;

    public TextMeshProUGUI CheckedItemText { get { return _checkcedItemText; } set { _checkcedItemText = value; } }

    //ButtonSound
    [SerializeField] private AudioClip _buttonClip;


    public void CheckMail()
    {
        SoundManager.Instance.PlaySFX(_buttonClip);
        UpdateItem();
        DeleteMail();
        ShowCheckedImage();

        gameObject.SetActive(false);
    }

    // ���� ������Ʈ
    private void UpdateItem()
    {
        //PlayerData Coin �� Update.
        int sum = PlayerDataManager.Instance.PlayerData.Items[_itemType] + _itemNum;
        PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Coin, sum);

        //�鿣�忡�� ����
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData").Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items");
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic[$"/{_itemType}"] = PlayerDataManager.Instance.PlayerData.Items[_itemType];
        root.UpdateChildrenAsync(dic);
    }

    //���ɹ��� ���� ����
    private void DeleteMail()
    {
        DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("MailData").Child(BackendManager.Instance.Auth.CurrentUser.UserId);
        root.Child(_mailTime).RemoveValueAsync();
    }

    private void ShowCheckedImage()
    {
        _checkcedItemImage.sprite = _checkedImageSprite;
        _checkcedItemText.text = $"+{ItemNum}";
        _checkedImage.ResetCurrentTime();
        _checkedImage.gameObject.SetActive(true);
    }

}
