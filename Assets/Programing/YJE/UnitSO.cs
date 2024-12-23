using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본적인 각 캐릭터의 기본 스탯(Lv.1기준)을 저장하는 Scriptable Object
/// - 엑셀 csv 파일에서 가져온 값을 저장할지
/// - 캐릭터 별로 작성한 에셋 파일을 적용시킬지
/// 추후 논의가 필요
/// </summary>
[CreateAssetMenu(menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public int hp;
    public int damage;
    public int skillCode;
}
