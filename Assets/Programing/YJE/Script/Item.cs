using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    private int itemId;
    public int ItemId { get { return itemId; } set { itemId = value; } }
    private string itemName;
    public string ItemName { get { return itemName; } set { itemName = value; } }
    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    private Sprite itemImage;
    public Sprite ItemImage { get { return itemImage; } set { itemImage = value; } }

    /// <summary>
    /// ItemList를 Dictionary로 제작할 때 사용
    /// Item의 종류가 추가되는 경우 switch문에 추가하여 사용 가능
    // - GachaSceneController.cs의 MakeItemList()에서 참조하여 사용
    // - 각 이미지 파일은 Resources.Load<Sprite>("경로/파일이름")으로 각자 지정 필요
    /// </summary>
    /// <param name="dataBaseList"></param>
    /// <param name="result"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Item MakeItemList(Dictionary<int, Dictionary<string, string>> dataBaseList, Item result, int index)
    {
        switch (index)
        {
            case 500:
                result.itemId = index;
                result.itemName = dataBaseList[index]["ItemName"];
                result.itemImage = Resources.Load<Sprite>("ShopTest/Gold");
                break;
            case 501:
                result.itemId = index;
                result.itemName = dataBaseList[index]["ItemName"];
                result.itemImage = Resources.Load<Sprite>("ShopTest/DinoBlood");
                break;
            case 502:
                result.itemId = index;
                result.itemName = dataBaseList[index]["ItemName"];
                result.itemImage = Resources.Load<Sprite>("ShopTest/BoneCrystal");
                break;
            case 503:
                result.itemId = index;
                result.itemName = dataBaseList[index]["ItemName"];
                result.itemImage = Resources.Load<Sprite>("ShopTest/DinoStone");
                break;
            case 504:
                result.itemId = index;
                result.itemName = dataBaseList[index]["ItemName"];
                result.itemImage = Resources.Load<Sprite>("ShopTest/Stone");
                break;
        }
        return result;
    }

    /// <summary>
    /// GachaItem의 정보를 ResultPanel/Panel 아래에 새로 만들어진 프리팹UI로 셋팅하는 함수
    // - GachaSceneController.cs에서 사용
    /// </summary>
    /// <param name="gachaItem"></param>
    /// <param name="resultUI"></param>
    /// <returns></returns>
    public GameObject SetGachaItemUI(Item gachaItem, GameObject resultUI)
    {
    resultUI.GetComponent<Item>().itemId = gachaItem.itemId;
    resultUI.GetComponent<Item>().itemName = gachaItem.itemName;
    resultUI.GetComponent<Item>().amount = gachaItem.amount;

        // 알맞은 UI 출력
        resultUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaItem.itemImage;
        resultUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gachaItem.itemName;
        resultUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = gachaItem.amount.ToString();
        return resultUI;
    }

    /// <summary>
    /// 이미 소유한 Character를 뽑은 경우
    /// 전환되는 아이템을 보여주는 UI Setting 함수
    /// </summary>
    /// <param name="gachaItem"></param>
    /// <param name="resultUI"></param>
    /// <returns></returns>
    public GameObject SetGachaReturnItemUI(Item gachaItem, GameObject resultUI)
    {
        resultUI.GetComponent<Item>().ItemId = gachaItem.ItemId;
        resultUI.GetComponent<Item>().Amount = gachaItem.Amount;

        // 알맞은 UI 출력
        resultUI.transform.GetChild(1).GetComponent<Image>().sprite = gachaItem.ItemImage;
        resultUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gachaItem.Amount.ToString();
        return resultUI;
    }
}
