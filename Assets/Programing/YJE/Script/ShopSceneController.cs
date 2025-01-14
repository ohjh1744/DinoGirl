using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopSceneController : UIBInder
{
    ShopBtnManager shopBtnManager;
    [SerializeField] AudioClip shopBgm;
    [SerializeField] AudioClip buttonSfx;
    public AudioClip ButtonSfx { get { return buttonSfx; } set { buttonSfx = value; } }

    [Header("UI")]
    [SerializeField] GameObject resultCharPrefab; // 결과가 캐릭터인 경우 사용할 프리팹
    [SerializeField] GameObject resultItemPrefab; // 결과가 아이템인 경우 사용할 프리팹
    [SerializeField] RectTransform returnContent; // 중복캐릭터 아이템 반환 프리팹이 생성 될 위치
    [SerializeField] GameObject returnPrefab; // 중복캐릭터 아이템 반환 프리팹

    private void Awake()
    {
        shopBtnManager = gameObject.GetComponent<ShopBtnManager>();
        BindAll();
        SoundBgm();
        ShowBaseGachaPanel();
        SettingBtn();
    }

    private void OnEnable()
    {
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] += UpdateCoinUI;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] += UpdateDinoBloodUI;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] += UpdateBoneCrystalUI;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] += UpdateDinoStoneUI;
        PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Stone] += UpdateStoneUI;

    }
    private void OnDisable()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.PlayerData != null)
        {
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Coin] -= UpdateCoinUI;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoBlood] -= UpdateDinoBloodUI;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.BoneCrystal] -= UpdateBoneCrystalUI;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.DinoStone] -= UpdateDinoStoneUI;
            PlayerDataManager.Instance.PlayerData.OnItemChanged[(int)E_Item.Stone] -= UpdateStoneUI;
        }
    }

    /// <summary>
    /// BGM 재생 함수
    /// - ShopBtnManager.cs에서 사용
    /// </summary>
    public void SoundBgm()
    {
        SoundManager.Instance.PlayeBGM(shopBgm);
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
        UpdatePlayerData();
    }
    
    /// <summary>
    /// 각 Item 재화 상단 표시
    /// - 변동 시 계속 업데이트가 필요하므로 함수로 제작하여 사용
    /// </summary>
    public void UpdatePlayerData()
    {
        UpdateCoinUI(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Coin]);
        UpdateDinoBloodUI(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoBlood]);
        UpdateBoneCrystalUI(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.BoneCrystal]);
        UpdateDinoStoneUI(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.DinoStone]);
        UpdateStoneUI(PlayerDataManager.Instance.PlayerData.Items[(int)E_Item.Stone]);
    }
    
    private void UpdateCoinUI(int value)
    {
        GetUI<TextMeshProUGUI>("CoinText").SetText(value.ToString());
    }
    private void UpdateDinoBloodUI(int value)
    {
        GetUI<TextMeshProUGUI>("DinoBloodText").SetText(value.ToString());
    }
    private void UpdateBoneCrystalUI(int value)
    {
        GetUI<TextMeshProUGUI>("BoneCrystalText").SetText(value.ToString());
    }
    private void UpdateDinoStoneUI(int value)
    {
        GetUI<TextMeshProUGUI>("DinoStoneText").SetText(value.ToString());
    }
    private void UpdateStoneUI(int value)
    {
        GetUI<TextMeshProUGUI>("StoneText").SetText(value.ToString());
    }

    private void SettingBtn()
    {
        // 결과패널 버튼 클릭 시 패널 비활성화 함수 연결
        GetUI<Button>("SingleResultPanel").onClick.AddListener(shopBtnManager.OnDisableGachaPanelBtn);
        GetUI<Button>("TenResultPanel").onClick.AddListener(shopBtnManager.OnDisableGachaPanelBtn);
        // GachaBtn 스크립트의 각 버튼별 함수 연결
        GetUI<Button>("BaseSingleBtn").onClick.AddListener(shopBtnManager.OnBaseSingleBtn);
        GetUI<Button>("BaseSingleBtn").onClick.AddListener(() => SoundManager.Instance.PlaySFX(buttonSfx));
        GetUI<Button>("BaseTenBtn").onClick.AddListener(shopBtnManager.OnBaseTenBtn);
        GetUI<Button>("BaseTenBtn").onClick.AddListener(() => SoundManager.Instance.PlaySFX(buttonSfx));
        // Lobby로 돌아가기 버튼 함수 연동
        GetUI<Button>("BackBtn").onClick.AddListener(shopBtnManager.OnBackToRobby);
        GetUI<Button>("BackBtn").onClick.AddListener(() => SoundManager.Instance.PlaySFX(buttonSfx));
        // 상점 활성화 버튼 연결
        GetUI<Button>("ChangeShopBtn").onClick.AddListener(shopBtnManager.OnShopBtn);
        GetUI<Button>("ChangeShopBtn").onClick.AddListener(() => SoundManager.Instance.PlaySFX(buttonSfx));
        // 기본 Gacha 변경 버튼 함수 연동
        GetUI<Button>("ChangeBaseGachaBtn").onClick.AddListener(shopBtnManager.OnBaseGachaBtn);
        GetUI<Button>("ChangeBaseGachaBtn").onClick.AddListener(() => SoundManager.Instance.PlaySFX(buttonSfx));
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
        // 상점NPC 활성화
        GetUI<Image>("ShopCharacter").gameObject.SetActive(true);
    }
    public void ShowShopPanel()
    {
        GetUI<Image>("BaseGachaPanel").gameObject.SetActive(false);
        GetUI<Image>("ShopPanel").gameObject.SetActive(true);
        // 상점NPC 활성화
        GetUI<Image>("ShopCharacter").gameObject.SetActive(true);
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

        SoundManager.Instance.StopBGM();

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

        SoundManager.Instance.StopBGM();
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
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowBuyOverlapPopUp()
    {
        GetUI<Image>("BuyPopUp").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("OverlapBuyPopUpText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("OverlapGachaPopUpText").gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        GetUI<Image>("BuyPopUp").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("OverlapBuyPopUpText").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("OverlapGachaPopUpText").gameObject.SetActive(false);
    }

    /// <summary>
    /// 가챠 시도 시 재화부족 안내창
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowGachaOverlapPopUp()
    {
        GetUI<Image>("BuyPopUp").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("OverlapGachaPopUpText").gameObject.SetActive(true);
        GetUI<TextMeshProUGUI>("OverlapBuyPopUpText").gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        GetUI<Image>("BuyPopUp").gameObject.SetActive(false);
        GetUI<TextMeshProUGUI>("OverlapGachaPopUpText").gameObject.SetActive(false);
    }
}
