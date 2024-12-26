using UnityEngine;

/// <summary>
/// 임시 test를 위한 스크립트
/// - DataManager가 다 불러와진 상태에서 로딩되도록 하기 위한 스크립트
/// - 추후 로비씬과 연동 삭제 가능
/// </summary>
public class DBTest : MonoBehaviour
{
    // LotterySetting
    [SerializeField] GameObject target;
    private void Update()
    {
        if (CsvDataManager.Instance.IsLoad)
        {
            Debug.Log("변동");
            target.SetActive(true); // LotterySetting.cs 시작
            gameObject.SetActive(false); // Tester 비활성화
        }
    }
}
