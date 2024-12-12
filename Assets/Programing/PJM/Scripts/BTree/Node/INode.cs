
public interface INode
{
    // 노드의 통일성을 위해 인터페이스를 사용
    
    public enum ENodeState // 노드의 상태
    {
        Running,
        Success,
        Failure,
    }
        
    /// <summary>
    /// 노드가 어떤 상태인지 반환함
    /// </summary>
    /// <returns></returns>
    public ENodeState Evaluate();
}

