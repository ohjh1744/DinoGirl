using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO : 
// 1. 각 패널별 비활성화가 필요한 경우 함수 제작
// 2. 이벤트를 활용하여 LoadingCheck 스크립트와 기초 Setting 연관짓기
// 3. resultList에 저장된 뽑기를 resultPrefab에 알맞은 위치에 파일을 넣어서 출력하도록 함수 수정하기

/// <summary>
/// GachaScene의 전체적인 관리를 하는 스크립트
/// - CsvDataManager와 연결
/// - PlayData와 연결
/// - UIBInder를 사용하여 이벤트 선언 후 알맞게 이벤트로 각 UI의 활성화 설정
/// </summary>
public class GachaSceneController : UIBInder
{
    private bool isLoading = false;
    public bool IsLoading { get { return isLoading; } set { isLoading = value; } }

    // csvDataManager.cs에서 가져올 특정 DataList를 받을 Disctionary
    Dictionary<int, Dictionary<string, string>> gachaList = new Dictionary<int, Dictionary<string, string>>();

    List<GameObject> resultList = new List<GameObject>(); // 뽑기의 결과를 저장

    [Header("Gacha Lists")]
    [SerializeField] public List<Gacha> baseGachaList = new List<Gacha>();
    [SerializeField] public List<Gacha> eventGachaList = new List<Gacha>();

    [Header("UI")]
    [SerializeField] RectTransform resultContent; // 10연차 결과 내역 프리팹이 생성 될 위치
    [SerializeField] GameObject resultPrefab; // 10연차 결과 내역 프리팹
    [SerializeField] GameObject resultPanel; // 10연차 결과 창

    private void Awake()
    {
        BindAll();
        SettingStartUI();
        SettingStartPanel();

    }

    /// <summary>
    /// 테스트를 위한 Update문 실행
    /// - 추후 제거 후 LoadingCheck 스크립트에서 판단 후 이벤트로 사용하기
    /// </summary>
    private void Update()
    {
        if (CsvDataManager.Instance.IsLoad && !isLoading)
        {
            MakeGachaList();
            SettingBtn();
        }
        else if (isLoading)
        {
            return;
        }
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
        for (int i = 1; i < gachaList.Count; i++)
        {
            Debug.Log(gachaList[i]["Check"]);
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

    /// <summary>
    /// UI버튼세팅
    /// </summary>
    private void SettingBtn()
    {
        GetUI<Button>("TenResultPanel").onClick.AddListener(DisabledGachaResultPanel);
        GetUI<Button>("BaseSingleBtn").onClick.AddListener(BaseSingleBtn);
        GetUI<Button>("BaseTenBtn").onClick.AddListener(BaseTenBtn);
        GetUI<Button>("EventSingleBtn").onClick.AddListener(EventSingleBtn);
        GetUI<Button>("EventTenBtn").onClick.AddListener(EventTenBtn);

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
    /// 시작 시 버튼의 문구 설정
    /// </summary>
    private void SettingStartUI()
    {
        GetUI<TextMeshProUGUI>("BaseSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("BaseTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("EventSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("EventTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("ChangeBaseGacahText").SetText("상설");
        GetUI<TextMeshProUGUI>("ChangeEventGacahText").SetText("이벤트");
    }

    /// <summary>
    /// BaseGachaPanel 활성화
    /// </summary>
    private void ShowBaseGachaPanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(true);
    }
    /// <summary>
    /// EventGachaPanel 활성화
    /// </summary>
    private void ShowEventGachaPanel()
    {
        GetUI<Image>("EventGachaPanel").gameObject.SetActive(true);
    }
    /// <summary>
    /// GachaResultPanel 활성화
    /// </summary>
    private void ShowGachaResultPanel()
    {
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(true);
    }
    /// <summary>
    /// SingleResultPanel 활성화
    /// </summary>
    private void ShowSingleResultPanel()
    {
        GetUI<Image>("SingleResultPanell").gameObject.SetActive(true);
    }
    /// <summary>
    /// TenResultPanel 활성화
    /// </summary>
    private void ShowTenResultPanel()
    {
        GetUI<Image>("TenResultPanel").gameObject.SetActive(true);
    }
    /// <summary>
    /// GachaResultPanel 비활성화
    /// - 결과 저장 리스트를 초기화
    /// - 결과 패널을 비활성화
    /// </summary>
    private void DisabledGachaResultPanel()
    {
        resultList.Clear();
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        GetUI<Image>("SingleResultPanell").gameObject.SetActive(false);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(false);
    }




    /// <summary>
    /// 기본 1연차 버튼 실행 시
    /// - baseGachaList에 저장된 확률로 출력
    /// - baseGachaList에 저장된 확률로 출력
    /// </summary>
    private void BaseSingleBtn()
    {
        int total = 0;
        for (int i = 0; i < baseGachaList.Count; i++)
        {
            total += baseGachaList[i].Probability;
        }

        int weight = 0;
        int selectNum = 0;
        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
        for (int i = 0; i < baseGachaList.Count; i++)
        {
            weight += baseGachaList[i].Probability;
            if (selectNum <= weight)
            {
                if (baseGachaList[i].Check == 1)
                {
                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    Debug.Log("반환한 아이템 : " + baseGachaList[i].ItemId);
                }
                else if (baseGachaList[i].Check == 0)
                {
                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    Debug.Log("반환한 아이템 : " + baseGachaList[i].CharId);
                }

                break;
            }
        }
    }
    /// <summary>
    /// 기본 10연차 버튼 실행 시
    /// - baseGachaList에 저장된 확률로 출력
    /// - baseGachaList에 저장된 확률로 출력
    /// </summary>
    private void BaseTenBtn()
    {
        int total = 0;
        for (int i = 0; i < baseGachaList.Count; i++)
        {
            total += baseGachaList[i].Probability;
        }
        ShowGachaResultPanel();
        int weight = 0; // 현재 위치의 가중치
        int selectNum = 0; // 선택한 랜덤 번호
        int count = 0; // 총 10번의 회수를 카운팅 하는 변수

        do
        {
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

            // 가챠용 리스트의 횟수 만큼 반복하며 가중치에 해당하는 결과 출력
            for (int i = 0; i < baseGachaList.Count; i++)
            {
                weight += baseGachaList[i].Probability;
                if (selectNum <= weight)
                {
                    Debug.Log(baseGachaList[i].ItemId);
                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    count++;
                    weight = 0;
                    break;
                }
            }
        } while (count < 10);
    }
    /// <summary>
    /// 이벤트 1연차 버튼 실행 시
    /// - eventGachaList에 저장된 확률로 출력
    /// - eventGachaList에 저장된 확률로 출력
    /// </summary>
    private void EventSingleBtn()
    {
        int total = 0;
        for (int i = 0; i < eventGachaList.Count; i++)
        {
            total += eventGachaList[i].Probability;
        }

        int weight = 0;
        int selectNum = 0;
        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
        for (int i = 0; i < eventGachaList.Count; i++)
        {
            weight += eventGachaList[i].Probability;
            if (selectNum <= weight)
            {
                if (eventGachaList[i].Check == 1)
                {
                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    Debug.Log("반환한 아이템 : " + eventGachaList[i].ItemId);
                }
                else if (eventGachaList[i].Check == 0)
                {

                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    Debug.Log("반환한 아이템 : " + eventGachaList[i].CharId);
                }

                break;
            }
        }
    }
    /// <summary>
    /// 이벤트 10연차 버튼 실행 시
    /// - eventGachaList에 저장된 확률로 출력
    /// - eventGachaList에 저장된 확률로 출력
    /// </summary>
    private void EventTenBtn()
    {
        int total = 0;
        for (int i = 0; i < eventGachaList.Count; i++)
        {
            total += eventGachaList[i].Probability;
        }
        ShowGachaResultPanel();
        int weight = 0; // 현재 위치의 가중치
        int selectNum = 0; // 선택한 랜덤 번호
        int count = 0; // 총 10번의 회수를 카운팅 하는 변수

        do
        {
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));

            // 가챠용 리스트의 횟수 만큼 반복하며 가중치에 해당하는 결과 출력
            for (int i = 0; i < eventGachaList.Count; i++)
            {
                weight += eventGachaList[i].Probability;
                if (selectNum <= weight)
                {
                    Debug.Log(eventGachaList[i].ItemId);
                    resultList.Add(Instantiate(resultPrefab, resultContent));
                    count++;
                    weight = 0;
                    break;
                }
            }
        } while (count < 10);
    }
}
