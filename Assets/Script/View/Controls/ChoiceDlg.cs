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
        choiceSpeakerFrame = choiceInfoPanel.transform.Find("SpeakerFrame").gameObject;
        choiceSpeaker = choiceInfoPanel.transform.Find("Speaker")?.gameObject;
        choiceSpeakerText = choiceInfoPanel.transform.Find("SpeakerName").GetComponent<Text>();
        choiceSpeakerText.fontSize = Convert.ToInt32(choiceSpeakerText.fontSize*ScreenAdaptator.instance.RealFontSize);
        choiceTitleText = choiceInfoPanel.transform.Find("TitleText").GetComponent<Text>();
        choiceTitleText.fontSize = Convert.ToInt32(choiceTitleText.fontSize*ScreenAdaptator.instance.RealFontSize);
        choiceItemPanel = transform.Find("ItemPanel").gameObject;
        //choiceItemPanel.GetComponent<VerticalLayoutGroup>().spacing *= ScreenAdaptator.instance.RealFontSize;
        firstChoiceItem = choiceItemPanel.transform.Find("FirstItem").GetComponent<Button>();
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = "";
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize = Convert.ToInt32(firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize*ScreenAdaptator.instance.RealFontSize);
        choiceItems = new List<Button>();
    }

    private void ShowChoice() {
        firstChoiceItem.enabled = true;
        choiceSpeaker?.SetActive(true);
        // 设定游戏状态
        DataCenter.instance.Status = Constant.EGameStatus.OnChoice;
        // 显示选择对话框
        gameObject.SetActive(true);
        // 显示对话者头像
        if (choice.speakerId < 0)
            choice.speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.modals[choice.speakerId];
        ObjectPool.AElement obj = null;
        if ((Modal.ModalType)modal.typeId == Modal.ModalType.Player) {
            obj = ObjectPool.instance.GetAnElement<Player>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        } else {
            obj = ObjectPool.instance.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath, Constant.SPRITE_IN_DIALOG_SORTING_ORDER);
        }
        obj.transform.SetParent(choiceSpeakerFrame.transform, false);
        obj.transform.localPosition = new Vector3(0, 0, obj.transform.localPosition.z);
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        if (choiceSpeaker != null) {
            var mod = choiceSpeaker.GetComponent<Modal>();
            if (mod != null)
                mod.RemoveSelf(false);
            else if (choiceSpeaker.GetComponent<Player>() != null)
                choiceSpeaker.GetComponent<Player>().RemoveSelf();
            else
                Destroy(choiceSpeaker);
        }
        choiceSpeaker = obj.gameObject;
        choiceSpeakerText.text = StringInternational.GetValue(modal.name);
        // 设定选择的标题介绍对话的内容
        choiceTitleText.text = StringInternational.GetValue(choice.title);
        // 添加选项
        foreach (var i in choice.data)
        {
            CreateChoiceItem(StringInternational.GetValue(i.content, i.contentData));
        }
        firstChoiceItem.Select();   // 选中第一项
        if(choice.data != null && choice.data.Length > 0) {
            // 若上此点选过第一项, 则由于第一项未被销毁, 本次调用select不会使按钮高亮, 这样的话需要做如下处理
            choiceItems[0].Select();
            firstChoiceItem.Select();
        }
    }

    public void ClearChoice()
    {
        // 清除选项
        foreach (var i in choiceItems)
        {
            var nav = i.navigation;
            nav.selectOnDown = null;
            nav.selectOnUp = null;
            nav.selectOnLeft = null;
            nav.selectOnRight = null;
            i.navigation = nav;
            Destroy(i.gameObject);
        }
        choiceItems.Clear();
        firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = "";
        var firstnav = firstChoiceItem.navigation;
        firstnav.selectOnDown = null;
        firstnav.selectOnUp = null;
        firstnav.selectOnLeft = null;
        firstnav.selectOnRight = null;
        firstChoiceItem.navigation = firstnav;
        firstChoiceItem.enabled = false;

        // 隐藏选择对话框
        choiceSpeaker?.SetActive(false);
        gameObject.SetActive(false);
        DataCenter.instance.Status = nextStatus;
    }

    private Button CreateChoiceItem(string content)
    {
        if (firstChoiceItem.transform.Find("Text").GetComponent<Text>().text.Equals(""))
        {
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = content;
            return firstChoiceItem;
        }
        else
        {
            var clonedItem = Instantiate(firstChoiceItem.gameObject, choiceItemPanel.transform, false);
            clonedItem.transform.Find("Text").GetComponent<Text>().text = content;
            clonedItem.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            var index = choiceItems.Count + 1;
            clonedItem.GetComponent<Button>().onClick.AddListener(delegate () { OnItemClicked(index); });
            choiceItems.Add(clonedItem.GetComponent<Button>());
            if (index == 1) {
                var lastnav = firstChoiceItem.navigation;
                lastnav.selectOnDown = clonedItem.GetComponent<Button>();
                firstChoiceItem.navigation = lastnav;
                var nav = clonedItem.GetComponent<Button>().navigation;
                nav.selectOnUp = firstChoiceItem;
                nav.selectOnDown = null;
                clonedItem.GetComponent<Button>().navigation = nav;
            } else {
                var lastnav = choiceItems[index - 2].GetComponent<Button>().navigation;
                lastnav.selectOnDown = clonedItem.GetComponent<Button>();
                choiceItems[index - 2].GetComponent<Button>().navigation = lastnav;
                var nav = clonedItem.GetComponent<Button>().navigation;
                nav.selectOnUp = choiceItems[index - 2].GetComponent<Button>();
                nav.selectOnDown = null;
                clonedItem.GetComponent<Button>().navigation = nav;
            }
            return clonedItem.GetComponent<Button>();
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

    private GameObject choiceSpeakerFrame;
    private GameObject choiceSpeaker;
    private Text choiceTitleText;
    private Text choiceSpeakerText;
    private GameObject choiceItemPanel;
    private Constant.ChoiceData choice;
    private Modal choiceMod;
    private Button firstChoiceItem;
    private List<Button> choiceItems;
    private uint chosenIndex;
    private Constant.EGameStatus nextStatus;
}
