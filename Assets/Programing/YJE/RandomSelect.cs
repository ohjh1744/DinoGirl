using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelect : MonoBehaviour
{
    /// <summary>
    /// 전체 뽑기 횟수와 알맞은 GachaId의 List를 받아서
    /// 그 횟수 만큼 아이템(캐릭터)를 확률에 맞게 생성 후 저장
    /// Lottery가 가진 probability에 맞춰서 ID 하나 반환
    /// </summary>
    /// <param name="count"></param>
    /// <param name="lotteries"></param>
    public void RandomTake(int count, List<Lottery> lotteries)
    {
        // TODO : lotteries가 가지고 있는 Lottery 정보에서의 가중치를 설정하여 랜덤으로
        // count 횟수 만큼 Lottery를 출력
        // 출력된 Lottery의 ID를 각 List에 저장
        // ItemG 혹은 CharaterG로 오브젝트풀로 사용할 프리팹을 불러와 정보를 저장하고
        // UI로 출력
    }

}
