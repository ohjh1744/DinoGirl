using Firebase.Database;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopChar : MonoBehaviour
{
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

    /// <summary>
    /// Gacha에서 사용하는 CharacterList를 Dictionary로 사용할 때 사용
    /// - 캐릭터 종류가 추가되는 경우 Switch문에 분기 설정하여 사용
    //  - GachaSceneController.cs의 MakeCharList()에서 참조하여 사용
    /// </summary>
    /// <param name="dataBaseList"></param>
    /// <param name="result"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public ShopChar MakeCharList(Dictionary<int, Dictionary<string, string>> dataBaseList, ShopChar result, int index)
    {
        result.charId = index;
        result.charName = dataBaseList[index]["Name"];
        result.rarity = TypeCastManager.Instance.TryParseInt(dataBaseList[index]["Rarity"]);
        switch (index) // 각 캐릭터에 알맞는 이미지 설정
        {
            case 1:
                result.charImageProfile = Resources.Load<Sprite>("Characters/2_testCelesProfile");
                result.video = Resources.Load<GameObject>("Prefab/1_Tricia");
                result.price = SetPrice(result.rarity);
                break;
            case 2:
                result.charImageProfile = Resources.Load<Sprite>("Characters/2_testCelesProfile");
                result.video = Resources.Load<GameObject>("Prefab/2_Celes");
                result.price = SetPrice(result.rarity);
                break;
            case 3:
                result.charImageProfile = Resources.Load<Sprite>("Characters/3_testReginaProfile");
                result.video = Resources.Load<GameObject>("Prefab/3_Regina");
                result.price = SetPrice(result.rarity);
                break;
            case 4:
                result.charImageProfile = Resources.Load<Sprite>("Characters/4_testSpinneProfile");
                result.video = Resources.Load<GameObject>("Prefab/4_Spinne");
                result.price = SetPrice(result.rarity);
                break;
            case 5:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.video = Resources.Load<GameObject>("Prefab/5_Aila");
                result.price = SetPrice(result.rarity);
                break;
            case 6:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.video = Resources.Load<GameObject>("Prefab/6_Quezna");
                result.price = SetPrice(result.rarity);
                break;
            case 7:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.video = Resources.Load<GameObject>("Prefab/7_Uloro");
                result.price = SetPrice(result.rarity);
                break;
            case 8:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.video = Resources.Load<GameObject>("Prefab/8_Eost");
                result.price = SetPrice(result.rarity);
                break;
            case 9:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.video = Resources.Load<GameObject>("Prefab/9_Melorin"); 
                result.price = SetPrice(result.rarity); 
                break;
        }
        return result;
    }

    private int SetPrice(int rarity)
    {
        switch (rarity)
        {
            case 2:
                price = 100;
                break;
            case 3:
                price = 500;
                break;
            case 4:
                price = 1000;
                break;
        }
        return price;
    }


    /// <summary>
    /// ShopChar의 정보를 ResultPanel/Panel 아래에 새로 만들어진 프리팹UI로 셋팅하는 함수
    // - GachaSceneController.cs에서 사용
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
        resultCharUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gachaChar.charName;

        GameObject rarities = resultCharUI.transform.GetChild(2).gameObject;
        // 별 개수 설정
        for (int i = 0; i < gachaChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    /// <summary>
    /// shopChar의 정보를 ShopScrollView-Viewport-CharacterContent 아래에 새로 만들어진 프리팹UI로 셋팅하는 함수
    // - ShopMaker.cs에서 사용
    /// </summary>
    /// <param name="gachaChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetShopCharUI(ShopChar shopChar, GameObject resultCharUI)
    {
        // 데이터 설정
        resultCharUI.GetComponent<ShopChar>().charId = shopChar.charId;
        resultCharUI.GetComponent<ShopChar>().charName = shopChar.CharName;
        resultCharUI.GetComponent<ShopChar>().rarity = shopChar.rarity;
        resultCharUI.GetComponent<ShopChar>().price = shopChar.price;

        // UI 출력 설정
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = shopChar.charImageProfile;
        resultCharUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shopChar.charName;

        GameObject rarities = resultCharUI.transform.GetChild(2).gameObject;
        // 별 개수 설정
        for (int i = 0; i < shopChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    public void OnBuyCharacter()
    {
        Debug.Log("버튼클릭");
        // 구매하기 버튼 클릭 시 재생할 함수
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
                Debug.Log("중복");
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
                Debug.Log($"구매완료 : {gameObject.GetComponent<ShopChar>().price}");
                int result = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] - gameObject.GetComponent<ShopChar>().price;
                PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Stone, result);
                // 실제 빌드 시 사용 - UserId불러오기
                DatabaseReference setItemRoot;
                setItemRoot = root.Child(BackendManager.Auth.CurrentUser.UserId).Child("_items/4");
                setItemRoot.SetValueAsync(result); // firebase 값 변경
            }

        }

        // 중복시
        // 구매 불가능 한 캐릭터 안내 텍스트 출력
        // 미중복시
        // 돌파석(Stone) 재화 감소(캐릭터 레어도(GachaChar.Rarity) 별 감소 수치 적용)
        // 플레이어 유닛 데이터 로컬+서버 업데이트
    }
}
