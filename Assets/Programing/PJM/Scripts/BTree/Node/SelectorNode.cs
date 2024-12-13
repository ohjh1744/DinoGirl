using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : INode
{
    // 자식 노드 중, 처음으로 Success나 Running 상태를 가진 노드가 발생시 해당 노드까지 진행 후 멈춤
    private List<INode> _childs;
    private int _runningNodeIndex = -1;

    public SelectorNode(List<INode> childs)
    {
        _childs = childs;
    }

    /// <summary>
    /// 자식 상태에 따른 결과를 반환
    /// 자식 상태가 Running : Running 반환
    /// 자식 상태가 Success : Success 반환
    /// 자식 상태가 Failure : 다음 자식으로 이동
    /// </summary>
    /// <returns>ENodeState</returns>
    public INode.ENodeState Evaluate()
    {
        if (_childs == null)
            return INode.ENodeState.Failure;
        
        // 이전 탐색때 Running 중인 노드가 있었을 경우 해당 노드 우선 평가
        if (_runningNodeIndex != -1)
        {
            INode.ENodeState result = _childs[_runningNodeIndex].Evaluate();
            if (result == INode.ENodeState.Running)
            {
                return result;
            }
            else
            {
                // 평가 결과가 Running 이 아니었을 경우(Running 동작의 종료)
                _runningNodeIndex = -1;
                // Success? Failure? 선택필요
                return INode.ENodeState.Success; 
            }
            
        }

        for (int i = 0; i < _childs.Count; i++)
        {
            var result = _childs[i].Evaluate();
            
            switch (result)
            {
                // 앞선 업데이트에서 Running이 있었을 경우 failure를 반환하도록 추가해야함
                // 앞서 Running이 있었을 때 여기서 success를 반환하면 둘 다 수행하게 될 수도 있음
                
                case INode.ENodeState.Running :
                    _runningNodeIndex = i;
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    return INode.ENodeState.Success;
                case INode.ENodeState.Failure:
                    continue;
            }
        }
        return INode.ENodeState.Failure;
    }
}
