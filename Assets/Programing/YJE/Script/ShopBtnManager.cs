using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBtnManager : MonoBehaviour
{
    ShopSceneController shopSceneController;
    ShopMakeStart shopMakeStart;
    ValueChange valueChange;
    [SerializeField] SceneChanger sceneChanger;

    private RectTransform singleResultContent; // 1연차 결과 내역 프리팹이 생성 될 위치
    private RectTransform tenResultContent; // 10연차 결과 내역 프리팹이 생성 될 위치
    private RectTransform returnContent; // 중복캐릭터 아이템 반환 프리팹이 생성 될 위치
    private RectTransform singleVideoContent; // 1연차 결과 내역 프리팹이 생성 될 위치
    public RectTransform SingleVideoContet { get {  return singleVideoContent; } set { singleResultContent = value; } }
    private RectTransform tenVideoContent; // 10연차 결과 내역 프리팹이 생성 될 위치

    private GameObject resultCharPrefab; // 결과가 캐릭터인 경우 사용할 프리팹
    private GameObject resultItemPrefab; // 결과가 아이템인 경우 사용할 프리팹
    private GameObject returnPrefab; // 중복캐릭터 아이템 반환 프리팹

    private List<Gacha> baseGachaList = new List<Gacha>(); // 기본 뽑기 List
    private List<GameObject> resultList = new List<GameObject>(); // 뽑기의 결과를 저장

    private Dictionary<int, Item> itemDic = new Dictionary<int, Item>(); // 아이템 Dictionary
    private Dictionary<int, ShopChar> charDic = new Dictionary<int, ShopChar>(); // 캐릭터 Dictionary
    private Dictionary<int, GachaReturn> charReturnItemDic = new Dictionary<int, GachaReturn>(); // 중복 캐릭터 반환 아이템 Dictionary

    [SerializeField] int gachaCost;
    public int GachaCost { get { return gachaCost; } set { gachaCost = value; } }
    [SerializeField] string gachaCostItem;

    private void Awake()
    {
        shopSceneController = gameObject.GetComponent<ShopSceneController>();
        shopMakeStart = gameObject.GetComponent<ShopMakeStart>();
        valueChange = gameObject.GetComponent<ValueChange>();

        // 각 프리팹 지정
        resultCharPrefab = Resources.Load<GameObject>("Prefabs/ResultCharacter");
        resultItemPrefab = Resources.Load<GameObject>("Prefabs/ResultItem");
        returnPrefab = Resources.Load<GameObject>("Prefabs/ReturnItem");

        // 각 프리팹 생성 위치 지정
        singleResultContent = shopSceneController.GetUI<RectTransform>("SingleResultGrid");
        tenResultContent = shopSceneController.GetUI<RectTransform>("TenResultGrid");
        singleVideoContent = shopSceneController.GetUI<RectTransform>("SingleResultPanel");
        tenVideoContent = shopSceneController.GetUI<RectTransform>("TenResultPanel");
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
    /// 기본 1회 가챠 버튼
    /// </summary>
    public void OnBaseSingleBtn()
    {
        baseGachaList = shopMakeStart.BaseGachaList;
        GameObject resultUI = null;
        // 기본 플레이어의 재화 DinoStone(3)이 100 이상인 경우에만 실행
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
            shopSceneController.ShowSingleResultPanel(); // 1연차 결과 패널 활성화

            for (int i = 0; i < baseGachaList.Count; i++)
            {
                weight += baseGachaList[i].Probability;
                if (selectNum <= weight) // 가중치와 숫자를 비교
                {
                    // 아이템과 캐릭터에 따라서 결과값 출력
                    // GachaSceneController.cs에 GachaResultUI()로 반환된 GameObject를 resultList에 저장
                    resultUI = GachaResultUI(baseGachaList, i, 1);
                    resultList.Add(resultUI);
                    StartCoroutine(CharacterVideoR(resultUI)); // 가챠 루틴 실행
                    break;
                }
            }

            // 서버에서 플레이어의 데이터 값 수정
            // firebase 기본 UserData 루트
            DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");
            // 뽑기에 성공한 재화값 PlayerData 수정
            valueChange.SendChangeValue(gachaCostItem, gachaCost, false, root, PlayerDataManager.Instance.PlayerData);
            // 결과 리스트를 보며 알맞은 아이템과 캐릭터 반환을 확인하고 정보를 갱신
            valueChange.CheckCharId(resultList, root, PlayerDataManager.Instance.PlayerData);

            StopCoroutine(CharacterVideoR(resultUI)); // 가챠 루틴 종료
        }
        else
        {
            StartCoroutine(shopSceneController.ShowGachaOverlapPopUp());
            StopCoroutine(shopSceneController.ShowGachaOverlapPopUp());
        }
    }

    /// <summary>
    /// 기본 10회 가챠 버튼
    /// </summary>
    public void OnBaseTenBtn()
    {
        baseGachaList = shopMakeStart.BaseGachaList;
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
            shopSceneController.ShowTenResultPanel(); // 10연차 결과패널 활성화

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
                        resultUI = GachaResultUI(baseGachaList, i, 10);
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
            valueChange.SendChangeValue(gachaCostItem, gachaCost * 10, false, root, PlayerDataManager.Instance.PlayerData);
            // 결과 리스트를 보며 알맞은 아이템과 캐릭터 반환을 확인하고 정보를 갱신
            valueChange.CheckCharId(resultList, root, PlayerDataManager.Instance.PlayerData);
        }
        else
        {
            StartCoroutine(shopSceneController.ShowGachaOverlapPopUp());
            StopCoroutine(shopSceneController.ShowGachaOverlapPopUp());
        }
        StopCoroutine(CharacterTenVideoR());
    }

    /// <summary>
    /// 가차결과 패널 버튼
    /// </summary>
    public void OnDisableGachaPanelBtn()
    {
        ClearResultList();
        shopSceneController.SoundBgm();
        shopSceneController.DisabledGachaResultPanel();
    }

    /// <summary>
    /// 로비로 돌아가기 버튼에 적용
    /// </summary>
    public void OnBackToRobby()
    {
        sceneChanger.ChangeScene("Lobby_OJH");
        sceneChanger.CanChangeSceen = true;
    }

    public void OnBaseGachaBtn()
    {
        shopSceneController.ShowBaseGachaPanel();
    }
    public void OnShopBtn()
    {
        shopSceneController.ShowShopPanel();
    }


    /// <summary>
    /// 뽑기 실행 시
    /// GachaList와 index값을 받아서 해당하는 결과가 아이템/캐릭터인지 판단
    /// 분류에 따른 Prefab으로 GameObject를 생성
    /// 알맞은 결과를 UI로 출력
    /// GameObject로 반환하는 함수
    /// </summary>
    /// <param name="GachaList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GachaResultUI(List<Gacha> GachaList, int index, int count)
    {
        charDic = shopMakeStart.CharDic;
        itemDic = shopMakeStart.ItemDic;

        if (count == 1) // 1회 뽑기 시
        {
            switch (GachaList[index].Check)
            {
                case 0: // 반환이 캐릭터인 경우
                    ShopChar resultChar = charDic[GachaList[index].CharId];
                    resultChar.Amount = GachaList[index].Count;
                    GameObject resultCharUI = Instantiate(resultCharPrefab, singleResultContent);
                    resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                    return resultCharUI;
                case 1: // 반환이 아이템인 경우
                    Item result = itemDic[GachaList[index].ItemId]; // GachaItem 설정
                    result.Amount = GachaList[index].Count; // GachaItem의 Amount를 정해진 수량으로 설정

                    GameObject resultUI = Instantiate(resultItemPrefab, singleResultContent); // Prefab으로 정해진 위치에 생성 - 한개
                    resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem을 적용한 UI Setting
                    return resultUI;
                default:
                    return null;
            }
        }
        else if(count == 10) // 10회 뽑기 시
        {
            switch (GachaList[index].Check)
            {
                case 0: // 반환이 캐릭터인 경우
                    ShopChar resultChar = charDic[GachaList[index].CharId];
                    resultChar.Amount = GachaList[index].Count;
                    GameObject resultCharUI = Instantiate(resultCharPrefab, tenResultContent);
                    resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                    return resultCharUI;
                case 1: // 반환이 아이템인 경우
                    Item result = itemDic[GachaList[index].ItemId]; // GachaItem 설정
                    result.Amount = GachaList[index].Count; // GachaItem의 Amount를 정해진 수량으로 설정

                    GameObject resultUI = Instantiate(resultItemPrefab, tenResultContent); // Prefab으로 정해진 위치에 생성 - 열개
                    resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem을 적용한 UI Setting

                    return resultUI;
                default:
                    return null;
            }
        }
        return null;
    }

    /// <summary>
    /// 가챠에서의 Character의 중복을 확인한 후 Character을 이미 소지하고 있는 경우
    /// 아이템으로 변환한 내용으로 알맞은 UI를 출력
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="resultListObj"></param>
    public GameObject CharReturnItem(int UnitId, GameObject resultListObj)
    {
        itemDic = shopMakeStart.ItemDic;
        charReturnItemDic = shopMakeStart.CharReturnItemDic;
        returnContent = resultListObj.transform.GetChild(3).GetComponent<RectTransform>();
        GameObject resultObjUI = Instantiate(returnPrefab, returnContent); // 그 위치에 새로운 프리팹으로 생성

        Item resultItem = resultObjUI.gameObject.GetComponent<Item>(); // 프리팹에 내용 설정
        resultItem.ItemId = charReturnItemDic[UnitId].ItemId;
        resultItem.Amount = charReturnItemDic[UnitId].Count;
        resultItem.ItemName = itemDic[charReturnItemDic[UnitId].ItemId].ItemName;
        resultItem.ItemImage = itemDic[charReturnItemDic[UnitId].ItemId].ItemImage;

        resultObjUI = resultItem.SetGachaReturnItemUI(resultItem, resultObjUI);

        return resultObjUI;
    }

    /// <summary>
    /// 가챠의 캐릭터 뽑기 시 실행할 영상 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator CharacterVideoR(GameObject gameObj)
    {
        if (gameObj.GetComponent<ShopChar>())
        {
            GameObject obj = Instantiate(gameObj.GetComponent<ShopChar>().Video, singleVideoContent);
            obj.SetActive(true);
            yield return new WaitUntil(() => obj.gameObject == false);
        }
        shopSceneController.DisableSingleImage();
    }

    /// <summary>
    /// 가챠의 캐릭터 10회 뽑기 시 실행할 영상 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CharacterTenVideoR()
    {
        foreach (GameObject gameObj in resultList)
        {
            if (gameObj.GetComponent<ShopChar>())
            {
                GameObject obj = Instantiate(gameObj.GetComponent<ShopChar>().Video, tenVideoContent);
                obj.SetActive(true);
                yield return new WaitUntil(() => obj.gameObject == false);
                continue;
            }
            else
            {
                continue;
            }
        }
        shopSceneController.DisableTenImage();
    }


}
