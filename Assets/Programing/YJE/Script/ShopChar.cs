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

    private int rarity;

    private Sprite charImageProfile; // �����տ��� ����� �̹���

    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }

    // ���� ���� ���� - ��� ���� �ٸ� / �ڵ忡�� ����
    private int price;

    // �̱� �� ����� �ƾ�    
    // Resources ������ �ִ� �̹����� �����Ͽ� �����
    // Resources.Load<Sprite>("���ϰ��/���ϸ�");
    private GameObject video;
    public GameObject Video { get { return video; } set { video = value; } }

    private void OnEnable()
    {
        shopSceneController = gameObject.GetComponentInParent<ShopSceneController>();
    }

    /// <summary>
    /// Character�� rarity�� ���� ���� ���� ���� ����
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
    /// ShopMakeStart.cs���� CharDic�� ���� ShopChar�� ������ �����ϴ� �Լ�
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
        switch (index) // �� ĳ���Ϳ� �˸´� �̹��� ����
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
    /// ��í ��� ĳ���� ����
    /// </summary>
    /// <param name="gachaChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetGachaCharUI(ShopChar gachaChar, GameObject resultCharUI)
    {
        // ������ ����
        resultCharUI.GetComponent<ShopChar>().charId = gachaChar.charId;
        resultCharUI.GetComponent<ShopChar>().charName = gachaChar.charName;
        resultCharUI.GetComponent<ShopChar>().rarity = gachaChar.rarity;
        resultCharUI.GetComponent<ShopChar>().video = gachaChar.video;

        // UI ��� ����
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = gachaChar.charImageProfile;
        resultCharUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gachaChar.charName;

        GameObject rarities = resultCharUI.transform.GetChild(3).gameObject;
        // �� ���� ����
        for (int i = 0; i < gachaChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    /// <summary>
    /// ���� ĳ���� ����Ʈ�� ���� ����/����
    /// </summary>
    /// <param name="shopChar"></param>
    /// <param name="resultCharUI"></param>
    /// <returns></returns>
    public GameObject SetShopCharInfo(ShopChar shopChar, GameObject resultCharUI)
    {
        // ������ ����
        resultCharUI.GetComponent<ShopChar>().charId = shopChar.charId;
        resultCharUI.GetComponent<ShopChar>().charName = shopChar.charName;
        resultCharUI.GetComponent<ShopChar>().rarity = shopChar.rarity;
        resultCharUI.GetComponent<ShopChar>().price = shopChar.price;
        resultCharUI.GetComponent<ShopChar>().video = shopChar.video;

        // UI �̹����� �̸� UI
        resultCharUI.transform.GetChild(0).GetComponent<Image>().sprite = shopChar.charImageProfile;
        resultCharUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shopChar.charName;

        // ���� ���� UI
        GameObject buyBtn = resultCharUI.transform.GetChild(4).gameObject;
        buyBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = shopChar.price.ToString();

        GameObject rarities = resultCharUI.transform.GetChild(2).gameObject;
        // �� ���� ���� UI
        for (int i = 0; i < shopChar.rarity; i++)
        {
            rarities.transform.GetChild(i).gameObject.SetActive(true);
        }
        return resultCharUI;
    }

    public void OnBuyCharacter()
    {
        // �����ϱ� ��ư Ŭ�� �� ����� �Լ�
        SoundManager.Instance.PlaySFX(shopSceneController.ButtonSfx);
        // �� ������(BuyCharacter.gameobject.GachaChar.CharId)�� ���̵�� �÷��̾��� ���� ���� ���̵� �ߺ����� Ȯ��
        bool isChecked = false;
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] >= price) // ���ļ� ���� Ȯ�� �ʿ�
        {
            // �ߺ� ĳ���� ���� Ȯ��
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
                // �ؽ�Ʈ ���� �ڷ�ƾ ����
                StartCoroutine(shopSceneController.ShowBuyOverlapPopUp());
            }

            else if (!isChecked)
            {
                DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData");

                PlayerUnitData newUnit = new PlayerUnitData();
                newUnit.UnitId = gameObject.GetComponent<ShopChar>().CharId;
                newUnit.UnitLevel = 1;
                PlayerDataManager.Instance.PlayerData.UnitDatas.Add(newUnit);
                DatabaseReference unitRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_unitDatas");

                // ��� playerData.UnitDatas�� ������ DB������ ����
                for (int num = 0; num < PlayerDataManager.Instance.PlayerData.UnitDatas.Count; num++)
                {
                    // nowData�� PlayerUnitData ����
                    PlayerUnitData nowData = new PlayerUnitData();
                    nowData.UnitId = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitId;
                    nowData.UnitLevel = PlayerDataManager.Instance.PlayerData.UnitDatas[num].UnitLevel;

                    // ������ ��ġ�� ������� ������ ����
                    unitRoot.Child($"{num}/_unitId").SetValueAsync(nowData.UnitId);
                    unitRoot.Child($"{num}/_unitLevel").SetValueAsync(nowData.UnitLevel);
                }

                // ���� �Ϸ� �ؽ�Ʈ�� ���� �ڷ�ƾ ���
                int result = PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone] - gameObject.GetComponent<ShopChar>().price;
                PlayerDataManager.Instance.PlayerData.SetItem((int)E_Item.Stone, result);
                // ���� ���� �� ��� - UserId�ҷ�����
                DatabaseReference setItemRoot;
                setItemRoot = root.Child(BackendManager.Instance.Auth.CurrentUser.UserId).Child("_items/4");
                setItemRoot.SetValueAsync(result); // firebase �� ����
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
    /// ��í�� ĳ���� �̱� �� ������ ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator CharacterVideoR(GameObject gameObj)
    {
        shopSceneController.SoundPauseBGM();
        if (gameObj.GetComponent<ShopChar>())
        {
            GameObject obj = Instantiate(gameObj.GetComponent<ShopChar>().Video, gameObject.GetComponentInParent<ShopBtnManager>().SingleVideoContet);
            obj.SetActive(true);
            yield return new WaitUntil(() => obj.gameObject == false);
        }
        shopSceneController.DisableSingleImage();
        shopSceneController.DisabledGachaResultPanel();
        shopSceneController.SoundPlayBgm();
    }

}
