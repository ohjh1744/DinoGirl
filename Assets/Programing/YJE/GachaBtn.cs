using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;

public class GachaBtn : MonoBehaviour
{
    GachaSceneController gachaSceneController;
    GachaCheck gachaCheck;
    SceneChanger sceneChanger;

    [SerializeField] int gachaCost;
    [SerializeField] string gachaCostItem;
    [Header("GachaSceneController")]
    public List<GameObject> resultList = new List<GameObject>(); // 뽑기의 결과를 저장

    // GachaSceneController에 csv로 연동한 데이터를 받아서 사용
    [Header("Gacha Lists")]
    private List<Gacha> baseGachaList = new List<Gacha>();
    private List<Gacha> eventGachaList = new List<Gacha>();

    private void Awake()
    {
        gachaSceneController = gameObject.GetComponent<GachaSceneController>();
        gachaCheck = gameObject.GetComponent<GachaCheck>();
        sceneChanger = gameObject.GetComponent<SceneChanger>();
    }

    public void BackToRobby()
    {
        sceneChanger.CanChangeSceen = true;
        sceneChanger.ChangeScene("Lobby_OJH");
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
        // 기본 플레이어의 재화 DinoStone(3)이 100 이상인 경우에만 실행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost)
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
                    // Debug.Log($"반환한 GameObject : {baseGachaList[i].ItemId}");
                    break;
                }
            }
            // 서버에서 플레이어의 데이터 값 수정
            // firebase 기본 UserData 루트
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");

            gachaCheck.SendChangeValue(gachaCostItem, gachaCost, false, root, PlayerDataManager.Instance.PlayerData);

            // 뽑기에 성공한 재화값 PlayerData 수정
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].gameObject.GetComponent<GachaItem>()) // GachaItem이 존재하는 Item인 경우
                {
                    gachaCheck.SendChangeValue(resultList[i].gameObject.GetComponent<GachaItem>().ItemName,
                                               resultList[i].gameObject.GetComponent<GachaItem>().Amount, true,
                                               root, PlayerDataManager.Instance.PlayerData);
                }
                else if (resultList[i].GetComponent<GachaChar>()) // GachaChar가 존재하는 캐릭터인 경우
                {

                    int index = -1;

                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; j++)
                    {
                        if (resultList[i].GetComponent<GachaChar>().CharId == PlayerDataManager.Instance.PlayerData.UnitDatas[j].UnitId)
                        {
                            index = j;
                        }
                    }

                    if (index == -1)
                    {
                        Debug.Log("없는 캐릭터");
                        // 새로운 Unit을 저장
                        PlayerUnitData newUnit = new PlayerUnitData();
                        newUnit.UnitId = resultList[i].GetComponent<GachaChar>().CharId;
                        newUnit.UnitLevel = 1;
                        PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                        // 실제 빌드 시 사용 - UserId불러오기 
                        DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");
                        // Test 용
                        // DatabaseReference unitRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_unitDatas");

                        for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                        {
                            PlayerUnitData nowData = new PlayerUnitData();
                            nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                            nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;
                            unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                            unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                        }
                    }
                    else
                    {
                        Debug.Log("이미 소유한 캐릭터");
                        GameObject resultItem = gachaSceneController.CharReturnItem(resultList[i].gameObject.GetComponent<GachaChar>().CharId, resultList[i].gameObject);
                        gachaCheck.SendChangeValue(resultItem.gameObject.GetComponent<GachaItem>().ItemName,
                                                   resultItem.gameObject.GetComponent<GachaItem>().Amount, true,
                                                   root, PlayerDataManager.Instance.PlayerData);
                    }

                    // PlayerData의 UnitDatas에 동일한 캐릭터 아이디가 있는지 여부를 확인

                }

            }
            gachaSceneController.UpdatePlayerUI(); // UI 업데이트
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
        // 기본 플레이어의 재화 DinoStone(3)이 1000 이상인 경우에만 실행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost * 10)
        {
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
            // 뽑기에 사용한 재화값 PlayerData 수정
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            gachaCheck.SendChangeValue(gachaCostItem, gachaCost * 10, false, root, PlayerDataManager.Instance.PlayerData);

            // 뽑기에 성공한 재화값 PlayerData 수정
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].gameObject.GetComponent<GachaItem>()) // GachaItem이 존재하는 Item인 경우
                {
                    gachaCheck.SendChangeValue(resultList[i].gameObject.GetComponent<GachaItem>().ItemName,
                                               resultList[i].gameObject.GetComponent<GachaItem>().Amount, true,
                                               root, PlayerDataManager.Instance.PlayerData);
                }
                else if (resultList[i].GetComponent<GachaChar>()) // GachaChar가 존재하는 캐릭터인 경우
                {

                    int index = -1;

                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; j++)
                    {
                        if (resultList[i].GetComponent<GachaChar>().CharId == PlayerDataManager.Instance.PlayerData.UnitDatas[j].UnitId)
                        {
                            index = j;
                        }
                    }

                    if (index == -1)
                    {
                        Debug.Log("없는 캐릭터");
                        // 새로운 Unit을 저장
                        PlayerUnitData newUnit = new PlayerUnitData();
                        newUnit.UnitId = resultList[i].GetComponent<GachaChar>().CharId;
                        newUnit.UnitLevel = 1;
                        PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                        // 실제 빌드 시 사용 - UserId불러오기 
                        DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");
                        // Test 용
                        // DatabaseReference unitRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_unitDatas");

                        for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                        {
                            PlayerUnitData nowData = new PlayerUnitData();
                            nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                            nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;
                            unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                            unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                        }
                    }
                    else
                    {
                        Debug.Log("이미 소유한 캐릭터");
                        GameObject resultItem = gachaSceneController.CharReturnItem(resultList[i].gameObject.GetComponent<GachaChar>().CharId, resultList[i].gameObject);
                        gachaCheck.SendChangeValue(resultItem.gameObject.GetComponent<GachaItem>().ItemName,
                                                   resultItem.gameObject.GetComponent<GachaItem>().Amount, true,
                                                   root, PlayerDataManager.Instance.PlayerData);
                    }

                    // PlayerData의 UnitDatas에 동일한 캐릭터 아이디가 있는지 여부를 확인

                }
            }
            gachaSceneController.UpdatePlayerUI(); // UI 업데이트
        }
        else
        {
            Debug.Log("재화 부족으로 실행 불가");
            gachaSceneController.DisabledGachaResultPanel();
        }


    }


    /// <summary>
    /// 이벤트 1연차 버튼 실행 시
    /// - eventGachaList에 저장된 확률로 출력
    /// - eventGachaList에 저장된 확률로 출력
    /// </summary>
    public void EventSingleBtn()
    {
        eventGachaList = gachaSceneController.eventGachaList;
        // 기본 플레이어의 재화 DinoStone(3)이 100 이상인 경우에만 실행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost)
        {
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
            // 서버에서 플레이어의 데이터 값 수정
            // firebase 기본 UserData 루트
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            gachaCheck.SendChangeValue(gachaCostItem, gachaCost, false, root, PlayerDataManager.Instance.PlayerData);

            // 뽑기에 성공한 재화값 PlayerData 수정
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].gameObject.GetComponent<GachaItem>()) // GachaItem이 존재하는 Item인 경우
                {
                    gachaCheck.SendChangeValue(resultList[i].gameObject.GetComponent<GachaItem>().ItemName,
                                               resultList[i].gameObject.GetComponent<GachaItem>().Amount, true,
                                               root, PlayerDataManager.Instance.PlayerData);
                }
                else if (resultList[i].GetComponent<GachaChar>()) // GachaChar가 존재하는 캐릭터인 경우
                {

                    int index = -1;

                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; j++)
                    {
                        if (resultList[i].GetComponent<GachaChar>().CharId == PlayerDataManager.Instance.PlayerData.UnitDatas[j].UnitId)
                        {
                            index = j;
                        }
                    }

                    if (index == -1)
                    {
                        Debug.Log("없는 캐릭터");
                        // 새로운 Unit을 저장
                        PlayerUnitData newUnit = new PlayerUnitData();
                        newUnit.UnitId = resultList[i].GetComponent<GachaChar>().CharId;
                        newUnit.UnitLevel = 1;
                        PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                        // 실제 빌드 시 사용 - UserId불러오기 
                        DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");
                        // Test 용
                        // DatabaseReference unitRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_unitDatas");

                        for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                        {
                            PlayerUnitData nowData = new PlayerUnitData();
                            nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                            nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;
                            unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                            unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                        }
                    }
                    else
                    {
                        Debug.Log("이미 소유한 캐릭터");
                        GameObject resultItem = gachaSceneController.CharReturnItem(resultList[i].gameObject.GetComponent<GachaChar>().CharId, resultList[i].gameObject);
                        gachaCheck.SendChangeValue(resultItem.gameObject.GetComponent<GachaItem>().ItemName,
                                                   resultItem.gameObject.GetComponent<GachaItem>().Amount, true,
                                                   root, PlayerDataManager.Instance.PlayerData);
                    }

                    // PlayerData의 UnitDatas에 동일한 캐릭터 아이디가 있는지 여부를 확인

                }
            }
            gachaSceneController.UpdatePlayerUI(); // UI 업데이트
        }
        else
        {
            Debug.Log("재화 부족으로 실행 불가");
            gachaSceneController.DisabledGachaResultPanel();
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
        // 기본 플레이어의 재화 DinoStone(3)이 1000 이상인 경우에만 실행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost * 10)
        {
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
            // 뽑기에 사용한 재화값 PlayerData 수정
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            gachaCheck.SendChangeValue(gachaCostItem, gachaCost * 10, false, root, PlayerDataManager.Instance.PlayerData);

            // 뽑기에 성공한 재화값 PlayerData 수정
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i].gameObject.GetComponent<GachaItem>()) // GachaItem이 존재하는 Item인 경우
                {
                    gachaCheck.SendChangeValue(resultList[i].gameObject.GetComponent<GachaItem>().ItemName,
                                               resultList[i].gameObject.GetComponent<GachaItem>().Amount, true,
                                               root, PlayerDataManager.Instance.PlayerData);
                }
                else if (resultList[i].GetComponent<GachaChar>()) // GachaChar가 존재하는 캐릭터인 경우
                {

                    int index = -1;

                    for (int j = 0; j < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; j++)
                    {
                        if (resultList[i].GetComponent<GachaChar>().CharId == PlayerDataManager.Instance.PlayerData.UnitDatas[j].UnitId)
                        {
                            index = j;
                        }
                    }

                    if (index == -1)
                    {
                        Debug.Log("없는 캐릭터");
                        // 새로운 Unit을 저장
                        PlayerUnitData newUnit = new PlayerUnitData();
                        newUnit.UnitId = resultList[i].GetComponent<GachaChar>().CharId;
                        newUnit.UnitLevel = 1;
                        PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                        // 실제 빌드 시 사용 - UserId불러오기 
                        DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");
                        // Test 용
                        // DatabaseReference unitRoot = root.Child("Y29oJ7Tu2RQr0SZlbgYzZcDz5Xb2").Child("_unitDatas");

                        for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                        {
                            PlayerUnitData nowData = new PlayerUnitData();
                            nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                            nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;
                            unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                            unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                        }
                    }
                    else
                    {
                        Debug.Log("이미 소유한 캐릭터");
                        GameObject resultItem = gachaSceneController.CharReturnItem(resultList[i].gameObject.GetComponent<GachaChar>().CharId, resultList[i].gameObject);
                        gachaCheck.SendChangeValue(resultItem.gameObject.GetComponent<GachaItem>().ItemName,
                                                   resultItem.gameObject.GetComponent<GachaItem>().Amount, true,
                                                   root, PlayerDataManager.Instance.PlayerData);
                    }

                    // PlayerData의 UnitDatas에 동일한 캐릭터 아이디가 있는지 여부를 확인

                }
            }
            gachaSceneController.UpdatePlayerUI(); // UI 업데이트
        }
        else
        {
            Debug.Log("재화 부족으로 실행 불가");
            gachaSceneController.DisabledGachaResultPanel();
        }
    }
}



