

using System.Collections;
using System.Collections.Generic;

public class LinkedTree<T_Tag, T_Val> : ITree<T_Tag, T_Val> {
    // 实现接口
    public LinkedTree(T_Tag tag, T_Val val, LinkedTree<T_Tag, T_Val> parent) {
        Tag = tag;
        Value = val;
        this.parent = parent;
        IsCheckTag = false;
    }

    public ITree<T_Tag, T_Val> this[T_Tag tag] {
        get {
            return children[tag];
        }
        set {
            if (value.GetType() != typeof(LinkedTree<T_Tag, T_Val>))
                throw new System.ArgumentException("The child of the tree you are setting is not a linked tree!", "tag");
            else if (tag.Equals(value.Tag))
                children[tag] = ToLinkedTree(value);
            else
                throw new System.ArgumentException("The tag of the value is different from old tag", "tag");
        }
    }

    public T_Tag Tag { get; set; }

    public T_Val Value { get; set; }

    public int ChildrenCount { get { return children.Count; } }

    public ETreeTraversalWay EnumeratorType { get; set; }

    public bool AddChild(T_Tag tag, T_Val value) {
        if (IsCheckTag && (GetRoot().Tag.Equals(tag) || GetRoot().GetChildInTree(tag) != null))
            return false;
        if (children.ContainsKey(tag))
            return false;
        children.Add(tag, new LinkedTree<T_Tag, T_Val>(tag, value, this));
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
        return new Enumerator(this);
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
        tree.GetParent().RemoveChild(tree.Tag);
        tree.parent = this;
        children.Add(tree.Tag, tree);
        return true;
    }

    public Enumerator GetEnumerator(ETreeTraversalWay type) {
        return new Enumerator(this, type);
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

    // 成员
    private LinkedTree<T_Tag, T_Val> parent;
    private Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>> children = new Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>();

    public class Enumerator : IEnumerator<ITree<T_Tag, T_Val>> {
        public Enumerator(LinkedTree<T_Tag, T_Val> tree, ETreeTraversalWay traversalWay = ETreeTraversalWay.Unknown) {
            targetTree = tree;
            if (enumeratorType == ETreeTraversalWay.Unknown)
                enumeratorType = targetTree.EnumeratorType;
            else
                enumeratorType = traversalWay;
            Reset();
            Current = null;
            if (enumeratorType == ETreeTraversalWay.ChildrenOnly) {
                targetCurr.Push(tree.children.GetEnumerator());
            }
        }

        ~Enumerator() {
            Dispose();
        }

        public ITree<T_Tag, T_Val> Current { get; private set; }

        object IEnumerator.Current {
            get {
                return Current;
            }
        }

        public void Dispose() {
        }

        /// <summary>
        /// 迭代器遍历算法, 见 <see cref="ETreeTraversalWay"/>
        /// 实现采用非递归算法, 不可逆遍历的效率略低
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() {
            bool ret = false;
            switch (enumeratorType) {   // TODO : 需要完善遍历算法, 但目前暂不需要
                case ETreeTraversalWay.ChildrenOnly:
                    ret = targetCurr.Peek().MoveNext();
                    Current = targetCurr.Peek().Current.Value;
                    break;
                case ETreeTraversalWay.LeavesOnly:
                    // 终点
                    if (Current == null) {
                        Current = targetTree;
                        while (Current.ChildrenCount > 0) {
                            var en = ToLinkedTree(Current).children.GetEnumerator();
                            en.MoveNext();
                            targetCurr.Push(en);
                            Current = en.Current.Value;
                        }
                        ret = Current != null;
                        break;
                    }
                    if (targetCurr.Count <= 0) {
                        Current = null;
                        ret = false;
                        break;
                    }
                    // 前往下一个, 若无下一个, 则返回到父节点的下一个, 若父节点也无,则继续寻找父节点
                    ret = targetCurr.Peek().MoveNext();
                    while (!ret) {
                        targetCurr.Pop();
                        // 若上一步一直追溯到根节点也没有下一个, 则证明已遍历完毕
                        if (targetCurr.Count <= 0) {
                            break;
                        }
                        ret = targetCurr.Peek().MoveNext();
                    }
                    // 若上一步一直追溯到根节点也没有下一个, 则证明已遍历完毕
                    if (targetCurr.Count <= 0) {
                        Current = null;
                        ret = false;
                        break;
                    }
                    // 寻找到下一个后, 查询是否有子节点, 若有, 则转移为子节点, 若无则证明是叶子节点, 为遍历目标
                    while (targetCurr.Peek().Current.Value.children.Count > 0) {
                        targetCurr.Push(targetCurr.Peek().Current.Value.children.GetEnumerator());
                    }
                    // 记录遍历目标
                    Current = targetCurr.Peek().Current.Value;
                    ret = true;
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
            return ret;
        }

        public void Reset() {
            targetCurr = new Stack<Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>.Enumerator>();
            Current = targetTree;
        }

        private Stack<Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>>.Enumerator> targetCurr;
        private readonly LinkedTree<T_Tag, T_Val> targetTree;
        private readonly ETreeTraversalWay enumeratorType;
    }
}