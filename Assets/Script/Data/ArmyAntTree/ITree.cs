using System.Collections.Generic;

enum ETreeTraversalWay
{
    RandomTraversal,    // 随机遍历
    LayerorderTraversal,// 逐层遍历 ( 广度优先遍历 )
    PreorderTraversal,  // 先序遍历 ( 深度优先遍历 )
    PostorderTraversal, // 后序遍历
}

interface ITree<T_Tag, T_Val> : IEnumerable<KeyValuePair<T_Tag, T_Val>>
{
    T_Tag Tag { get; set; }
    T_Val Value { get; set; }

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

    ITree<T_Tag, T_Val> this[T_Tag tag]{get;set;}

}
