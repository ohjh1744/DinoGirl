using UnityEngine;

[CreateAssetMenu(menuName = "Unit")]
public class UnitSO : ScriptableObject
{
    public int charId;
    public int skillId;
    public string name;
    public float hp;
    public float ATK;
    public float DEF;
    public float coolTime;
}
