using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelNode : BaseNode
{
    public enum SuccessPolicy { AllSuccess, OneSuccess }
    public enum FailurePolicy { AllFailure, OneFailure }

    private List<BaseNode> _childs;
    private SuccessPolicy _successPolicy;
    private FailurePolicy _failurePolicy;

    public ParallelNode(List<BaseNode> childs, SuccessPolicy successPolicy, FailurePolicy failurePolicy)
    {
        _childs = childs;
        _successPolicy = successPolicy;
        _failurePolicy = failurePolicy;
    }

    public override ENodeState Evaluate()
    {
        int successCount = 0;
        int failureCount = 0;

        foreach (var child in _childs) 
        {
            var result = child.Evaluate();

            if (result == ENodeState.Success)
                successCount++;
            else if (result == ENodeState.Failure)
                failureCount++;
        }

        if (_successPolicy == SuccessPolicy.AllSuccess && successCount == _childs.Count)
            return ENodeState.Success;

        if (_successPolicy == SuccessPolicy.OneSuccess && successCount > 0)
            return ENodeState.Success;

        if (_failurePolicy == FailurePolicy.AllFailure && failureCount == _childs.Count)
            return ENodeState.Failure;

        if (_failurePolicy == FailurePolicy.OneFailure && failureCount > 0)
            return ENodeState.Failure;

        return ENodeState.Running;
    }

    public override void ResetNode()
    {
        foreach (var child in _childs)
        {
            child.ResetNode();
        }
    }
}
