using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이미 가진 캐릭터를 뽑았을 경우
// 아이템으로 반환하여 출력하기 위한 클래스
public class GachaReturn : MonoBehaviour
{
    private int itemId; // 반환할 아이템ID
    public int ItemId { get { return itemId; } set { itemId = value; } }
    private int count; // 반환할 개수
    public int Count { get { return count; } set { count = value; } }

    /// <summary>
    /// DB에 저장된 Return에 대한 정보를 저장하는 함수
    /// - ShopMakeStart.cs의 MakeCharReturnItemDic() 사용
    /// </summary>
    /// <param name="dataBaseList"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    public GachaReturn SetReturnInfo(Dictionary<int, Dictionary<string, string>> dataBaseList, int i)
    {
        GachaReturn gachaItemReturn = new GachaReturn();
        gachaItemReturn.ItemId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["ItemID"]);
        gachaItemReturn.Count = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Count"]);

        return gachaItemReturn;
    }


}
