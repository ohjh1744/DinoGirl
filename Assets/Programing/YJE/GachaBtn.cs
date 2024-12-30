using Firebase.Database;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class GachaBtn : MonoBehaviour
{
    GachaSceneController gachaSceneController;

    [Header("GachaSceneController")]
    public List<GameObject> resultList = new List<GameObject>(); // 뽑기의 결과를 저장

    // GachaSceneController에 csv로 연동한 데이터를 받아서 사용
    [Header("Gacha Lists")]
    private List<Gacha> baseGachaList = new List<Gacha>();
    private List<Gacha> eventGachaList = new List<Gacha>();

    private void Awake()
    {
        gachaSceneController = gameObject.GetComponent<GachaSceneController>();
    }

    /// <summary>
    /// 결과 패널 비활성화 시
    /// resultList 를 초기화
    //  - GachaSceneController.cs에서 사용
    /// </summary>
    public void ClearResultList()
    {
        for (int i = 0; i < resultList.Count; i++)
        {
            Destroy(resultList[i]);
        }
        resultList.Clear();
    }

    /// <summary>
    /// 기본 1연차 버튼 실행 시
    /// - baseGachaList에 저장된 확률로 출력
    /// - baseGachaList에 저장된 확률로 출력
    /// </summary>
    public void BaseSingleBtn()
    {
        baseGachaList = gachaSceneController.baseGachaList;
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= 100)
        {
            // baseGachaList의 전체 Probability의 합산을 구하기
            int total = 0;
            for (int i = 0; i < baseGachaList.Count; i++)
            {
                total += baseGachaList[i].Probability;
            }

            int weight = 0;
            int selectNum = 0;
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f)); // 랜덤 숫자 뽑기
            gachaSceneController.ShowSingleResultPanel(); // 1연차 결과 패널 활성화

            for (int i = 0; i < baseGachaList.Count; i++)
            {
                weight += baseGachaList[i].Probability;
                if (selectNum <= weight) // 가중치와 숫자를 비교
                {
                    // 아이템과 캐릭터에 따라서 결과값 출력
                    // GachaSceneController.cs에 GachaResultUI()로 반환된 GameObject를 resultList에 저장
                    GameObject resultUI = gachaSceneController.GachaSingleResultUI(baseGachaList, i);
                    resultList.Add(resultUI);
                    Debug.Log($"반환한 GameObject : {baseGachaList[i].ItemId}");
                    break;
                }
            }
            /*
            // 뽑기에 사용한 재화값 PlayerData 수정
            int DSValue = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] - 100;
            // 플레이어 데이터의 값 수정
            PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.DinoStone, DSValue);
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData"); // firebase 기본 UserData 루트
            // 실제 빌드 시 사용 - UserId불러오기
            // DatabaseReference items = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/3");
            // Test용
            DatabaseReference items = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_items/3");
            items.SetValueAsync(DSValue); // firebase 값 변경
            gachaSceneController.UpdatePlayerUI(); // UI 업데이트

            // 뽑기에 성공한 재화값 PlayerData 수정
            */
        }
        else
        {
            Debug.Log("재화 부족으로 실행 불가");
            gachaSceneController.DisabledGachaResultPanel();
        }
        
    }

    /// <summary>
    /// 기본 10연차 버튼 실행 시
    /// - baseGachaList에 저장된 확률로 출력
    /// - baseGachaList에 저장된 확률로 출력
    /// </summary>
    public void BaseTenBtn()
    {
        baseGachaList = gachaSceneController.baseGachaList;
        // baseGachaList의 전체 Probability의 합산을 구하기
        int total = 0;
        for (int i = 0; i < baseGachaList.Count; i++)
        {
            total += baseGachaList[i].Probability;
        }

        gachaSceneController.ShowTenResultPanel(); // 10연차 결과패널 활성화

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
                    // 아이템과 캐릭터에 따라서 결과값 출력
                    // GachaSceneController.cs에 GachaResultUI()로 반환된 GameObject를 resultList에 저장
                    GameObject resultUI = gachaSceneController.GachaTenResultUI(baseGachaList, i);
                    resultList.Add(resultUI);
                    Debug.Log($"반환한 GameObject : {baseGachaList[i].ItemId}");
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
    public void EventSingleBtn()
    {
        eventGachaList = gachaSceneController.eventGachaList;
        // eventGachaList의 전체 Probability의 합산을 구하기
        int total = 0;
        for (int i = 0; i < eventGachaList.Count; i++)
        {
            total += eventGachaList[i].Probability;
        }

        int weight = 0;
        int selectNum = 0;
        selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f)); // 랜덤 숫자 뽑기
        gachaSceneController.ShowSingleResultPanel(); // 1연차 결과 패널 활성화

        for (int i = 0; i < eventGachaList.Count; i++)
        {
            weight += eventGachaList[i].Probability;
            if (selectNum <= weight) // 가중치와 숫자를 비교
            {
                // 아이템과 캐릭터에 따라서 결과값 출력
                // GachaSceneController.cs에 GachaSingleResultUI()로 반환된 GameObject를 resultList에 저장
                GameObject resultUI = gachaSceneController.GachaSingleResultUI(eventGachaList, i);
                resultList.Add(resultUI);
                Debug.Log($"반환한 GameObject : {eventGachaList[i].ItemId}");
                break;
            }
        }

    }

    /// <summary>
    /// 이벤트 10연차 버튼 실행 시
    /// - eventGachaList에 저장된 확률로 출력
    /// - eventGachaList에 저장된 확률로 출력
    /// </summary>
    public void EventTenBtn()
    {
        eventGachaList = gachaSceneController.eventGachaList;
        int total = 0;
        for (int i = 0; i < eventGachaList.Count; i++)
        {
            total += eventGachaList[i].Probability;
        }
        gachaSceneController.ShowTenResultPanel(); // 10연차 결과패널 활성화

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
                    // 아이템과 캐릭터에 따라서 결과값 출력
                    // GachaSceneController.cs에 GachaTenResultUI로 반환된 GameObject를 resultList에 저장
                    GameObject resultUI = gachaSceneController.GachaTenResultUI(eventGachaList, i);
                    resultList.Add(resultUI);
                    Debug.Log($"반환한 GameObject : {eventGachaList[i].ItemId}");
                    count++;
                    weight = 0;
                    break;
                }
            }
        } while (count < 10);
    }
}



