

using System.Collections;
using System.Collections.Generic;

class LinkedTree<T_Tag, T_Val> : ITree<T_Tag, T_Val>
{
    public LinkedTree(T_Tag tag, T_Val val, LinkedTree<T_Tag, T_Val>parent )
    {
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
            if(value.GetType() != typeof(LinkedTree<T_Tag, T_Val>))
                throw new System.ArgumentException("The child of the tree you are setting is not a linked tree!", "tag");
            else if (tag.Equals(value.Tag))
                children[tag] = ToLinkedTree(value);
            else
                throw new System.ArgumentException("The tag of the value is different from old tag", "tag");
        }
    }

    public T_Tag Tag { get; set; }

    public T_Val Value { get; set; }

    public ETreeTraversalWay EnumeratorType { get; set; }

    public bool IsCheckTag { get; set; }

    public static LinkedTree<T_Tag, T_Val> ToLinkedTree(ITree<T_Tag, T_Val> value)
    {
        if (value.GetType() != typeof(LinkedTree<T_Tag, T_Val>))
            return null;
        return (LinkedTree<T_Tag, T_Val>)value;
    }

    public bool AddChild(T_Tag tag, T_Val value)
    {
        if (IsCheckTag && (GetRoot().Tag.Equals(tag) || GetRoot().GetChildInTree(tag) != null))
            return false;
        if (children.ContainsKey(tag))
            return false;
        children.Add(tag, new LinkedTree<T_Tag, T_Val>(tag, value, this));
        return true;
    }

    public bool AddChild(LinkedTree<T_Tag, T_Val> tree)
    {
        if (IsCheckTag && (GetRoot().Tag.Equals(tree.Tag) || GetRoot().GetChildInTree(tree.Tag) != null))
            return false;
        if (children.ContainsKey(tree.Tag))
            return false;
        tree.GetParent().RemoveChild(tree.Tag);
        tree.parent = this;
        children.Add(tree.Tag, tree);
        return true;
    }

    public void ClearChildren()
    {
        foreach(var elem in children)
        {
            elem.Value.parent = null;
        }
        children.Clear();
    }

    public ITree<T_Tag, T_Val> GetChild(T_Tag tag)
    {
        return children[tag];
    }

    public ITree<T_Tag, T_Val> GetChildInTree(T_Tag tag)
    {
        var ret = GetChild(tag);
        if(ret == null)
        {
            var childrens = GetChildren();
            if (childrens == null)
                return null;
            foreach(var elem in childrens)
            {
                var finded = GetChildInTree(tag);
                if (finded != null)
                    return finded;
            }
        }
        return ret;
    }

    public ITree<T_Tag, T_Val>[] GetChildren()
    {
        if (children.Count == 0)
            return null;
        var ret = new ITree<T_Tag, T_Val>[children.Count];
        var index = 0;
        foreach (var elem in children)
        {
            ret[index++] = elem.Value;
        }
        return ret;
    }

    public IEnumerator<KeyValuePair<T_Tag, T_Val>> GetEnumerator()
    {
        throw new System.NotImplementedException(); // TODO
    }

    public bool RemoveChild(T_Tag tag)
    {
        var target = children[tag];
        if (target != null)
            target.parent = null;
        return children.Remove(tag);
    }

    public bool RemoveChildInTree(T_Tag tag)
    {
        var ret = RemoveChild(tag);
        if (!ret)
        {
            var childrens = GetChildren();
            if (childrens == null)
                return false;
            foreach (var elem in childrens)
            {
                var finded = RemoveChildInTree(tag);
                if (finded)
                    return true;
            }
        }
        return ret;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public ITree<T_Tag, T_Val> GetParent()
    {
        return parent;
    }

    public ITree<T_Tag, T_Val> GetRoot()
    {
        var ret = this;
        while (ret.parent != null)
            ret = ret.parent;
        return ret;
    }

    private LinkedTree<T_Tag, T_Val> parent;
    private Dictionary<T_Tag, LinkedTree<T_Tag, T_Val>> children;
}
