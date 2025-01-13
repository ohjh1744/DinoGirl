using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSceneController : UIBInder
{
    ShopBtnManager shopBtnManager;

    [Header("UI")]
    [SerializeField] GameObject resultCharPrefab; // 결과가 캐릭터인 경우 사용할 프리팹
    [SerializeField] GameObject resultItemPrefab; // 결과가 아이템인 경우 사용할 프리팹
    [SerializeField] RectTransform returnContent; // 중복캐릭터 아이템 반환 프리팹이 생성 될 위치
    [SerializeField] GameObject returnPrefab; // 중복캐릭터 아이템 반환 프리팹

    private void Awake()
    {
        shopBtnManager = gameObject.GetComponent<ShopBtnManager>();
        BindAll();
        ShowBaseGachaPanel();
        SettingBtn();
    }

    /// <summary>
    /// 시작 시 버튼의 문구 설정
    /// - 버튼의 문구 변경 가능
    //  - LoadingCheck.cs에서 이벤트로 사용
    /// </summary>
    public void SettingStartUI()
    {
        // 각 Button 텍스트 설정
        GetUI<TextMeshProUGUI>("BaseSingleText").SetText("1회 뽑기");
        GetUI<TextMeshProUGUI>("BaseTenText").SetText("10회 뽑기");
        GetUI<TextMeshProUGUI>("ChangeBaseGacahText").SetText("상설");
        GetUI<TextMeshProUGUI>("ChangeShopText").SetText("상점");
        GetUI<TextMeshProUGUI>("GachaSingleCostText").SetText($"{shopBtnManager.GachaCost}");
        GetUI<TextMeshProUGUI>("GachaTenCostText").SetText($"{shopBtnManager.GachaCost * 10}");
        UpdatePlayerUI();
    }

    /// <summary>
    /// 각 Item 재화 상단 표시
    /// - 변동 시 계속 업데이트가 필요하므로 함수로 제작하여 사용
    /// </summary>
    public void UpdatePlayerUI()
    {
        GetUI<TextMeshProUGUI>("CoinText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin].ToString());
        GetUI<TextMeshProUGUI>("DinoBloodText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood].ToString());
        GetUI<TextMeshProUGUI>("BoneCrystalText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal].ToString());
        GetUI<TextMeshProUGUI>("DinoStoneText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone].ToString());
        GetUI<TextMeshProUGUI>("StoneText").SetText(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone].ToString());
    }

    private void SettingBtn()
    {
        // 결과패널 버튼 클릭 시 패널 비활성화 함수 연결
        GetUI<Button>("SingleResultPanel").onClick.AddListener(shopBtnManager.OnDisableGachaPanelBtn);
        GetUI<Button>("TenResultPanel").onClick.AddListener(shopBtnManager.OnDisableGachaPanelBtn);
        // GachaBtn 스크립트의 각 버튼별 함수 연결
        GetUI<Button>("BaseSingleBtn").onClick.AddListener(shopBtnManager.OnBaseSingleBtn);
        GetUI<Button>("BaseTenBtn").onClick.AddListener(shopBtnManager.OnBaseTenBtn);
        // Lobby로 돌아가기 버튼 함수 연동
        GetUI<Button>("BackBtn").onClick.AddListener(shopBtnManager.OnBackToRobby);
        // 상점 활성화 버튼 연결
        GetUI<Button>("ChangeShopBtn").onClick.AddListener(shopBtnManager.OnShopBtn);
        // Gacha 종류 변경 버튼 함수 연동
        GetUI<Button>("ChangeBaseGachaBtn").onClick.AddListener(shopBtnManager.OnBaseGachaBtn);
    }

    /// <summary>
    /// BaseGachaPanel 활성화
    /// </summary>
    public void ShowBaseGachaPanel()
    {
        // 기본 패널 활성화
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(true);
        // 돌아가는 버튼 활성화
        GetUI<Image>("BackBtn").gameObject.SetActive(true);
        // 상점 캐릭터 활성화
        GetUI<Image>("ShopCharacter").gameObject.SetActive(true);
        // 가챠 선택 버튼 활성화
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(true);
        // 상점 패널 비활성화
        GetUI<Image>("ShopPanel").gameObject.SetActive(false);
        // 가챠 결과 패널 비활성화
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        // 로딩 패널 비활성화
        GetUI<Image>("LoadingPanel").gameObject.SetActive(false);
        // 중복 구매 팝업
        GetUI<Image>("BuyPopUp").gameObject.SetActive(false);
    }
    public void ShowShopPanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(false);
        GetUI<Image>("ShopPanel").gameObject.SetActive(true);
    }
    /// <summary>
    /// 가챠 1연차 패널 활성화
    /// </summary>
    public void ShowSingleResultPanel()
    {
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(true);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(true);
        GetUI<Image>("SingleImage").gameObject.SetActive(true);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(false);
    }
    public void DisableSingleImage()
    {
        GetUI<Image>("SingleImage").gameObject.SetActive(false);
    }
    public void ShowTenResultPanel()
    {
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(true);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(false);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(true);
        GetUI<Image>("TenImage").gameObject.SetActive(true);
    }
    public void DisableTenImage()
    {
        GetUI<Image>("TenImage").gameObject.SetActive(false);
    }
    /// <summary>
    /// GachaResultPanel 비활성화
    /// - 결과 패널을 비활성화
    /// </summary>
    public void DisabledGachaResultPanel()
    {
        // 기본 뽑기 종류 변경 버튼 활성화
        GetUI<Image>("ChangeBaseGachaBtn").gameObject.SetActive(true);
        // 결과 패널 비활성화
        GetUI<Image>("GachaResultPanel").gameObject.SetActive(false);
        GetUI<Image>("SingleResultPanel").gameObject.SetActive(false);
        GetUI<Image>("TenResultPanel").gameObject.SetActive(false);
        // 각 아이템 재화 Text 활성화
        GetUI<TextMeshProUGUI>("CoinText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("DinoBloodText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("BoneCrystalText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("DinoStoneText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("StoneText").gameObject.SetActive(true);
        // 상점 캐릭터 활성화
        GetUI<Image>("ShopCharacter").gameObject.SetActive(true);
        // 돌아가는 버튼 활성화
        GetUI<Image>("BackBtn").gameObject.SetActive(true);

    }

    /// <summary>
    /// 상점 구매 시 중복 구매 방지 팝업창
    /// ShopChar.cs에서 사용
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowBuyOverlapPopUp()
    {
        Debug.Log("팝업창 코루틴 시작");
        GetUI<Image>("BuyPopUp").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("OverlapPopUpText").gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        GetUI<Image>("BuyPopUp").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("OverlapPopUpText").gameObject.SetActive(false);
    }
}
