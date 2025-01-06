using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaChar : MonoBehaviour
{
    private int charId;
    public int CharId { get { return charId; } set { charId = value; } }

    private string charName;
    public string CharName { get { return charName; } set { charName = value; } }

    private int rarity;
    public int Rarity { get { return rarity; } set { rarity = value; } }

    private Sprite charImageProfile; // 프리팹에서 사용할 이미지
    public Sprite CharImageProfile { get { return charImageProfile; } set { charImageProfile = value; } }

    // 뽑기 시 출력할 이미지
    // Resources 폴더에 있는 이미지를 연동하여 사용함
    // Resources.Load<Sprite>("파일경로/파일명");
    private Sprite charGachaImage;
    public Sprite CharGachaImage { get { return charGachaImage; } set { charGachaImage = value; } }

    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }

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
    public GachaChar MakeCharList(Dictionary<int, Dictionary<string, string>> dataBaseList, GachaChar result, int index)
    {
        result.charId = index;
        result.charName = dataBaseList[index]["Name"];
        result.rarity = TypeCastManager.Instance.TryParseInt(dataBaseList[index]["Rarity"]);
        switch (index) // 각 캐릭터에 알맞는 이미지 설정
        {
            case 1:
                result.charImageProfile = Resources.Load<Sprite>("Characters/2_testCelesProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/1_testTricia");
                result.video = Resources.Load<GameObject>("Characters/1_Tricia");
                break;
            case 2:
                result.charImageProfile = Resources.Load<Sprite>("Characters/2_testCelesProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/2_testCeles");
                result.video = Resources.Load<GameObject>("Characters/2_Celes");
                break;
            case 3:
                result.charImageProfile = Resources.Load<Sprite>("Characters/3_testReginaProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/3_testRegina");
                result.video = Resources.Load<GameObject>("Characters/3_Regina");
                break;
            case 4:
                result.charImageProfile = Resources.Load<Sprite>("Characters/4_testSpinneProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/4_testSpinne");
                result.video = Resources.Load<GameObject>("Characters/4_Spinne");
                break;
            case 5:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/5_testAila");
                result.video = Resources.Load<GameObject>("Characters/5_Aila");
                break;
            case 6:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/6_testQuezna.png");
                result.video = Resources.Load<GameObject>("Characters/6_Quezna");
                break;
            case 7:
                result.charImageProfile = Resources.Load<Sprite>("Characters/5_testAilaProfile");
                result.charGachaImage = Resources.Load<Sprite>("Characters/7_testUloro");
                result.video = Resources.Load<GameObject>("Characters/7_Uloro");
                break;
        }
        return result;
    }

    /// <summary>
    /// GachaChar의 정보를 ResultPanel/Panel 아래에 새로 만들어진 프리팹UI로 셋팅하는 함수
    // - GachaSceneController.cs에서 사용
    /// </summary>
    /// <param name="gachaChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetGachaCharUI(GachaChar gachaChar, GameObject resultCharUI)
    {
        // 데이터 설정
        resultCharUI.gameObject.GetComponent<GachaChar>().charId = gachaChar.charId;
        resultCharUI.gameObject.GetComponent<GachaChar>().charName = gachaChar.CharName;
        resultCharUI.gameObject.GetComponent<GachaChar>().rarity = gachaChar.rarity;
        resultCharUI.GetComponent<GachaChar>().video = gachaChar.video;

        // UI 출력 설정
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaChar.charImageProfile;
        resultCharUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gachaChar.charName;

        GameObject rarities = resultCharUI.transform.GetChild(2).gameObject;
        // 별 개수 설정
        for (int i = 0; i< gachaChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

}
