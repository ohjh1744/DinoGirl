using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// LoadingManager.cs에 연동하여 사용하는
/// ShopScene의 시작 세팅
/// </summary>
public class ShopMakeStart : MonoBehaviour
{
    ShopSceneController shopSceneController;

    // csvDataManager.cs에서 가져올 특정 DataList를 받을 Disctionary
    private Dictionary<int, Dictionary<string, string>> dataBaseList = new Dictionary<int, Dictionary<string, string>>();
    private Dictionary<int, Item> itemDic = new Dictionary<int, Item>(); // 아이템 Dictionary
    public Dictionary<int, Item> ItemDic { get { return itemDic; } set { itemDic = value; } } 
    private Dictionary<int, ShopChar> charDic = new Dictionary<int, ShopChar>(); // 캐릭터 Dictionary
    public Dictionary<int, ShopChar> CharDic { get { return charDic; } set { charDic = value; }  }
    private Dictionary<int, GachaReturn> charReturnItemDic = new Dictionary<int, GachaReturn>(); // 중복 캐릭터 반환 아이템 Dictionary
    public Dictionary<int, GachaReturn> CharReturnItemDic { get { return charReturnItemDic; } set { charReturnItemDic = value; } }
    private List<Gacha> baseGachaList = new List<Gacha>(); // 기본 뽑기 List
    public List<Gacha> BaseGachaList { get { return baseGachaList; } set { baseGachaList = value; } }

    private RectTransform characterContent;
    private GameObject shopCharPrefab;
    private void Awake()
    {
        shopSceneController = gameObject.GetComponent<ShopSceneController>();
        shopCharPrefab = Resources.Load<GameObject>("Prefabs/ShopListChar");
    }


    /// <summary>
    /// csv데이터로 알맞은 가차 리스트를 분리하는 함수
    /// - 새로운 가챠 내용을 리스트를 추가하려는 경우
    ///     1. csv 파일에 GachaGroup을 묶어서 내용 수정
    ///     2. LoadingCheck 스크립트 앞에 GachaGroup의 종류만큼 리스트 선언
    ///     2. 함수의 switch문에 새로운 case로 GachaGroup 분기점 제작
    ///     3. 각 GachaGroup별 리스트 초기화
    /// </summary>
    public void MakeGachaList()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Gacha]; // csv데이터로 가챠리스트 가져오기

        for (int i = 1; i <= dataBaseList.Count; i++) // dataBaseList를 전부 확인하면서
        {
            Gacha gacha = new Gacha();
            gacha.Check = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Check"]);
            switch (gacha.Check) // 종류를 확인하여 id 저장
            {
                case 0: // 종류가 Character인 경우
                    gacha.CharId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["CharID"]);
                    break;
                case 1: // 종류가 Item인 경우
                    gacha.ItemId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["ItemID"]);
                    break;
                default:
                    break;
            }
            gacha.Probability = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Probability"]); // 확률 저장
            gacha.Count = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Count"]); // 반환 갯수 저장

            // GachaGroup을 확인하여 List에 저장 - 이벤트 가챠를 추가하고 싶은 경우 GachaGroup을 생성하여 분기하고 새로운 뽑기 리스트를 생성하여 사용
            switch (dataBaseList[i]["GachaGroup"])
            {
                case "1":
                    baseGachaList.Add(gacha);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// DB에서 받아온 Item을 Item 형식의 리스트에 사용할 수 있는 형태로 할당하여 itemDic 완성
    /// - Item의 종류 추가시 내용을 수정해야하고 각 ItemId를 설정하여 사용해야하며 GachaItem.cs의 MakeItemList함수 분기 추가가 필요함
    /// </summary>
    public void MakeItemDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Item];
        Item gold = new Item();
        gold = gold.MakeItemList(dataBaseList, gold, 500);
        itemDic.Add(gold.ItemId, gold);

        Item dinoBlood = new Item();
        dinoBlood = dinoBlood.MakeItemList(dataBaseList, dinoBlood, 501);
        itemDic.Add(dinoBlood.ItemId, dinoBlood);

        Item boneCrystal = new Item();
        boneCrystal = boneCrystal.MakeItemList(dataBaseList, boneCrystal, 502);
        itemDic.Add(boneCrystal.ItemId, boneCrystal);

        Item dinoStone = new Item();
        dinoStone = dinoStone.MakeItemList(dataBaseList, dinoStone, 503);
        itemDic.Add(dinoStone.ItemId, dinoStone);

        Item stone = new Item();
        stone = stone.MakeItemList(dataBaseList, stone, 504);
        itemDic.Add(stone.ItemId, stone);
    }

    /// <summary>
    ///  DB에서 받아온 Character를 ShopChar 형식의 리스트에서 사용할 수 있는 형태로 저장
    /// - Character의 종류 추가시 내용을 수정해야하고 각 CharId를 설정하여 사용해야하며 ShopChar.cs의 MakeCharList함수 분기 추가가 필요함
    /// - ShopChar.cs의 SetCharInfo()를 사용하여 새로 만든 ShopChar에 정보를 저장
    /// </summary>
    public void MakeCharDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        ShopChar tricia = new ShopChar();
        tricia = tricia.SetCharInfo(dataBaseList, 1);
        charDic.Add(tricia.CharId, tricia);
        ShopChar celes = new ShopChar();
        celes = celes.SetCharInfo(dataBaseList, 2);
        charDic.Add(celes.CharId, celes);
        ShopChar regina = new ShopChar();
        regina = regina.SetCharInfo(dataBaseList, 3);
        charDic.Add(regina.CharId, regina);
        ShopChar spinne = new ShopChar();
        spinne = spinne.SetCharInfo(dataBaseList, 4);
        charDic.Add(spinne.CharId, spinne);
        ShopChar aila = new ShopChar();
        aila = aila.SetCharInfo(dataBaseList, 5);
        charDic.Add(aila.CharId, aila);
        ShopChar quezna = new ShopChar();
        quezna = quezna.SetCharInfo(dataBaseList, 6);
        charDic.Add(quezna.CharId, quezna);
        ShopChar uloro = new ShopChar();
        uloro = uloro.SetCharInfo(dataBaseList, 7);
        charDic.Add(uloro.CharId, uloro);
        ShopChar eost = new ShopChar();
        eost = eost.SetCharInfo(dataBaseList, 8);
        charDic.Add(eost.CharId, eost);
        ShopChar melorin = new ShopChar();
        melorin = melorin.SetCharInfo(dataBaseList, 9);
        charDic.Add(melorin.CharId, melorin);
    }

    /// <summary>
    /// DB에서 받아온 GachaReturn를 GachaItemReturn 형식의 리스트에서 사용할 수 있는 형태로 저장
    /// - GachaReturn.cs의 SetReturnInfo()를 사용하여 새로 만든 GachaReturn에 정보를 저장
    /// </summary>
    public void MakeCharReturnItemDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.GachaReturn];
        for (int i = 1; i <= dataBaseList.Count; i++)
        {
            GachaReturn gachaItemReturn = new GachaReturn();
            gachaItemReturn = gachaItemReturn.SetReturnInfo(dataBaseList, i);
            charReturnItemDic.Add(i, gachaItemReturn);
        }
    }


    /// <summary>
    /// charDic을 이용하여 상점에 구매목록에 캐릭터를 띄우기 위한 오브젝트 제작
    /// </summary>
    public void ShopCharMaker()
    {
        characterContent = shopSceneController.GetUI<RectTransform>("CharacterContent");

        for (int i = 1; i <= charDic.Count; i++)
        {
            GameObject shopCharUI = Instantiate(shopCharPrefab, characterContent);
            ShopChar shopChar = shopCharUI.GetComponent<ShopChar>();
            charDic.TryGetValue(i, out shopChar);

            shopCharUI = shopChar.SetShopCharInfo(shopChar, shopCharUI);
        }
    }
}
