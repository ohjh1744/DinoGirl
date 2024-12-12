using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner
{
    private INode _rootNode;

    public BehaviourTreeRunner(INode rootNode)
    {
        this._rootNode = rootNode;
    }

    public void Operate()
    {
        _rootNode.Evaluate();
    }
}
