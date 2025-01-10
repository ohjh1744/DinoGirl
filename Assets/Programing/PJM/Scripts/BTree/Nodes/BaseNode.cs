using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode
{
    public enum ENodeState
    {
        Running,
        Success,
        Failure,
    }

    public abstract ENodeState Evaluate();

    public virtual void ResetNode() { }
}
