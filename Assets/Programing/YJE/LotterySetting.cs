using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotterySetting : MonoBehaviour
{
    // csvDataManager.cs에서 가져올 특정 DataList를 받을 List
    List<Dictionary<string, string>> settingList = new List<Dictionary<string, string>>();

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
        // TODO : csv 파일에서 GachaID의 개수를 파악하여 필요한 총 List 생성
    }
}
