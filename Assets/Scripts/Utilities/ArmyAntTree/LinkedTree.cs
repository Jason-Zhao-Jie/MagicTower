

using System.Collections;
using System.Collections.Generic;

public class LinkedTree<T_Tag, T_Val> : ITree<T_Tag, T_Val> {
    // 实现接口
    public LinkedTree(T_Tag tag, T_Val val, LinkedTree<T_Tag, T_Val> parent) {
        Value = val;
        if (parent != null) {
            parent[tag] = this;
        } else {
            Tag = tag;
        }
        IsCheckTag = false;
    }

    public ITree<T_Tag, T_Val> this[T_Tag tag] {
        get {
            return children[tag];
        }
        set {
            if (value.GetType() != typeof(LinkedTree<T_Tag, T_Val>))
                throw new System.ArgumentException("The child of the tree you are setting is not a linked tree!", "tag");
            children[tag] = ToLinkedTree(value);
            children[tag].parent = this;
            children[tag].Tag = tag;
            if (!CheckTreeRef(children[tag], true)) {
                throw new System.ArgumentException("The parent has a node as this as its parent or one of grand parents !");
            }
        }
    }

    public T_Tag Tag { get; set; }

    public T_Val Value { get; set; }

    public int ChildrenCount { get { return children.Count; } }
    public string Json {
        get {
            string ret = "{";
            foreach(var elem in children) {
                ret += '"' + elem.Key.ToString() + '"' + ':' + elem.Value.ToString() + ',';
            }
            ret.Trim(',');
            return ret + '}';
        }
    }
    public override string ToString() {
        return Json;
    }

    public ETreeTraversalWay EnumeratorType { get; set; }

    public bool AddChild(T_Tag tag, T_Val value) {
        if (IsCheckTag && (GetRoot().Tag.Equals(tag) || GetRoot().GetChildInTree(tag) != null))
            return false;
        if (children.ContainsKey(tag))
            return false;
        new LinkedTree<T_Tag, T_Val>(tag, value, this);
        return true;
    }

    public void ClearChildren() {
        foreach (var elem in children) {
            elem.Value.parent = null;
        }
        children.Clear();
    }

    public ITree<T_Tag, T_Val> GetChild(T_Tag tag) {
        return children[tag];
    }

    public ITree<T_Tag, T_Val> GetChildInTree(T_Tag tag) {
        var ret = GetChild(tag);
        if (ret == null) {
            var childrens = GetChildren();
            if (childrens == null)
                return null;
            foreach (var elem in childrens) {
                var finded = GetChildInTree(tag);
                if (finded != null)
                    return finded;
            }
        }
        return ret;
    }

    public ITree<T_Tag, T_Val>[] GetChildren() {
        if (children.Count == 0)
            return null;
        var ret = new ITree<T_Tag, T_Val>[children.Count];
        var index = 0;
        foreach (var elem in children) {
            ret[index++] = elem.Value;
        }
        return ret;
    }

    public IEnumerator<ITree<T_Tag, T_Val>> GetEnumerator() {
        return GetEnumerator(EnumeratorType);
    }

    public bool RemoveChild(T_Tag tag) {
        var target = children[tag];
        if (target != null)
            target.parent = null;
        return children.Remove(tag);
    }

    public bool RemoveChildInTree(T_Tag tag) {
        var ret = RemoveChild(tag);
        if (!ret) {
            var childrens = GetChildren();
            if (childrens == null)
                return false;
            foreach (var elem in childrens) {
                var finded = RemoveChildInTree(tag);
                if (finded)
                    return true;
            }
        }
        return ret;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public ITree<T_Tag, T_Val> GetParent() {
        return parent;
    }

    public ITree<T_Tag, T_Val> GetRoot() {
        var ret = this;
        while (ret.parent != null)
            ret = ret.parent;
        return ret;
    }

    // 自带方法
    public bool IsCheckTag { get; set; }

    public bool AddChild(LinkedTree<T_Tag, T_Val> tree) {
        if (IsCheckTag && (GetRoot().Tag.Equals(tree.Tag) || GetRoot().GetChildInTree(tree.Tag) != null))
            return false;
        if (children.ContainsKey(tree.Tag))
            return false;
        this[tree.Tag] = tree;
        return true;
    }

    public IEnumerator<ITree<T_Tag, T_Val>> GetEnumerator(ETreeTraversalWay type) {
        
        if (type == ETreeTraversalWay.Unknown)
            type = EnumeratorType;
        switch (type) {   // TODO : 需要完善迭代器生成算法, 但目前暂不需要
            case ETreeTraversalWay.ChildrenOnly:
                foreach (var item in children) {
                    yield return item.Value;
                }
                break;
            case ETreeTraversalWay.LeavesOnly: {
                    if (ChildrenCount <= 0) {
                        yield return this;
                    } else {
                        var current = this;     // 要输出的迭代器成员
                        Stack<Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>.Enumerator> targetCurr = new Stack<Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>.Enumerator>();
                        do {
                            // 寻找到下一个后, 查询是否有子节点, 若有, 则转移为子节点, 若无则证明是叶子节点, 为遍历目标
                            for (var enumerator = current.children.GetEnumerator(); current.ChildrenCount > 0; enumerator = current.children.GetEnumerator()) {
                                enumerator.MoveNext();
                                targetCurr.Push(enumerator);
                                current = enumerator.Current.Value;
                            }
                            yield return current;
                            // 前往下一个, 若无下一个, 则返回到父节点的下一个, 若父节点也无,则继续寻找父节点
                            var currEnum = targetCurr.Pop();
                            var ret = currEnum.MoveNext();
                            while (!ret) {
                                // 若上一步一直追溯到根节点也没有下一个, 则证明已遍历完毕
                                if (targetCurr.Count > 0) {
                                    currEnum = targetCurr.Pop();
                                    ret = currEnum.MoveNext();
                                } else {
                                    break;
                                }
                            }
                            if (ret) {
                                targetCurr.Push(currEnum);
                            }
                        } while (targetCurr.Count > 0);
                    }
                }
                break;
            case ETreeTraversalWay.RandomTraversal:
                break;
            case ETreeTraversalWay.LayerorderTraversal:
                break;
            case ETreeTraversalWay.PreorderTraversal:
                break;
            case ETreeTraversalWay.PostorderTraversal:
                break;
        }
    }

    public static LinkedTree<T_Tag, T_Val> ToLinkedTree(ITree<T_Tag, T_Val> value) {
        if (value.GetType() != typeof(LinkedTree<T_Tag, T_Val>))
            return null;
        return (LinkedTree<T_Tag, T_Val>)value;
    }

    public int GetBranchDepth() {
        int ret = 0;
        foreach (var elem in children) {
            var elemDepth = elem.Value.GetBranchDepth();
            if (ret < elemDepth)
                ret = elemDepth;
        }
        return 1 + ret;
    }

    public int GetDepthInRoot() {
        int ret = 1;
        var roadParent = parent;
        while (roadParent != null) {
            ret++;
            roadParent = roadParent.parent;
        }
        return ret;
    }

    public ITree<T_Tag, T_Val>[] GetBranchRoad() {
        var list = new Stack<ITree<T_Tag, T_Val>>();
        list.Push(this);
        var roadParent = parent;
        while (roadParent != null) {
            list.Push(roadParent);
            roadParent = roadParent.parent;
        }
        var ret = new ITree<T_Tag, T_Val>[list.Count];
        var index = 0;
        while (list.Count > 0) {
            ret[index++] = list.Pop();
        }
        return ret;
    }

    /// <summary>
    ///     检查树的节点是否有重复
    /// </summary>
    /// <param name="node"> 如果输入此参数且不为null, 则仅检查树中是否有与此节点重复的节点 </param>
    /// <param name="parentOnly"> 如果输入此参数且为true, 则仅检查该节点的父节点序列中是否有该节点自己 </param>
    /// <returns> 无重复返回true </returns>
    public bool CheckTreeRef(LinkedTree<T_Tag, T_Val> node = null, bool parentOnly = false) {
        if (parentOnly) {
            if(node != null) {
                var p = node.parent;
                while(p != null) {
                    if(p == node) {
                        return false;
                    }
                    p = p.parent;
                }
                return true;
            } else {
                // TODO 没有指定节点, 检测整个树是否有父子重复
                return false;
            }
        } else {
            if (node != null) {
                // TODO 检测整个树是否有与此重复的节点
                return false;
            } else {
                // TODO 检测整个树是否有任意节点之间互相重复
                return false;
            }
        }
    }

    // 成员
    private LinkedTree<T_Tag, T_Val> parent = null;
    private Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>> children = new Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>();
}