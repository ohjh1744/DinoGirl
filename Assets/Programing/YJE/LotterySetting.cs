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

    //List<Dictionary<string, string>> settingList = new List<Dictionary<string, string>>();

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
        LotteryDataLoad();
        singleBtn.SetActive(true);
        tenBtn.SetActive(true);
        loadingPanel.SetActive(false);

    }

    private void LotteryDataLoad()
    {
        // TODO : cvs 파일 DB에서 원하는 정보를 불러와 GachaID 별로 List<Lottery> 생성
        // Lottery.cs에 Lottery 항목 참조

    }

    private void MakeLotteryList()
    {
        gachaList = CsvDataManager.Instance.DataLists[(int)E_CsvData.Gacha]; // csv데이터로 가챠리스트 가져오기
        for(int i = 0; i < gachaList.Count; i++)
        {

        }
        // TODO : csv 파일에서 GachaID의 개수를 파악하여 필요한 총 List 생성

    }
}
