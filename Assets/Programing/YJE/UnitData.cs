using UnityEngine;


public enum UnitType { };
public enum UnitState { };
[CreateAssetMenu (menuName ="Unit")]
public class UnitData : ScriptableObject
{
    public UnitType type {  get; private set; }
    public UnitState state { get; private set; }
    public int damage { get; private set; }
    public int hp { get; private set; }
}
