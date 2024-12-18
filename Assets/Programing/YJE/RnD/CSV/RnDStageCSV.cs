using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RnDStageCSV
{
    // 사용할 Stage의 변수들
    // CSV파일에서 불러올 제목에 맞춰서 알맞은 자료형과 함께 생성
    public int id {  get; set; }
    public string stageName { get; set; }
    public int timeLimit { get; set; }
    // public bool isCleared { get; set; }
    public int monsterCount { get; set; }
    public int monsterPos { get; set; }
}
