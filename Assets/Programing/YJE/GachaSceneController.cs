using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GachaScene의 전체적인 관리를 하는 스크립트
/// - UIBInder를 사용하여 이벤트 선언 후 알맞게 이벤트로 각 UI의 활성화 설정
/// </summary>
public class GachaSceneController : UIBInder
{
    private bool isLoading = false;
    public bool IsLoading { get { return isLoading; } set { isLoading = value; } }
    // csvDataManager.cs에서 가져올 특정 DataList를 받을 Disctionary
    Dictionary<int, Dictionary<string, string>> gachaList = new Dictionary<int, Dictionary<string, string>>();

    [Header("Gacha Lists")]
    [SerializeField] public List<Gacha> baseGachaList = new List<Gacha>();
    [SerializeField] public List<Gacha> eventGachaList = new List<Gacha>();

    private void Awake()
    {
        BindAll();
        //MakeGachaList();
        ShowUIStart();
        DisablePanel();

    }
    private void Start()
    {

    }

    /// <summary>
    /// csv데이터로 알맞은 가차 리스트를 분리하는 함수
    /// - 새로운 가챠 내용을 리스트를 추가하려는 경우
    ///     1. csv 파일에 GachaGroup을 묶어서 내용 수정
    ///     2. LoadingCheck 스크립트 앞에 GachaGroup의 종류만큼 리스트 선언
    ///     2. 함수의 switch문에 새로운 case로 GachaGroup 분기점 제작
    ///     3. 각 GachaGroup별 리스트 초기화
    /// </summary>
    private void MakeGachaList()
    {
        gachaList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Gacha]; // csv데이터로 가챠리스트 가져오기
        for (int i = 0; i < gachaList.Count; i++)
        {
            Gacha gachatem = new Gacha();
            gachatem.Check = TypeCastManager.Instance.TryParseInt(gachaList[i]["Check"]);
            switch (gachatem.Check) // 종류를 확인
            {
                case 1: // 종류가 Item인 경우
                    gachatem.ItemId = TypeCastManager.Instance.TryParseInt(gachaList[i]["ItemID"]);
                    break;
                case 2: // 종류가 Character인 경우
                    gachatem.CharId = TypeCastManager.Instance.TryParseInt(gachaList[i]["CharID"]);
                    break;
                default:
                    break;
            }
            gachatem.Probability = TypeCastManager.Instance.TryParseInt(gachaList[i]["Probability"]); // 확률 저장
            gachatem.Count = TypeCastManager.Instance.TryParseInt(gachaList[i]["Count"]); // 반환 갯수 저장

            switch (gachaList[i]["GachaGroup"]) // GachaGroup을 확인하여 List에 저장
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
        // 모든 리스트 세팅
        isLoading = true;
    }

    private void DisablePanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(true);
        GetUI<Image>("EventGachaPanel").gameObject.SetActive(false);
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(true);
        GetUI<Image>("ChangeEventGachaBtn").gameObject.SetActive(true);
        GetUI<Image>("ShopCharacter").gameObject.SetActive(false);
    }
    private void ShowUIStart()
    {
        GetUI<TextMeshProUGUI>("BaseSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("BaseTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("EventSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("EventTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("ChangeBaseGacahText").SetText("상설");
        GetUI<TextMeshProUGUI>("ChangeEventGacahText").SetText("이벤트");
    }

    private void SettingList()
    {

    }

    private void SettingBtn()
    {

    }

}
