using System.Collections.Generic;

public enum ETreeTraversalWay
{
    Unknown,
    /// <summary>
    /// 仅遍历直接子节点
    /// </summary>
    ChildrenOnly,
    /// <summary>
    /// 仅遍历所有叶子节点
    /// </summary>
    LeavesOnly,
    /// <summary>
    /// 随机遍历
    /// </summary>
    RandomTraversal,
    /// <summary>
    /// 逐层遍历 ( 广度优先遍历 )
    /// </summary>
    LayerorderTraversal,
    /// <summary>
    /// 先序遍历 ( 深度优先遍历 )
    /// </summary>
    PreorderTraversal,
    /// <summary>
    /// 后序遍历
    /// </summary>
    PostorderTraversal,
}

public interface ITree<T_Tag, T_Val> : IEnumerable<ITree<T_Tag, T_Val>>
{
    T_Tag Tag { get; set; }
    T_Val Value { get; set; }
    int ChildrenCount { get; }
    ETreeTraversalWay EnumeratorType { get; set; }

    ITree<T_Tag, T_Val> GetParent();
    ITree<T_Tag, T_Val> GetRoot();
    ITree<T_Tag, T_Val> GetChild(T_Tag tag);
    ITree<T_Tag, T_Val> GetChildInTree(T_Tag tag);
    /// <summary>
    /// 获取指定tag的所有子树. 如不指定tag, 则获取所有子树
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    ITree<T_Tag, T_Val>[] GetChildren();

    bool AddChild(T_Tag tag, T_Val value);

    bool RemoveChild(T_Tag tag);
    bool RemoveChildInTree(T_Tag tag);
    void ClearChildren();

    int GetBranchDepth();
    int GetDepthInRoot();
    ITree<T_Tag, T_Val>[] GetBranchRoad();

    ITree<T_Tag, T_Val> this[T_Tag tag]{get;set;}

}
