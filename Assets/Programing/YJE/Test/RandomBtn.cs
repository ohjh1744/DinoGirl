using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBtn : MonoBehaviour
{
    private void Awake()
    {
        // TODO : 버튼에 10연차, 1연차 함수 연결
        // 각 버튼에 LotterSetting.cs에서 만든 GachaID별 List를 적용 - 1연차 / 10연차 세트로
        // 각 함수에서 뽑기를 진행 (공통)
    }

    /// <summary>
    /// GachaID : 200000
    /// </summary>
    private void SingleTakeBtn()
    {
        // TODO : 단일 뽑기 진행
        // 뽑기를 표현하기 위한 UI 패널 활성화
        // RandomSelect.cs의 RandomTake(int count, List<Lottery> lotteries) 함수 실행
        // 반환된 ID 값에 알맞는 ItemG or CharaterG를 생성 - UI패널 아래에 생성하여 패널 종료 시 사라지도록 수행
    }
    private void TenTakeBtn()
    {

    }
}
