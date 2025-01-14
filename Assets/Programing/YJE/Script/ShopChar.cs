using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopChar : MonoBehaviour
{

    ShopSceneController shopSceneController;

    private int charId;
    public int CharId { get { return charId; } set { charId = value; } }

    private string charName;
    public string CharName { get { return charName; } set { charName = value; } }

    private int rarity;
    public int Rarity { get { return rarity; } set { rarity = value; } }

    private Sprite charImageProfile; // 프리팹에서 사용할 이미지
    public Sprite CharImageProfile { get { return charImageProfile; } set { charImageProfile = value; } }

    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }

    // 상점 구매 가격 - 레어도 별로 다름 / 코드에서 설정
    [SerializeField] private int price;

    // 뽑기 시 출력할 컷씬    
    // Resources 폴더에 있는 이미지를 연동하여 사용함
    // Resources.Load<Sprite>("파일경로/파일명");
    private GameObject video;
    public GameObject Video { get { return video; } set { video = value; } }

    private void OnEnable()
    {
        shopSceneController = gameObject.GetComponentInParent<ShopSceneController>();
    }

    /// <summary>
    /// Character의 rarity에 따른 상점 구매 가격 설정
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    private int SetPrice(int rarity)
    {
        switch (rarity)
        {
            case 2:
                price = 50;
                break;
            case 3:
                price = 100;
                break;
            case 4:
                price = 300;
                break;
        }
        return price;
    }

    /// <summary>
    /// ShopMakeStart.cs에서 CharDic에 들어가는 ShopChar에 정보를 저장하는 함수
    /// </summary>
    /// <param name="dataBaseList"></param>
    /// <param name="result"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public ShopChar SetCharInfo(Dictionary<int, Dictionary<string, string>> dataBaseList, int index)
    {
        ShopChar result = new ShopChar();
        result.charId = index;
        result.charName = dataBaseList[index]["Name"];
        result.rarity = TypeCastManager.Instance.TryParseInt(dataBaseList[index]["Rarity"]);
        switch (index) // 각 캐릭터에 알맞는 이미지 설정
        {
            case 1:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_1");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/1_Tricia");
                result.price = SetPrice(result.rarity);
                break;
            case 2:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_2");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/2_Celes");
                result.price = SetPrice(result.rarity);
                break;
            case 3:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_3");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/3_Regina");
                result.price = SetPrice(result.rarity);
                break;
            case 4:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_4");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/4_Spinne");
                result.price = SetPrice(result.rarity);
                break;
            case 5:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_5");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/5_Aila");
                result.price = SetPrice(result.rarity);
                break;
            case 6:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_6");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/6_Quezna");
                result.price = SetPrice(result.rarity);
                break;
            case 7:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_7");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/7_Uloro");
                result.price = SetPrice(result.rarity);
                break;
            case 8:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_8");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/8_Eost");
                result.price = SetPrice(result.rarity);
                break;
            case 9:
                result.charImageProfile = Resources.Load<Sprite>("Portrait/portrait_9");
                result.video = Resources.Load<GameObject>("CutScenePrefabs/9_Melorin");
                result.price = SetPrice(result.rarity);
                break;
        }
        return result;
    }

    /// <summary>
    /// 가챠 결과 캐릭터 세팅
    /// </summary>
    /// <param name="gachaChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetGachaCharUI(ShopChar gachaChar, GameObject resultCharUI)
    {
        // 데이터 설정
        resultCharUI.GetComponent<ShopChar>().charId = gachaChar.charId;
        resultCharUI.GetComponent<ShopChar>().charName = gachaChar.CharName;
        resultCharUI.GetComponent<ShopChar>().rarity = gachaChar.rarity;
        resultCharUI.GetComponent<ShopChar>().video = gachaChar.video;

        // UI 출력 설정
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaChar.charImageProfile;
        resultCharUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gachaChar.charName;

        GameObject rarities = resultCharUI.transform.GetChild(3).gameObject;
        // 별 개수 설정
        for (int i = 0; i < gachaChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    /// <summary>
    /// 상점 캐릭터 리스트의 정보 저장/세팅
    /// </summary>
    /// <param name="shopChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetShopCharInfo(ShopChar shopChar, GameObject resultCharUI)
    {
        // 데이터 설정
        resultCharUI.GetComponent<ShopChar>().charId = shopChar.charId;
        resultCharUI.GetComponent<ShopChar>().charName = shopChar.CharName;
        resultCharUI.GetComponent<ShopChar>().rarity = shopChar.rarity;
        resultCharUI.GetComponent<ShopChar>().price = shopChar.price;
        resultCharUI.GetComponent<ShopChar>().video = shopChar.video;

        // UI 이미지와 이름 UI
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = shopChar.charImageProfile;
        resultCharUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shopChar.charName;

        // 가격 설정 UI
        GameObject buyBtn = resultCharUI.transform.GetChild(4).gameObject;
        buyBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shopChar.price.ToString();

        GameObject rarities = resultCharUI.transform.GetChild(2).gameObject;
        // 별 개수 설정 UI
        for (int i = 0; i < shopChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    public void OnBuyCharacter()
    {
        // 구매하기 버튼 클릭 시 재생할 함수
        SoundManager.Instance.PlaySFX(shopSceneController.ButtonSfx);
        // 각 프리팹(BuyCharacter.gameobject.GachaChar.CharId)의 아이디와 플레이어의 소유 유닛 아이디 중복여부 확인
        bool isChecked = false;
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] >= price) // 돌파석 개수 확인 필요
        {
            // 중복 캐릭터 여부 확인
            for (int i = 0; i < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; i++)
            {
                if (gameObject.GetComponent<ShopChar>().CharId == PlayerDataManager.Instance.PlayerData.UnitDatas[i].UnitId)
                {
                    isChecked = true;
                    break;
                }
                else
                    continue;
            }

            if (isChecked)
            {
                // 텍스트 띄우는 코루틴 실행
                StartCoroutine(shopSceneController.ShowBuyOverlapPopUp());
            }

            else if (!isChecked)
            {
                DatabaseReference root = BackendManager.Database.RootReference.Child("UserData");

                PlayerUnitData newUnit = new PlayerUnitData();
                newUnit.UnitId = gameObject.GetComponent<ShopChar>().CharId;
                newUnit.UnitLevel = 1;
                PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                DatabaseReference unitRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_unitDatas");

                // 모든 playerData.UnitDatas의 정보를 DB서버에 갱신
                for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                {
                    // nowData로 PlayerUnitData 생성
                    PlayerUnitData nowData = new PlayerUnitData();
                    nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                    nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;

                    // 지정된 위치에 순서대로 서버에 저장
                    unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                    unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                }

                // 구매 완료 텍스트를 띄우는 코루틴 출력
                int result = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] - gameObject.GetComponent<ShopChar>().price;
                PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Stone, result);
                // 실제 빌드 시 사용 - UserId불러오기
                DatabaseReference setItemRoot;
                setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                setItemRoot.SetValueAsync(result); // firebase 값 변경
                shopSceneController.ShowSingleResultPanel();
                StartCoroutine(CharacterVideoR(gameObject));
            }
            StopCoroutine(CharacterVideoR(gameObject));
        }
        else
        {
            StartCoroutine(shopSceneController.ShowGachaOverlapPopUp());
            StopCoroutine(shopSceneController.ShowGachaOverlapPopUp());
        }
        StopCoroutine(shopSceneController.ShowBuyOverlapPopUp());
    }

    /// <summary>
    /// 가챠의 캐릭터 뽑기 시 실행할 영상 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator CharacterVideoR(GameObject gameObj)
    {
        if (gameObj.GetComponent<ShopChar>())
        {
            GameObject obj = Instantiate(gameObj.GetComponent<ShopChar>().Video, gameObject.GetComponentInParent<ShopBtnManager>().SingleVideoContet);
            obj.SetActive(true);
            yield return new WaitUntil(() => obj.gameObject == false);
        }
        shopSceneController.DisableSingleImage();
        shopSceneController.DisabledGachaResultPanel();
    }

}
