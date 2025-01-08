using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseUnitController : BaseUnitController
{
    
    protected override BaseNode SetBTree()
    {
        Debug.LogWarning("자식에서 구현하세요 Btree");
        throw new System.NotImplementedException();
    }
}
