using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaItem : MonoBehaviour
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
    /// </summary>
    /// <param name="dataBaseList"></param>
    /// <param name="result"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public GachaItem MakeItemList(Dictionary<int, Dictionary<string, string>> dataBaseList, GachaItem result, int index)
    {
        switch (index)
        {
            case 500:
                result.ItemId = index;
                result.ItemName = dataBaseList[index]["ItemName"];
                result.ItemImage = Resources.Load<Sprite>("Lottery/TestG");
                break;
            case 501:
                result.ItemId = index;
                result.ItemName = dataBaseList[index]["ItemName"];
                result.ItemImage = Resources.Load<Sprite>("Lottery/TestDB");
                break;
            case 502:
                result.ItemId = index;
                result.ItemName = dataBaseList[index]["ItemName"];
                result.ItemImage = Resources.Load<Sprite>("Lottery/TestBC");
                break;
            case 503:
                result.ItemId = index;
                result.ItemName = dataBaseList[index]["ItemName"];
                result.ItemImage = Resources.Load<Sprite>("Lottery/TestDS");
                break;
            case 504:
                result.ItemId = index;
                result.ItemName = dataBaseList[index]["ItemName"];
                result.ItemImage = Resources.Load<Sprite>("Lottery/TestS");
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
    public GameObject SetGachaItemUI(GachaItem gachaItem, GameObject resultUI)
    {
        resultUI.gameObject.GetComponent<GachaItem>().ItemId = gachaItem.ItemId;
        resultUI.gameObject.GetComponent<GachaItem>().ItemName = gachaItem.ItemName;
        resultUI.gameObject.GetComponent<GachaItem>().Amount = gachaItem.Amount;

        // 알맞은 UI 출력
        resultUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaItem.ItemImage;
        resultUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gachaItem.ItemName;
        resultUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gachaItem.Amount.ToString();
        return resultUI;
    }

    /// <summary>
    /// 이미 소유한 Character를 뽑은 경우
    /// 전환되는 아이템을 보여주는 UI Setting 함수
    //  - GachaSceneController.cs에서 사용
    /// </summary>
    /// <param name="gachaItem"></param>
    /// <param name="resultUI"></param>
    /// <returns></returns>
    public GameObject SetGachaReturnItemUI(GachaItem gachaItem, GameObject resultUI)
    {
        resultUI.gameObject.GetComponent<GachaItem>().ItemId = gachaItem.ItemId;
        resultUI.gameObject.GetComponent<GachaItem>().Amount = gachaItem.Amount;

        // 알맞은 UI 출력
        resultUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaItem.ItemImage;
        resultUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gachaItem.Amount.ToString();
        return resultUI;
    }

}
