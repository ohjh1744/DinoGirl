using UnityEngine;

/// <summary>
/// LoadingPanel의 활성화 여부를 판단하는 스크립트
/// - GachaScene이 시작되면 활성화 상태로 시작
/// - 테스트 용도로 싱글톤인 DataManager의 Data와 PlayerDataManager에서 PlayerData가 제대로 불러와 진 것을 확인 후
/// - (실제 사용) 각종 Setting을 하는 함수 실행
/// - TODO : 병합 시에는 테스트 부분 주석처리 필수
/// - 모두 종료 후 BaseGacahPanel / ChangeBaseGachaBtn / ChangeEventGacahBtn 활성화하고 LoadingPanel을 비활성화
/// </summary>
public class LoadingCheck : MonoBehaviour
{
    [SerializeField] GachaSceneController gachaSceneController;
    private void Update()
    {
        // TODO : 임의의 테스트 용 주석처리 필요
            if (CsvDataManager.Instance.IsLoad)
            {
                if (gachaSceneController.IsLoading) // 최종으로 남을 로딩 부분
                {
                    // TODO : 이벤트로 IsLoadingClear를 설정하여 실행시키기
                    // - BaseGachaPanel을 활성화하고
                    // - ChangeBaseGachaBtn 활성화
                    // - ChangeEventGachaBtn 활성화
                    // - ShopCharacter 활성화
                    gameObject.SetActive(false);
                }
            }
        else
        {
            return;
        }
    }

}
