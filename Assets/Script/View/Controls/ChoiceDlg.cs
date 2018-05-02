using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceDlg : ObjectPool.AElement
{
    public const string PREFAB_DIR = "ChoiceDlg";
    public const int PREFAB_ID = 2;

    public static ChoiceDlg StartChoice(Transform parent, Constant.ChoiceData choiceData, Modal mod, Constant.EGameStatus nextStatus = Constant.EGameStatus.InGame)
    {
        // 弹出战斗框
        var ret = ObjectPool.instance.GetAnElement<ChoiceDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
        // 设定信息
        ret.choice = choiceData;
        ret.choiceMod = mod;
        ret.nextStatus = nextStatus;
        ret.transform.SetParent(parent, false);
        ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
        ret.ShowChoice();
        return ret;
    }

    // Use this for initialization
    void Awake()
    {
        var choiceInfoPanel = transform.Find("ChoiceInfoPanel").gameObject;
        choiceSpeaker = choiceInfoPanel.transform.Find("Speaker").gameObject;
        choiceSpeakerText = choiceInfoPanel.transform.Find("SpeakerName").GetComponent<Text>();
        choiceSpeakerText.fontSize = Convert.ToInt32(choiceSpeakerText.fontSize*ScreenAdaptator.instance.RealFontSize);
        choiceSpeaker.transform.position = new Vector3(choiceSpeakerText.transform.position.x, choiceSpeaker.transform.position.y, choiceSpeaker.transform.position.z);
        choiceTitleText = choiceInfoPanel.transform.Find("TitleText").GetComponent<Text>();
        choiceTitleText.fontSize = Convert.ToInt32(choiceTitleText.fontSize*ScreenAdaptator.instance.RealFontSize);
        choiceItemPanel = transform.Find("ItemPanel").gameObject;
        choiceItemPanel.GetComponent<VerticalLayoutGroup>().spacing *= ScreenAdaptator.instance.RealFontSize;
        firstChoiceItem = choiceItemPanel.transform.Find("FirstItem").gameObject;
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = "";
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize = Convert.ToInt32(firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize*ScreenAdaptator.instance.RealFontSize);
        firstChoiceItem.GetComponent<LayoutElement>().minHeight *= ScreenAdaptator.instance.RealFontSize;
        var itemRect = firstChoiceItem.GetComponent<RectTransform>().sizeDelta;
        itemRect.y = firstChoiceItem.GetComponent<LayoutElement>().minHeight;
        firstChoiceItem.GetComponent<RectTransform>().sizeDelta = itemRect;
        choiceItems = new List<GameObject>();
    }

    private void ShowChoice()
    {
        // 设定游戏状态
        DataCenter.instance.Status = Constant.EGameStatus.OnChoice;
        // 显示选择对话框
        gameObject.SetActive(true);
        // 显示对话者头像
        if (choice.speakerId < 0)
            choice.speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.modals[choice.speakerId];
        var obj = ObjectPool.instance.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
        obj.transform.SetParent(transform, false);
        obj.transform.position = choiceSpeaker.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = choiceSpeaker.GetComponent<SpriteRenderer>().sortingOrder;
        var mod = choiceSpeaker.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else
            choiceSpeaker.GetComponent<Player>().RemoveSelf();
        choiceSpeaker = obj.gameObject;
        choiceSpeakerText.text = modal.name;
        // 设定选择的标题介绍对话的内容
        choiceTitleText.text = choice.title;
        // 添加选项
        foreach (var i in choice.data)
        {
            CreateChoiceItem(i.content);
        }
        // 矫正选择框的大小
        RedrawItems();
    }

    public void ClearChoice()
    {
        gameObject.SetActive(true);
        // 清除选项
        foreach (var i in choiceItems)
        {
            Destroy(i);
        }
        choiceItems.Clear();
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = "";
        // 隐藏选择对话框
        DataCenter.instance.Status = nextStatus;
    }

    private GameObject CreateChoiceItem(string content)
    {
        if (firstChoiceItem.transform.Find("Text").GetComponent<Text>().text.Equals(""))
        {
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = content;
            return firstChoiceItem;
        }
        else
        {
            var clonedItem = Instantiate(firstChoiceItem, choiceItemPanel.transform, false);
            clonedItem.transform.Find("Text").GetComponent<Text>().text = content;
            choiceItems.Add(clonedItem);
            clonedItem.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            clonedItem.GetComponent<Button>().onClick.AddListener(delegate () { OnItemClicked(choiceItems.Count); });
            return clonedItem;
        }
    }

    public void ChooseToItem(uint index)
    {
        if (index > choiceItems.Count)
            return;
        if (index == 0)
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Bold;
        else
            choiceItems[System.Convert.ToInt32(index) + 1].transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Bold;
        if (chosenIndex == 0)
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Bold;
        else
            choiceItems[System.Convert.ToInt32(chosenIndex) + 1].transform.Find("Text").GetComponent<Text>().fontStyle = FontStyle.Normal;
        chosenIndex = index;
    }

    private void RedrawItems()
    {
        // 计算选择部分总高度,排列选项
        // firstChoiceItem.transform.localPosition = new Vector3(firstChoiceItem.transform.localPosition.x, 0, firstChoiceItem.transform.localPosition.z);
        var unitHeight = firstChoiceItem.GetComponent<LayoutElement>().minHeight;
        var spacing = choiceItemPanel.GetComponent<VerticalLayoutGroup>().spacing;
        var totalHeight = unitHeight;
        foreach (var v in choiceItems)
        {
            // v.transform.localPosition = new Vector3(firstChoiceItem.transform.localPosition.x, totalHeight, firstChoiceItem.transform.localPosition.z);
            totalHeight += unitHeight + spacing * 2;
        }

        // 调整选择框总大小
        var newRect = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().rect.height);
        newRect.y += totalHeight - unitHeight - 25;
        GetComponent<RectTransform>().sizeDelta = newRect;
    }

    public void OnItemClicked(int index)
    {
        ObjectPool.instance.RecycleAnElement(this);
        ClearChoice();
        EventManager.instance.DispatchEvent(choice.data[chosenIndex].eventId, choiceMod, choice.data[chosenIndex].eventData);
    }

    public override ObjectPool.ElementType GetPoolTypeId()
    {
        return ObjectPool.ElementType.Dialog;
    }

    public override bool RecycleSelf()
    {
        return RecycleSelf<ChoiceDlg>();
    }

    public override string ResourcePath { get { return Constant.DIALOG_DIR + PREFAB_DIR; } }
    public static string GetResourcePath() { return Constant.DIALOG_DIR + PREFAB_DIR; }

    public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath)
    {
        return true;
    }

    public override void OnReuse(ObjectPool.ElementType tid, int elemId)
    {
    }

    public override bool OnUnuse(ObjectPool.ElementType tid, int elemId)
    {
        return true;
    }

    private GameObject choiceSpeaker;
    private Text choiceTitleText;
    private Text choiceSpeakerText;
    private GameObject choiceItemPanel;
    private Constant.ChoiceData choice;
    private Modal choiceMod;
    private GameObject firstChoiceItem;
    private List<GameObject> choiceItems;
    private uint chosenIndex;
    private Constant.EGameStatus nextStatus;
}
