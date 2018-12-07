using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 用于简单的滚动列表的 Component,
/// 采用 Unity 自带的 ScrollView 即可, 但是 content 需要添加一种 LayoutGroup :
/// VerticalLayoutGroup, HorizontalLayoutGroup, 或者 GridLayoutGroup
/// </summary>
public class ListView : UnityEngine.UI.ScrollRect {
    public class DefaultItemNotSetException : System.Exception {

    }

    /// <summary>
    /// 设定 Default Item, 类似于 cocos2d 的 ListView
    /// </summary>
    public UnityEngine.RectTransform DefaultElement {
        get {
            return defaultElement;
        }
        set {
            defaultElement = value;
            if (value != null && content.GetComponent<UnityEngine.UI.GridLayoutGroup>() != null) {
                content.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = defaultElement.rect.size;
            }
        }
    }

    /// <summary>
    /// 设定 content 的 padding 
    /// </summary>
    public UnityEngine.RectOffset padding {
        get { return content.GetComponent<UnityEngine.UI.LayoutGroup>().padding; }
        set { content.GetComponent<UnityEngine.UI.LayoutGroup>().padding = value; }
    }

    /// <summary>
    /// 获取指定索引位置的子项. 索引超出时按列表索引超出处理
    /// </summary>
    /// <param name="index">子项索引</param>
    /// <returns></returns>
    public UnityEngine.RectTransform this[int index] {
        get {
            return children[index];
        }
        private set {
            if(ItemCount > index) {
                throw new System.IndexOutOfRangeException();
            }else if(ItemCount == index) {
                children[index] = value;
                value.SetParent(content);
            } else {
                children[index].SetParent(null);
                children[index] = value;
                value.SetParent(content);
                value.SetSiblingIndex(index);
            }
        }
    }

    /// <summary>
    /// 子项总数量
    /// </summary>
    public int ItemCount {
        get {
            return children.Count;
        }
    }

    /// <summary>
    /// 在表末尾添加一个 default item
    /// </summary>
    /// <returns> 成功添加的item </returns>
    public UnityEngine.RectTransform PushbackDefaultItem() {
        if(defaultElement == null) {
            throw new DefaultItemNotSetException();
        }
        var ret = Instantiate(defaultElement.gameObject).GetComponent<UnityEngine.RectTransform>();
        this[ItemCount] = ret;
        return ret;
    }

    /// <summary>
    /// 在指定索引处插入 default item
    /// </summary>
    /// <param name="index">要插入的索引处</param>
    /// <returns>返回插入的 item </returns>
    public UnityEngine.RectTransform InsertDefaultItem(int index) {
        if (defaultElement == null) {
            throw new DefaultItemNotSetException();
        }
        var ret = Instantiate(defaultElement.gameObject).GetComponent<UnityEngine.RectTransform>();
        ret.SetParent(content);
        ret.SetSiblingIndex(index);
        children.Insert(index, ret);
        return ret;
    }

    /// <summary>
    /// 删除指定索引处的item, 后面的依次补上
    /// </summary>
    /// <param name="index"> 要删除的 item 的索引 </param>
    /// <returns></returns>
    public UnityEngine.RectTransform DeleteItem(int index) {
        var ret = children[index];
        ret.SetParent(null);
        children.RemoveAt(index);
        return ret;
    }

    /// <summary>
    /// 将指定索引处的 item 替换成新的 default item
    /// </summary>
    /// <param name="index"> 要替换的 item 的索引 </param>
    /// <returns></returns>
    public UnityEngine.RectTransform ReplaceItemToDefault(int index) {
        if (defaultElement == null) {
            throw new DefaultItemNotSetException();
        }
        var ret = children[index];
        ret.SetParent(null);
        var newItem = Instantiate(defaultElement.gameObject).GetComponent<UnityEngine.RectTransform>();
        newItem.SetParent(content);
        newItem.SetSiblingIndex(index);
        this[index] = newItem;
        return ret;
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();

    }

    // Update is called once per frame
    void Update() {

    }

    private UnityEngine.RectTransform defaultElement;
    private List<UnityEngine.RectTransform> children = new List<UnityEngine.RectTransform>();
}
