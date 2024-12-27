using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DBTest와 연동하여 DataManager가 Data를 전부 불러온 후에 각종 세팅을 진행
/// - 전체적인 씬의 진행사항에 필요한 정보들과 다른 클래스나 데이터와 연동된 정보를 저장
/// - 씬이 로딩중일 때 모든 세팅을 마친 후 플레이어가 뽑기를 실행 할 수 있어야함
/// - 전체 설정 후 LoadingPanel 비활성화 + button 활성화 하면서 뽑기를 실행
/// - 로비씬과 연동시 DBTest가 사라지니 LoadingPanel이 활성화 된 상태에서 세팅을 진행하고
///   세팅이 완료되면 전체 설정 후 LoadingPanel 비활성화 + button 활성화 하면서 뽑기를 실행하도록 해야함
/// </summary>
public class LotterySetting : MonoBehaviour
{
    // csvDataManager.cs에서 가져올 특정 DataList를 받을 Disctionary
    [SerializeField] Dictionary<int, Dictionary<string, string>> gachaList = new Dictionary<int, Dictionary<string, string>>();
    [Header("Lottery Lists")]
    public List<Lottery> lotteryList1 = new List<Lottery>();
    public List<Lottery> lotteryList2 = new List<Lottery>();

    //List<Dictionary<string, string>> settingList = new List<Dictionary<string, string>>();
    private bool isChecked;
    public bool IsChecked { get { return isChecked; } set { isChecked = value; } }

    [Header("UI")]
    [SerializeField] private GameObject singleBtn;
    [SerializeField] private GameObject tenBtn;
    [SerializeField] private GameObject loadingPanel;

    private void Awake()
    {

        // TODO :
        // MakeLotteryList() 실행
        // LotteryDataLoad() 실행
        // 버튼들 활성화
        // Loading Panel 비활성화
        MakeLotteryList();
        singleBtn.SetActive(true);
        tenBtn.SetActive(true);
        loadingPanel.SetActive(false);

    }

    /// <summary>
    /// csv데이터로 알맞은 가챠 리스트를 분리하는 함수
    /// - 새로운 가챠를 추가하는 경우
    ///    1. csv파일에 GachaGroup을 묶어서 파일 업로드
    ///    2. 함수의 switch문에 새로운 case로 GachaGroup 분기점 제작
    ///    3. Lottery Lists에 분기할 list를 미리 제작 후 분기에 알맞게 데이터 저장
    /// </summary>
    private void MakeLotteryList()
    {
        gachaList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Gacha]; // csv데이터로 가챠리스트 가져오기
        for (int i = 1; i < gachaList.Count + 1; i++)
        {
            // Lottery 타입의 lottery를 선언하고 형변환을 거쳐 ID와 Probability를 저장
            Lottery lottery = new Lottery();
            lottery.Id = TypeCastManager.Instance.TryParseInt(gachaList[i]["ItemID"]);
            lottery.Probability = TypeCastManager.Instance.TryParseInt(gachaList[i]["Probability"]);
            // GachaGroup을 기준으로 알맞은 loggeryList에 저장
            switch (gachaList[i]["GachaGroup"])
            {
                case "1":
                    lotteryList1.Add(lottery);
                    break;
                case "2":
                    lotteryList2.Add(lottery);
                    break;
                default:
                    break;
            }
        }
    }
}
