using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner
{
    private BaseNode _rootNode;

    public BehaviourTreeRunner(BaseNode rootNode)
    {
        this._rootNode = rootNode;
    }

    public void Operate()
    {
        _rootNode.Evaluate();
    }
}
