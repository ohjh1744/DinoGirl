using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO : 
// 1. Firebase에 저장된 플레이어 데이터를 확인하고
//    - 다이노 스톤이 부족한 경우 뽑기 진행 x
//    - 뽑기를 진행하는 경우 변경된 다이노 스톤 재화를 Firebase에 전송
// 2. 뽑기 결과를 정리하여 Firebase에 변경된 값을 전송
//    - Item의 경우 기존 플레이어의 데이터에 더한 값으로 전송
//    - 캐릭터의 경우 기존 플레이어가 가지고 있는지 확인
//      1. 플레이어가 가지고 있는 경우 아이템으로 반환하여 전송
//      2. 플레이어가 가지고 있지 않은 경우 플레이어 데이터의 유닛에 추가로 전송

/// <summary>
/// GachaScene의 전체적인 관리를 하는 스크립트
/// - CsvDataManager와 연결
/// - PlayData와 연결
/// - UIBInder를 사용하여 이벤트 선언 후 알맞게 이벤트로 각 UI의 활성화 설정
/// </summary>
public class GachaSceneController : UIBInder
{
    GachaBtn gachaBtn;

    // csvDataManager.cs에서 가져올 특정 DataList를 받을 Disctionary
    Dictionary<int, Dictionary<string, string>> dataBaseList = new Dictionary<int, Dictionary<string, string>>();

    public Dictionary<int, GachaItem> ItemDictionary = new Dictionary<int, GachaItem>();
    public Dictionary<int, GachaChar> CharDictionary = new Dictionary<int, GachaChar>();
    public Dictionary<int, GachaItemReturn> CharReturnItemDic = new Dictionary<int, GachaItemReturn>();

    [Header("Gacha Lists")]
    public List<Gacha> baseGachaList = new List<Gacha>();
    public List<Gacha> eventGachaList = new List<Gacha>();

    [Header("UI")]
    [SerializeField] RectTransform singleResultContent; // 1연차 결과 내역 프리팹이 생성 될 위치
    [SerializeField] RectTransform tenResultContent; // 10연차 결과 내역 프리팹이 생성 될 위치
    [SerializeField] GameObject resultCharPrefab; // 결과가 캐릭터인 경우 사용할 프리팹
    [SerializeField] GameObject resultItemPrefab; // 결과가 아이템인 경우 사용할 프리팹
    [SerializeField] RectTransform returnContent; // 중복캐릭터 아이템 반환 프리팹이 생성 될 위치
    [SerializeField] GameObject returnPrefab; // 중복캐릭터 아이템 반환 프리팹

    private void Awake()
    {
        gachaBtn = gameObject.GetComponent<GachaBtn>();
        BindAll();
        SettingStartPanel();
    }
    /// <summary>
    /// 시작 시 버튼의 문구 설정
    /// - 버튼의 문구 변경 가능
    //  - LoadingCheck.cs에서 이벤트로 사용
    /// </summary>
    public void SettingStartUI()
    {
        // 각 Button 텍스트 설정
        GetUI<TextMeshProUGUI>("BaseSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("BaseTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("EventSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("EventTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("ChangeBaseGacahText").SetText("상설");
        GetUI<TextMeshProUGUI>("ChangeEventGacahText").SetText("이벤트");
        UpdatePlayerUI();
    }
    /// <summary>
    /// 각 Item 재화 상단 표시
    /// - 변동 시 계속 업데이트가 필요하므로 함수로 제작하여 사용
    /// </summary>
    public void UpdatePlayerUI()
    {
        GetUI<TextMeshProUGUI>("CoinText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin].ToString());
        GetUI<TextMeshProUGUI>("DinoBloodText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood].ToString());
        GetUI<TextMeshProUGUI>("BoneCrystalText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal].ToString());
        GetUI<TextMeshProUGUI>("DinoStoneText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone].ToString());
        GetUI<TextMeshProUGUI>("StoneText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone].ToString());
    }
    /// <summary>
    /// 시작 시 패널 활성화와 비활성화 설정
    /// </summary>
    private void SettingStartPanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(true);
        GetUI<Image>("EventGachaPanel").gameObject.SetActive(false);
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(true);
        GetUI<Image>("ChangeEventGachaBtn").gameObject.SetActive(true);
        GetUI<Image>("ShopCharacter").gameObject.SetActive(false);
    }

    /// <summary>
    /// csv데이터로 알맞은 가차 리스트를 분리하는 함수
    //  - LoadingCheck.cs에서 이벤트로 사용
    /// - 새로운 가챠 내용을 리스트를 추가하려는 경우
    ///     1. csv 파일에 GachaGroup을 묶어서 내용 수정
    ///     2. LoadingCheck 스크립트 앞에 GachaGroup의 종류만큼 리스트 선언
    ///     2. 함수의 switch문에 새로운 case로 GachaGroup 분기점 제작
    ///     3. 각 GachaGroup별 리스트 초기화
    /// </summary>
    public void MakeGachaList()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Gacha]; // csv데이터로 가챠리스트 가져오기
        for (int i = 1; i < dataBaseList.Count; i++)
        {
            Debug.Log(dataBaseList[i]["Check"]);
            Gacha gachatem = new Gacha();
            gachatem.Check = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Check"]);
            switch (gachatem.Check) // 종류를 확인
            {
                case 0: // 종류가 Character인 경우
                    gachatem.CharId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["CharID"]);
                    break;
                case 1: // 종류가 Item인 경우
                    gachatem.ItemId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["ItemID"]);
                    break;
                default:
                    break;
            }
            gachatem.Probability = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Probability"]); // 확률 저장
            gachatem.Count = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Count"]); // 반환 갯수 저장

            switch (dataBaseList[i]["GachaGroup"]) // GachaGroup을 확인하여 List에 저장
            {
                case "1":
                    baseGachaList.Add(gachatem);
                    break;
                case "2":
                    eventGachaList.Add(gachatem);
                    break;
                default:
                    break;
            }

        }
    }

    /// <summary>
    /// DB에서 받아온 Item을 GachaItem 형식의 리스트에 사용할 수 있는 형태로 저장
    /// - GachaBtn.cs 에서 아이템을 반환할 때 UI로 연동시켜서 제작하기 위해 사용
    //  - LoadingCheck.cs에서 이벤트로 설정
    /// </summary>
    public void MakeItemDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Item];
        GachaItem gold = new GachaItem();
        gold = gold.MakeItemList(dataBaseList, gold, 500);
        ItemDictionary.Add(gold.ItemId, gold);

        GachaItem dinoBlood = new GachaItem();
        dinoBlood = dinoBlood.MakeItemList(dataBaseList, dinoBlood, 501);
        ItemDictionary.Add(dinoBlood.ItemId, dinoBlood);

        GachaItem boneCrystal = new GachaItem();
        boneCrystal = boneCrystal.MakeItemList(dataBaseList, boneCrystal, 502);
        ItemDictionary.Add(boneCrystal.ItemId, boneCrystal);

        GachaItem dinoStone = new GachaItem();
        dinoStone = dinoStone.MakeItemList(dataBaseList, dinoStone, 503);
        ItemDictionary.Add(dinoStone.ItemId, dinoStone);

        GachaItem stone = new GachaItem();
        stone = stone.MakeItemList(dataBaseList, stone, 504);
        ItemDictionary.Add(stone.ItemId, stone);

    }

    /// <summary>
    /// DB에서 받아온 Character를 GachaChar 형식의 리스트에서 사용할 수 있는 형태로 저장
    /// - GachaBtn.cs 에서 아이템을 반환할 때 UI로 연동시켜서 제작하기 위해 사용
    /// - 캐릭터 목록 추가 시 추가 필요
    //  - LoadingCheck.cs에서 이벤트로 설정
    /// </summary>
    public void MakeCharDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Character];
        GachaChar tricia = new GachaChar();
        tricia = tricia.MakeCharList(dataBaseList, tricia, 1);
        CharDictionary.Add(tricia.CharId, tricia);
        GachaChar celes = new GachaChar();
        celes = celes.MakeCharList(dataBaseList, celes, 2);
        CharDictionary.Add(celes.CharId, celes);
        GachaChar regina = new GachaChar();
        regina = regina.MakeCharList(dataBaseList, regina, 3);
        CharDictionary.Add(regina.CharId, regina);
        GachaChar spinne = new GachaChar();
        spinne = spinne.MakeCharList(dataBaseList, spinne, 4);
        CharDictionary.Add(spinne.CharId, spinne);
        GachaChar aila = new GachaChar();
        aila = aila.MakeCharList(dataBaseList, aila, 5);
        CharDictionary.Add(aila.CharId, aila);
        GachaChar quezna = new GachaChar();
        quezna = quezna.MakeCharList(dataBaseList, quezna, 6);
        CharDictionary.Add(quezna.CharId, quezna);
        GachaChar uloro = new GachaChar();
        uloro = uloro.MakeCharList(dataBaseList, uloro, 7);
        CharDictionary.Add(uloro.CharId, uloro);
    }
    /// <summary>
    /// DB에서 받아온 GachaReturn를 GachaItemReturn 형식의 리스트에서 사용할 수 있는 형태로 저장
    /// - GachaBtn.cs 에서 중복캐릭터를 아이템으로 반환할 때 사용
    //  - LoadingCheck.cs에서 이벤트로 설정
    /// </summary>
    public void MakeCharReturnItemDic()
    {
        dataBaseList = CsvDataManager.Instance.DataLists[(int)E_CsvData.GachaReturn];
        for (int i = 1; i <= dataBaseList.Count; i++)
        {
            GachaItemReturn gachaItemReturn = new GachaItemReturn();
            gachaItemReturn.ItemId = TypeCastManager.Instance.TryParseInt(dataBaseList[i]["ItemID"]);
            gachaItemReturn.Count= TypeCastManager.Instance.TryParseInt(dataBaseList[i]["Count"]);
            CharReturnItemDic.Add(i, gachaItemReturn);
        }
    }

    /// <summary>
    /// UI버튼세팅
    //  - LoadingCheck.cs에서 이벤트로 사용
    /// </summary>
    public void SettingBtn()
    {
        // 결과패널 버튼 클릭 시 패널 비활성화 함수 연결
        GetUI<Button>("SingleResultPanel").onClick.AddListener(DisabledGachaResultPanel);
        GetUI<Button>("TenResultPanel").onClick.AddListener(DisabledGachaResultPanel);
        // GachaBtn 스크립트의 각 버튼별 함수 연결
        GetUI<Button>("BaseSingleBtn").onClick.AddListener(gachaBtn.BaseSingleBtn);
        GetUI<Button>("BaseTenBtn").onClick.AddListener(gachaBtn.BaseTenBtn);
        GetUI<Button>("EventSingleBtn").onClick.AddListener(gachaBtn.EventSingleBtn);
        GetUI<Button>("EventTenBtn").onClick.AddListener(gachaBtn.EventTenBtn);
        // Gacha 종류 변경 버튼 함수 연동
        GetUI<Button>("ChangeBaseGachaBtn").onClick.AddListener(ShowBaseGachaPanel);
        GetUI<Button>("ChangeEventGachaBtn").onClick.AddListener(ShowEventGachaPanel);
    }

    /// <summary>
    /// BaseGachaPanel 활성화
    /// - EventGachaPanel 비활성화
    /// ChangeBaseGachaBtn에 설정
    /// </summary>
    private void ShowBaseGachaPanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(true);
        GetUI<Image>("EventGachaPanel").gameObject.SetActive(false);
    }
    /// <summary>
    /// EventGachaPanel 활성화
    /// - BaseGachaPanel 비활성화
    /// ChangeEventGachaBtn에 설정
    /// </summary>
    private void ShowEventGachaPanel()
    {
        GetUI<Image>("EventGachaPanel").gameObject.SetActive(true);
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(false);
    }

    /// <summary>
    /// Single/TenResult Panel 활성화 시 비활성화 하는 UI
    /// </summary>
    private void ShowResultPanelDisable()
    {
        // 기본 뽑기 종류 변경 버튼 비활성화
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(false);
        GetUI<Image>("ChangeEventGachaBtn").gameObject.SetActive(false);
        // 각 아이템 재화 Text 비활성화
        GetUI<TextMeshProUGUI>("CoinText").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("DinoBloodText").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("BoneCrystalText").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("DinoStoneText").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("StoneText").gameObject.SetActive(false);
    }

    /// <summary>
    /// SingleResultPanel 활성화
    //  - GachaBtn.cs에서 사용
    /// </summary>
    public void ShowSingleResultPanel()
    {
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(true);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(true);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(false);
        ShowResultPanelDisable(); // 비활성화 함수
    }
    /// <summary>
    /// TenResultPanel 활성화
    //  - GachaBtn.cs에서 사용
    /// </summary>
    public void ShowTenResultPanel()
    {
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(true);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(false);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(true);
        ShowResultPanelDisable(); // 비활성화 함수
    }


    /// <summary>
    /// GachaResultPanel 비활성화
    /// - 결과 저장 리스트를 초기화
    /// - 결과 패널을 비활성화
    // - GachaBtn.cs에서도 사용
    /// </summary>
    public void DisabledGachaResultPanel()
    {
        gachaBtn.ClearResultList(); // 뽑기의 결과는 GachaBtn 스크립트에 저장되어있으로 초기화 필수
        // 기본 뽑기 종류 변경 버튼 활성화
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(true);
        GetUI<Image>("ChangeEventGachaBtn").gameObject.SetActive(true);
        // 결과 패널 비활성화
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(false);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(false);
        // 각 아이템 재화 Text 활성화
        GetUI<TextMeshProUGUI>("CoinText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("DinoBloodText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("BoneCrystalText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("DinoStoneText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("StoneText").gameObject.SetActive(true);
    }

    /// <summary>
    /// 1회 뽑기 실행 시
    /// GachaList와 index값을 받아서 해당하는 결과가 아이템/캐릭터인지 판단
    /// 분류에 따른 Prefab으로 GameObject를 생성
    /// 알맞은 결과를 UI로 출력
    /// GameObject로 반환하는 함수
    //  - GachaBtn.cs에서 사용
    /// </summary>
    /// <param name="GachaList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GachaSingleResultUI(List<Gacha> GachaList, int index)
    {
        switch (GachaList[index].Check)
        {
            case 0: // 반환이 캐릭터인 경우
                GachaChar resultChar = CharDictionary[GachaList[index].CharId];
                resultChar.Amount = GachaList[index].Count;
                GameObject resultCharUI = Instantiate(resultCharPrefab, singleResultContent);
                resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                return resultCharUI;
            case 1: // 반환이 아이템인 경우
                GachaItem result = ItemDictionary[GachaList[index].ItemId]; // GachaItem 설정
                result.Amount = GachaList[index].Count; // GachaItem의 Amount를 정해진 수량으로 설정

                GameObject resultUI = Instantiate(resultItemPrefab, singleResultContent); // Prefab으로 정해진 위치에 생성 - 한개
                resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem을 적용한 UI Setting
                return resultUI;
            default:
                return null;
        }
    }

    /// <summary>
    /// 10회 뽑기 실행 시
    /// GachaList와 index값을 받아서 해당하는 결과가 아이템/캐릭터인지 판단
    /// 분류에 따른 Prefab으로 GameObject를 생성
    /// 알맞은 결과를 UI로 출력
    /// GameObject로 반환하는 함수
    //  - GachaBtn.cs에서 사용
    /// </summary>
    /// <param name="GachaList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GachaTenResultUI(List<Gacha> GachaList, int index)
    {
        switch (GachaList[index].Check)
        {
            case 0:
                // TODO : 반환이 캐릭터인 경우
                GachaChar resultChar = CharDictionary[GachaList[index].CharId];
                resultChar.Amount = GachaList[index].Count;
                GameObject resultCharUI = Instantiate(resultCharPrefab, tenResultContent);
                resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                return resultCharUI;
            case 1: // 반환이 아이템인 경우
                GachaItem result = ItemDictionary[GachaList[index].ItemId]; // GachaItem 설정
                result.Amount = GachaList[index].Count; // GachaItem의 Amount를 정해진 수량으로 설정

                GameObject resultUI = Instantiate(resultItemPrefab, tenResultContent); // Prefab으로 정해진 위치에 생성 - 열개
                resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem을 적용한 UI Setting

                return resultUI;
            default:
                return null;
        }
    }

    /// <summary>
    /// 이미 뽑아서 제작한 resultList의 캐릭터 프리팹 오브젝트 resultListObj
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="resultListObj"></param>
    public GameObject CharReturnItem(int UnitId, GameObject resultListObj)
    {
        returnContent = resultListObj.transform.GetChild(3).GetComponent<RectTransform>();
        GameObject resultObjUI = Instantiate(returnPrefab, returnContent); // 그 위치에 새로운 프리팹으로 생성

        GachaItem resultItem = resultObjUI.gameObject.GetComponent<GachaItem>();
        resultItem.ItemId = CharReturnItemDic[UnitId].ItemId;
        resultItem.Amount = CharReturnItemDic[UnitId].Count;
        resultItem.ItemName = ItemDictionary[CharReturnItemDic[UnitId].ItemId].ItemName;
        resultItem.ItemImage = ItemDictionary[CharReturnItemDic[UnitId].ItemId].ItemImage;

        resultObjUI = resultItem.SetGachaReturnItemUI(resultItem, resultObjUI);

        return resultObjUI;
    }
}
