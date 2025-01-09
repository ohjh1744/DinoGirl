using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaBtn : MonoBehaviour
{
    GachaSceneController gachaSceneController;
    GachaCheck gachaCheck;
    SceneChanger sceneChanger;

    private RectTransform singleVideoContent; // 1연차 결과 내역 프리팹이 생성 될 위치
    private RectTransform tenVideoContent; // 10연차 결과 내역 프리팹이 생성 될 위치

    // 가챠 재화 비용과 아이템 종류 지정 - 인스펙터창에서 편하게 수정 가능
    [SerializeField] int gachaCost;
    [SerializeField] string gachaCostItem;

    [Header("GachaSceneController")]
    private List<GameObject> resultList = new List<GameObject>(); // 뽑기의 결과를 저장

    // GachaSceneController에 csv로 연동한 데이터를 받아서 사용
    [Header("Gacha Lists")]
    private List<Gacha> baseGachaList = new List<Gacha>();

    private void Awake()
    {
        gachaSceneController = gameObject.GetComponent<GachaSceneController>();
        gachaCheck = gameObject.GetComponent<GachaCheck>();
        sceneChanger = gameObject.GetComponent<SceneChanger>();
        singleVideoContent = gachaSceneController.GetUI<RectTransform>("SingleResultPanel");
        tenVideoContent = gachaSceneController.GetUI<RectTransform>("TenResultPanel");
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
        baseGachaList = gachaSceneController.BaseGachaList;
        GameObject resultUI = null;
        // 기본 플레이어의 재화 DinoStone(3)이 100 이상인 경우에만 실행
        // TODO : 유료 재화를 합친 값이 필요 - 유료 다이노스톤 아이템 추가
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost)
        {
            // baseGachaList의 전체 Probability의 합산을 구하기
            int total = 0;
            foreach (Gacha gacha in baseGachaList)
            {
                total += gacha.Probability;
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
                    resultUI = gachaSceneController.GachaSingleResultUI(baseGachaList, i);
                    resultList.Add(resultUI);
                    StartCoroutine(CharacterVideoR(resultUI)); // 가챠 루틴 실행
                    break;
                }
            }

            // 서버에서 플레이어의 데이터 값 수정
            // firebase 기본 UserData 루트
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            // 뽑기에 성공한 재화값 PlayerData 수정
            gachaCheck.SendChangeValue(gachaCostItem, gachaCost, false, root, PlayerDataManager.Instance.PlayerData);
            // 결과 리스트를 보며 알맞은 아이템과 캐릭터 반환을 확인하고 정보를 갱신
            gachaCheck.CheckCharId(resultList, root, PlayerDataManager.Instance.PlayerData);
            // UI 업데이트
            gachaSceneController.UpdatePlayerUI();
            StopCoroutine(CharacterVideoR(resultUI)); // 가챠 루틴 종료
            //gachaSceneController.DisableSingleImage();
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
        baseGachaList = gachaSceneController.BaseGachaList;
        GameObject resultUI = null;
        // 기본 플레이어의 재화 DinoStone(3)이 1000 이상인 경우에만 실행
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost * 10)
        {
            // baseGachaList의 전체 Probability의 합산을 구하기
            int total = 0;
            foreach (Gacha gacha in baseGachaList)
            {
                total += gacha.Probability;
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
                        resultUI = gachaSceneController.GachaTenResultUI(baseGachaList, i);
                        resultList.Add(resultUI);
                        count++;
                        weight = 0;
                        break;
                    }
                }
            } while (count < 10);
            StartCoroutine(CharacterTenVideoR());
            // 뽑기에 사용한 재화값 PlayerData 수정
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            gachaCheck.SendChangeValue(gachaCostItem, gachaCost * 10, false, root, PlayerDataManager.Instance.PlayerData);
            // 결과 리스트를 보며 알맞은 아이템과 캐릭터 반환을 확인하고 정보를 갱신
            gachaCheck.CheckCharId(resultList, root, PlayerDataManager.Instance.PlayerData);
            // UI 업데이트
            gachaSceneController.UpdatePlayerUI();
        }
        else
        {
            Debug.Log("재화 부족으로 실행 불가");
            gachaSceneController.DisabledGachaResultPanel();
        }
        StopCoroutine(CharacterTenVideoR());
    }

    /// <summary>
    /// 가챠의 캐릭터 뽑기 시 실행할 영상 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CharacterVideoR(GameObject gameObj)
    {
        if (gameObj.GetComponent<GachaChar>())
        {
            GameObject obj = Instantiate(gameObj.GetComponent<GachaChar>().Video, singleVideoContent);
            obj.SetActive(true);
            yield return new WaitUntil(() => obj.gameObject == false);
        }
        gachaSceneController.DisableSingleImage();
    }

    /// <summary>
    /// 가챠의 캐릭터 10회 뽑기 시 실행할 영상 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CharacterTenVideoR()
    {
        foreach (GameObject gameObj in resultList)
        {
            if (gameObj.GetComponent<GachaChar>())
            {
                GameObject obj = Instantiate(gameObj.GetComponent<GachaChar>().Video, tenVideoContent);
                obj.SetActive(true);
                yield return new WaitUntil(() => obj.gameObject == false);
                continue;
            }
            else
            {
                continue;
            }
        }
        gachaSceneController.DisableTenImage();
    }
}



