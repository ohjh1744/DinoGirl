using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBossUnitController : BaseUnitController
{
    [SerializeField] private Skill _bossSkill;
    protected override BaseNode SetBTree()
    {
        throw new System.NotImplementedException();
    }
}
