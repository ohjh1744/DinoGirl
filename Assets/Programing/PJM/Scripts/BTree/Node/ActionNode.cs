using System;

public sealed class ActionNode : INode
{
    // 실제로 행위를 하는 노드
    // Func 델리게이트를 사용, 행위를 전달받아 실행
    
    private Func<INode.ENodeState> _action;
    
    public ActionNode(Func<INode.ENodeState> action)
    {
        _action = action;
    }
    public INode.ENodeState Evaluate() => _action?.Invoke() ?? INode.ENodeState.Failure;
}
