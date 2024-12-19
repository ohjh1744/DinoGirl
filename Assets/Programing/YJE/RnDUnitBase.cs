using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
/// <summary>
/// CSV에서 불러오는 Unit의 레벨 1 기본 데이터를 저장하기 위한 클래스
/// id / name / 공격력 / 방어력 / 체력 / 회복량 / 명중률 / 치명타 / 코스트 회복률 / 회피률 / 사거리 / type / 레벨업 계수
/// 
/// </summary>

public enum UnitType
{
    water, fire, earth, grass
    }
public class RnDUnitBase : RnDUnit
{
    public int id { get; private set; }
    public string name { get; private set; }
    public int atk { get; private set; }
    public int def { get; private set; }
    public int hp { get; private set; }
    public int healHp { get; private set; }
    public int hit { get; private set; }
    public int critical { get; private set; }
    public int healCost { get; private set; }
    public int dodge { get; private set; }
    public int sight { get; private set; }
    public UnitType type { get; private set; }
    public double levelUp { get; private set; }

}
