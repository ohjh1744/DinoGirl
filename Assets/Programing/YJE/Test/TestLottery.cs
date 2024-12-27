using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLottery : MonoBehaviour
{
    [SerializeField] RectTransform resultContent; // 10연차 결과 내역 프리팹이 생성 될 위치
    [SerializeField] GameObject resultPrefab; // 10연차 결과 내역 프리팹
    [SerializeField] GameObject resultPanel; // 10연차 결과 창

    // 가챠의 종류가 늘어나는 경우 추가
    [Header("Lottery Lists")]
    [SerializeField] LotterySetting lotterySetting;
    [SerializeField] List<Lottery> lotteryList1 = new List<Lottery>();
    [SerializeField] List<Lottery> lotteryList2 = new List<Lottery>();

    List<GameObject> resultList = new List<GameObject>(); // 10연차 결과를 저장

    private int total1 = 0; // 가중치의 합
    private int total2 = 0; // 가중치의 합

    private void Awake()
    {
        /*
        Lottery gold = new Lottery();
        gold.Id = 500;
        gold.Probability = 3000;
        lotteries.Add(gold);

        Lottery binoBlood = new Lottery();
        binoBlood.Id = 501;
        binoBlood.Probability = 3500;
        lotteries.Add(binoBlood);

        Lottery boneCrystal = new Lottery();
        boneCrystal.Id = 502;
        boneCrystal.Probability = 2500;
        lotteries.Add(boneCrystal);

        Lottery dinoStone = new Lottery();
        dinoStone.Id = 503;
        dinoStone.Probability = 500;
        lotteries.Add(dinoStone);

        Lottery stone = new Lottery();
        stone.Id = 504;
        stone.Probability = 500;
        lotteries.Add(stone);
        
        for (int i = 0; i < lotteries.Count; i++)
        {
            total += lotteries[i].Probability;
        }
        */
    }

    private void Start()
    {

    }

    private void UpdateList()
    {
        //lotteryList1.Clear();
        lotteryList1 = lotterySetting.lotteryList1;
        total1 = 0;
        for (int i = 0; i < lotteryList1.Count; i++)
        {
            total1 += lotteryList1[i].Probability;
        }

        //lotteryList2.Clear();
        lotteryList2 = lotterySetting.lotteryList2;
        total2 = 0;
        for (int i = 0; i < lotteryList2.Count; i++)
        {
            total2 += lotteryList2[i].Probability;
        }
    }

    /// <summary>
    /// 1연차 버튼 UI 클릭 시 실행
    /// </summary>
    public void SingleLottery1Btn()
    {
        UpdateList();
        int weight = 0;
        int selectNum = 0;
        selectNum = Mathf.RoundToInt(total1 * Random.Range(0.0f, 1.0f));
        for (int i = 0; i < lotteryList1.Count; i++)
        {
            weight += lotteryList1[i].Probability;
            if (selectNum <= weight)
            {
                Debug.Log("반환한 아이템 : " + lotteryList1[i].Id);
                break;
            }
        }
    }

    /// <summary>
    /// 10연차 버튼 UI 클릭 시 실행
    /// </summary>
    public void TenLottery1Btn()
    {
        UpdateList();
        resultPanel.gameObject.SetActive(true);
        int weight = 0; // 현재 위치의 가중치
        int selectNum = 0; // 선택한 랜덤 번호
        int count = 0; // 총 10번의 회수를 카운팅 하는 변수
        // 테스트 저장용
        Dictionary<int, float> results = new Dictionary<int, float>();
        results.Add(500, 0f);
        results.Add(501, 0f);
        results.Add(502, 0f);
        results.Add(503, 0f);
        results.Add(504, 0f);
        // 테스트용
        do
        {
            selectNum = Mathf.RoundToInt(total1 * Random.Range(0.0f, 1.0f));

            // 가챠용 리스트의 횟수 만큼 반복하며 가중치에 해당하는 결과 출력
            for (int i = 0; i < lotteryList1.Count; i++)
            {
                weight += lotteryList1[i].Probability;
                if (selectNum <= weight)
                {
                    Debug.Log(lotteryList1[i].Id);
                    if (results.ContainsKey(lotteryList1[i].Id))
                    {
                        results[lotteryList1[i].Id] += 1;
                        resultList.Add(Instantiate(resultPrefab, resultContent));
                    }
                    count++;
                    weight = 0;
                    break;
                }
            }

        } while (count < 10); // 테스트용 카운트 10000

        // 테스트 확률 표
        Debug.Log($"500번 : {results[500]} / {count} => {results[500] / count * 100}");
        Debug.Log($"501번 : {results[501]} / {count} => {results[501] / count * 100}");
        Debug.Log($"502번 : {results[502]} / {count} => {results[502] / count * 100}");
        Debug.Log($"503번 : {results[503]} / {count} => {results[503] / count * 100}");
        Debug.Log($"504번 : {results[504]} / {count} => {results[504] / count * 100}");
    }

    /// <summary>
    /// 결과 창 종료
    /// </summary>
    public void ResultPanelBtn()
    {
        for(int i = 0;i < resultList.Count; i++)
        {
            Destroy(resultList[i].gameObject);
        }
        resultList.Clear();
        resultPanel.SetActive(false);
    }
}
