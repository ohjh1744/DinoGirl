using System;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] ShopMakeStart shopMakeStart;
    [SerializeField] ShopSceneController shopSceneController;
    private bool isLoading = false;
    private event Action OnStartSetting;

    private void OnEnable()
    {
        OnStartSetting += shopMakeStart.MakeGachaList; // 사용할 가차 리스트 완성
        OnStartSetting += shopMakeStart.MakeItemDic; // 사용할 itemDic 완성
        OnStartSetting += shopMakeStart.MakeCharDic; // 사용할 charDic 완성
        OnStartSetting += shopMakeStart.MakeCharReturnItemDic; // 사용할 charReturnItemDic 완성
        OnStartSetting += shopMakeStart.ShopCharMaker; // 상점 캐릭터 구매목록 완성
        OnStartSetting += shopSceneController.SettingStartUI; // 시작 UI 설정
    }
    private void OnDisable()
    {
        // GameObject 비활성화 시 이벤트 정리
        OnStartSetting -= shopMakeStart.MakeGachaList;
        OnStartSetting -= shopMakeStart.MakeItemDic;
        OnStartSetting -= shopMakeStart.MakeCharDic;
        OnStartSetting -= shopMakeStart.MakeCharReturnItemDic;
        OnStartSetting -= shopMakeStart.ShopCharMaker;
        OnStartSetting -= shopSceneController.SettingStartUI;
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
