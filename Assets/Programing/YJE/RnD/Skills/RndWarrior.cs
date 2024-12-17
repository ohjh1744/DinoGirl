using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndWarrior : MonoBehaviour
{
    public RndWarriorSkill warriorSkill;

    private void Start()
    {
        warriorSkill.DoSkill();
    }
}
