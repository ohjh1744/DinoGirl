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

    private RectTransform singleResultContent; // 1���� ��� ���� �������� ���� �� ��ġ
    private RectTransform tenResultContent; // 10���� ��� ���� �������� ���� �� ��ġ
    private RectTransform returnContent; // �ߺ�ĳ���� ������ ��ȯ �������� ���� �� ��ġ
    private RectTransform singleVideoContent; // 1���� ��� ���� �������� ���� �� ��ġ
    public RectTransform SingleVideoContet { get {  return singleVideoContent; } set { singleResultContent = value; } }
    private RectTransform tenVideoContent; // 10���� ��� ���� �������� ���� �� ��ġ

    private GameObject resultCharPrefab; // ����� ĳ������ ��� ����� ������
    private GameObject resultItemPrefab; // ����� �������� ��� ����� ������
    private GameObject returnPrefab; // �ߺ�ĳ���� ������ ��ȯ ������

    private List<Gacha> baseGachaList = new List<Gacha>(); // �⺻ �̱� List
    private List<GameObject> resultList = new List<GameObject>(); // �̱��� ����� ����

    private Dictionary<int, Item> itemDic = new Dictionary<int, Item>(); // ������ Dictionary
    private Dictionary<int, ShopChar> charDic = new Dictionary<int, ShopChar>(); // ĳ���� Dictionary
    private Dictionary<int, GachaReturn> charReturnItemDic = new Dictionary<int, GachaReturn>(); // �ߺ� ĳ���� ��ȯ ������ Dictionary

    [SerializeField] int gachaCost;
    public int GachaCost { get { return gachaCost; } set { gachaCost = value; } }
    [SerializeField] string gachaCostItem;

    private void Awake()
    {
        shopSceneController = gameObject.GetComponent<ShopSceneController>();
        shopMakeStart = gameObject.GetComponent<ShopMakeStart>();
        valueChange = gameObject.GetComponent<ValueChange>();

        // �� ������ ����
        resultCharPrefab = Resources.Load<GameObject>("Prefabs/ResultCharacter");
        resultItemPrefab = Resources.Load<GameObject>("Prefabs/ResultItem");
        returnPrefab = Resources.Load<GameObject>("Prefabs/ReturnItem");

        // �� ������ ���� ��ġ ����
        singleResultContent = shopSceneController.GetUI<RectTransform>("SingleResultGrid");
        tenResultContent = shopSceneController.GetUI<RectTransform>("TenResultGrid");
        singleVideoContent = shopSceneController.GetUI<RectTransform>("SingleResultPanel");
        tenVideoContent = shopSceneController.GetUI<RectTransform>("TenResultPanel");
    }

    /// <summary>
    /// ��� �г� ��Ȱ��ȭ ��
    /// resultList �� �ʱ�ȭ
    /// </summary>
   private void ClearResultList()
    {
        for (int i = 0; i < resultList.Count; i++)
        {
            Destroy(resultList[i]);
        }
        resultList.Clear();
    }

    /// <summary>
    /// �⺻ 1ȸ ��í ��ư
    /// </summary>
    public void OnBaseSingleBtn()
    {
        baseGachaList = shopMakeStart.BaseGachaList;
        GameObject resultUI = null;
        // �⺻ �÷��̾��� ��ȭ DinoStone(3)�� 100 �̻��� ��쿡�� ����
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost)
        {
            // baseGachaList�� ��ü Probability�� �ջ��� ���ϱ�
            int total = 0;
            foreach (Gacha gacha in baseGachaList)
            {
                total += gacha.Probability;
            }
            int weight = 0;
            int selectNum = 0;
            selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f)); // ���� ���� �̱�
            shopSceneController.ShowSingleResultPanel(); // 1���� ��� �г� Ȱ��ȭ

            for (int i = 0; i < baseGachaList.Count; i++)
            {
                weight += baseGachaList[i].Probability;
                if (selectNum <= weight) // ����ġ�� ���ڸ� ��
                {
                    // �����۰� ĳ���Ϳ� ���� ����� ���
                    // GachaSceneController.cs�� GachaResultUI()�� ��ȯ�� GameObject�� resultList�� ����
                    resultUI = GachaResultUI(baseGachaList, i, 1);
                    resultList.Add(resultUI);
                    StartCoroutine(CharacterVideoR(resultUI)); // ��í ��ƾ ����
                    break;
                }
            }

            // �������� �÷��̾��� ������ �� ����
            // firebase �⺻ UserData ��Ʈ
            DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData");
            // �̱⿡ ������ ��ȭ�� PlayerData ����
            valueChange.SendChangeValue(gachaCostItem, gachaCost, false, root, PlayerDataManager.Instance.PlayerData);
            // ��� ����Ʈ�� ���� �˸��� �����۰� ĳ���� ��ȯ�� Ȯ���ϰ� ������ ����
            valueChange.CheckCharId(resultList, root, PlayerDataManager.Instance.PlayerData);

            StopCoroutine(CharacterVideoR(resultUI)); // ��í ��ƾ ����
        }
        else
        {
            StartCoroutine(shopSceneController.ShowGachaOverlapPopUp());
            StopCoroutine(shopSceneController.ShowGachaOverlapPopUp());
        }
    }

    /// <summary>
    /// �⺻ 10ȸ ��í ��ư
    /// </summary>
    public void OnBaseTenBtn()
    {
        baseGachaList = shopMakeStart.BaseGachaList;
        GameObject resultUI = null;
        // �⺻ �÷��̾��� ��ȭ DinoStone(3)�� 1000 �̻��� ��쿡�� ����
        if (PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone] >= gachaCost * 10)
        {
            // baseGachaList�� ��ü Probability�� �ջ��� ���ϱ�
            int total = 0;
            foreach (Gacha gacha in baseGachaList)
            {
                total += gacha.Probability;
            }
            shopSceneController.ShowTenResultPanel(); // 10���� ����г� Ȱ��ȭ

            int weight = 0; // ���� ��ġ�� ����ġ
            int selectNum = 0; // ������ ���� ��ȣ
            int count = 0; // �� 10���� ȸ���� ī���� �ϴ� ����
            do
            {
                selectNum = Mathf.RoundToInt(total * Random.Range(0.0f, 1.0f));
                // ��í�� ����Ʈ�� Ƚ�� ��ŭ �ݺ��ϸ� ����ġ�� �ش��ϴ� ��� ���
                for (int i = 0; i < baseGachaList.Count; i++)
                {
                    weight += baseGachaList[i].Probability;
                    if (selectNum <= weight)
                    {
                        // �����۰� ĳ���Ϳ� ���� ����� ���
                        // GachaSceneController.cs�� GachaResultUI()�� ��ȯ�� GameObject�� resultList�� ����
                        resultUI = GachaResultUI(baseGachaList, i, 10);
                        resultList.Add(resultUI);
                        count++;
                        weight = 0;
                        break;
                    }
                }
            } while (count < 10);
            StartCoroutine(CharacterTenVideoR());
            // �̱⿡ ����� ��ȭ�� PlayerData ����
            DatabaseReference root = BackendManager.Instance.Database.RootReference.Child("UserData");
            valueChange.SendChangeValue(gachaCostItem, gachaCost * 10, false, root, PlayerDataManager.Instance.PlayerData);
            // ��� ����Ʈ�� ���� �˸��� �����۰� ĳ���� ��ȯ�� Ȯ���ϰ� ������ ����
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
    /// ������� �г� ��ư
    /// </summary>
    public void OnDisableGachaPanelBtn()
    {
        ClearResultList();
        shopSceneController.SoundPlayBgm();
        shopSceneController.DisabledGachaResultPanel();
    }

    /// <summary>
    /// �κ�� ���ư��� ��ư�� ����
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
    /// �̱� ���� ��
    /// GachaList�� index���� �޾Ƽ� �ش��ϴ� ����� ������/ĳ�������� �Ǵ�
    /// �з��� ���� Prefab���� GameObject�� ����
    /// �˸��� ����� UI�� ���
    /// GameObject�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="GachaList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameObject GachaResultUI(List<Gacha> GachaList, int index, int count)
    {
        charDic = shopMakeStart.CharDic;
        itemDic = shopMakeStart.ItemDic;

        if (count == 1) // 1ȸ �̱� ��
        {
            switch (GachaList[index].Check)
            {
                case 0: // ��ȯ�� ĳ������ ���
                    ShopChar resultChar = charDic[GachaList[index].CharId];
                    resultChar.Amount = GachaList[index].Count;
                    GameObject resultCharUI = Instantiate(resultCharPrefab, singleResultContent);
                    resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                    return resultCharUI;
                case 1: // ��ȯ�� �������� ���
                    Item result = itemDic[GachaList[index].ItemId]; // GachaItem ����
                    result.Amount = GachaList[index].Count; // GachaItem�� Amount�� ������ �������� ����

                    GameObject resultUI = Instantiate(resultItemPrefab, singleResultContent); // Prefab���� ������ ��ġ�� ���� - �Ѱ�
                    resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem�� ������ UI Setting
                    return resultUI;
                default:
                    return null;
            }
        }
        else if(count == 10) // 10ȸ �̱� ��
        {
            switch (GachaList[index].Check)
            {
                case 0: // ��ȯ�� ĳ������ ���
                    ShopChar resultChar = charDic[GachaList[index].CharId];
                    resultChar.Amount = GachaList[index].Count;
                    GameObject resultCharUI = Instantiate(resultCharPrefab, tenResultContent);
                    resultCharUI = resultChar.SetGachaCharUI(resultChar, resultCharUI);
                    return resultCharUI;
                case 1: // ��ȯ�� �������� ���
                    Item result = itemDic[GachaList[index].ItemId]; // GachaItem ����
                    result.Amount = GachaList[index].Count; // GachaItem�� Amount�� ������ �������� ����

                    GameObject resultUI = Instantiate(resultItemPrefab, tenResultContent); // Prefab���� ������ ��ġ�� ���� - ����
                    resultUI = result.SetGachaItemUI(result, resultUI); // GachaItem�� ������ UI Setting

                    return resultUI;
                default:
                    return null;
            }
        }
        return null;
    }

    /// <summary>
    /// ��í������ Character�� �ߺ��� Ȯ���� �� Character�� �̹� �����ϰ� �ִ� ���
    /// ���������� ��ȯ�� �������� �˸��� UI�� ���
    /// </summary>
    /// <param name="UnitId"></param>
    /// <param name="resultListObj"></param>
    public GameObject CharReturnItem(int UnitId, GameObject resultListObj)
    {
        itemDic = shopMakeStart.ItemDic;
        charReturnItemDic = shopMakeStart.CharReturnItemDic;
        returnContent = resultListObj.transform.GetChild(4).GetComponent<RectTransform>();
        GameObject resultObjUI = Instantiate(returnPrefab, returnContent); // �� ��ġ�� ���ο� ���������� ����

        Item resultItem = resultObjUI.gameObject.GetComponent<Item>(); // �����տ� ���� ����
        resultItem.ItemId = charReturnItemDic[UnitId].ItemId;
        resultItem.Amount = charReturnItemDic[UnitId].Count;
        resultItem.ItemName = itemDic[charReturnItemDic[UnitId].ItemId].ItemName;
        resultItem.ItemImage = itemDic[charReturnItemDic[UnitId].ItemId].ItemImage;

        resultObjUI = resultItem.SetGachaReturnItemUI(resultItem, resultObjUI);

        return resultObjUI;
    }

    /// <summary>
    /// ��í�� ĳ���� �̱� �� ������ ���� �ڷ�ƾ
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
    /// ��í�� ĳ���� 10ȸ �̱� �� ������ ���� �ڷ�ƾ
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
