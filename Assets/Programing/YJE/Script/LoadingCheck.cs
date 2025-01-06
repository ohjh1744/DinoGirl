using System;
using UnityEngine;

/// <summary>
/// LoadingPanel의 활성화 여부를 판단하는 스크립트
/// - GachaScene이 시작되면 활성화 상태로 시작
/// - isLoading = true로 변경되면 LoadingCheck Panel 비활성화
/// - 테스트 용도로 싱글톤인 DataManager의 Data와 PlayerDataManager에서 PlayerData가 제대로 불러와 진 것을 확인 후
/// - (실제 사용) 각종 Setting을 하는 함수 실행
/// - TODO : 병합 시에는 테스트 부분 주석처리 필수
/// - 모두 종료 후 BaseGacahPanel / ChangeBaseGachaBtn / ChangeEventGacahBtn 활성화하고 LoadingPanel을 비활성화
/// </summary>
public class LoadingCheck : MonoBehaviour
{
    [SerializeField] GachaSceneController gachaSceneController;
    private bool isLoading = false;

    // GachaSceneController에서 Scene를 시작하기 전 필요한 Setting을 하는 이벤트 제작
    private event Action OnStartSetting;

    private void OnEnable()
    {
        OnStartSetting += gachaSceneController.SettingStartUI; // PlayerData까지 전부 불러온 후 재화 설정
        OnStartSetting += gachaSceneController.MakeGachaList; // 그룹별로 뽑기 List Setting
        OnStartSetting += gachaSceneController.MakeItemDic; // 사용하는 Item을 GachaItem형식의 Dictionary Setting
        OnStartSetting += gachaSceneController.MakeCharDic; // 사용하는 캐릭터를 GachaChar형식의 Dictionary Setting
        OnStartSetting += gachaSceneController.MakeCharReturnItemDic; // 중복 캐릭터 뽑기 시 GachaItemReturn형식의 Dictionary Setting
        OnStartSetting += gachaSceneController.SettingBtn; // 각 버튼에 알맞은 함수 할당
    }
    private void OnDisable()
    {
        // GameObject 비활성화 시 이벤트 정리
        OnStartSetting -= gachaSceneController.SettingStartUI;
        OnStartSetting -= gachaSceneController.MakeGachaList;
        OnStartSetting -= gachaSceneController.MakeItemDic;
        OnStartSetting -= gachaSceneController.MakeCharDic;
        OnStartSetting -= gachaSceneController.MakeCharReturnItemDic;
        OnStartSetting -= gachaSceneController.SettingBtn;
    }

    private void Update()
    {
        // CsvDataManger의 로딩이 완료되었는지 확인 - 통합테스트 시 if문 필요 x
        // Setting 완료를 확인해서 LoadingCheck로 변경
        if (CsvDataManager.Instance.IsLoad)
        {
            if (!isLoading)
            {
                OnStartSetting?.Invoke();
                isLoading = true;
                gameObject.SetActive(false);
            }
            else
                gameObject.SetActive(false);
        }
        else
        {
            return;
        }
    }

}
