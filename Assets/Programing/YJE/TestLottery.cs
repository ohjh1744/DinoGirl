using System.Collections.Generic;
using UnityEngine;

public class TestLottery : MonoBehaviour
{
    [SerializeField] RectTransform resultContent; // 10연차 결과 내역 프리팹이 생성 될 위치
    [SerializeField] GameObject resultPrefab; // 10연차 결과 내역 프리팹
    [SerializeField] GameObject resultPanel; // 10연차 결과 창
    List<Lottery> lotteries = new List<Lottery>(); // 아이디와 가중치를 저장할 리스트 => 가챠별로 필요
    List<GameObject> resultList = new List<GameObject>(); // 10연차 결과를 저장
    private int total = 0; // 가중치의 합
    private void Awake()
    {
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
    }

    /// <summary>
    /// 1연차 버튼 UI 클릭 시 실행
    /// </summary>
    public void SingleLotteryBtn()
    {
        int weight = 0;
        int selectNum = 0;
        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
        for (int i = 0; i < lotteries.Count; i++)
        {
            weight += lotteries[i].Probability;
            if (selectNum <= weight)
            {
                Debug.Log("반환한 아이템 : " + lotteries[i].Id);
                break;
            }
        }
    }

    /// <summary>
    /// 10연차 버튼 UI 클릭 시 실행
    /// </summary>
    public void TenLotteryBtn()
    {
        resultPanel.gameObject.SetActive(true);
        int weight = 0;
        int selectNum = 0;
        int count = 0;
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
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
            for (int i = 0; i < lotteries.Count; i++)
            {
                weight += lotteries[i].Probability;
                if (selectNum <= weight)
                {
                    Debug.Log(lotteries[i].Id);
                    if (results.ContainsKey(lotteries[i].Id))
                    {
                        results[lotteries[i].Id] += 1;
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
            Destroy(resultList[i]);
        }
        resultList.Clear();
        resultPanel.SetActive(false);
    }
}
