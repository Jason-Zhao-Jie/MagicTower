using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatDlg : ObjectPool.AElement
{
    public const string TOP_PREFAB_DIR = "TopChat";
    public const int TOP_PREFAB_ID = 4;
    public const string BOTTOM_PREFAB_DIR = "BottomChat";
    public const int BOTTOM_PREFAB_ID = 5;

    public const int SPEAKER_SORTING_ORDER = 1;

    public static ChatDlg ShowChat(bool isTop)
    {
        string dir;
        int id;
        if(isTop)
        {
            dir = TOP_PREFAB_DIR;
            id = TOP_PREFAB_ID;
        }
        else
        {
            dir = BOTTOM_PREFAB_DIR;
            id = BOTTOM_PREFAB_ID;
        }
        var ret = ObjectPool.instance.GetAnElement<ChatDlg>(id, ObjectPool.ElementType.Dialog, Constant.DIALOG_DIR + dir);
        ret.isTop = isTop;
        return ret;
    }

    // Use this for initialization
    void Awake()
    {
        speaker = transform.Find("Speaker").gameObject;
        speakerText = transform.Find("SpeakerName").GetComponent<UnityEngine.UI.Text>();
        speakerText.fontSize = Convert.ToInt32(speakerText.fontSize * ScreenAdaptator.instance.RealFontSize);
        speaker.transform.position = new Vector3(speakerText.transform.position.x, speaker.transform.position.y, speaker.transform.position.z);
        text = transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
        text.fontSize = Convert.ToInt32(text.fontSize * ScreenAdaptator.instance.RealFontSize);
    }

    public void SetChat(string content, int speakerId = -1)
    {
        // 查找对话者数据, 贴上人物头像
        if (speakerId < 0)
            speakerId = PlayerController.instance.PlayerId;
        var modal = DataCenter.instance.modals[speakerId];
        ObjectPool.AElement obj = null;
        if ((Modal.ModalType)modal.typeId == Modal.ModalType.Player)
        {
            obj = ObjectPool.instance.GetAnElement<Player>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
        }
        else
        {
            obj = ObjectPool.instance.GetAnElement<Modal>(modal.id, ObjectPool.ElementType.Sprite, Constant.PREFAB_DIR + modal.prefabPath);
        }
        obj.transform.SetParent(transform, false);
        obj.transform.position = speaker.transform.position;
        obj.transform.localScale = ScreenAdaptator.instance.BlockSize;
        obj.GetComponent<SpriteRenderer>().sortingOrder = SPEAKER_SORTING_ORDER;
        var mod = speaker.GetComponent<Modal>();
        if (mod != null)
            mod.RemoveSelf(false);
        else if (speaker.GetComponent<Player>() != null)
            speaker.GetComponent<Player>().RemoveSelf();
        else
            Destroy(speaker);
        speaker = obj.gameObject;
        // 对话者名字
        speakerText.text = StringInternational.GetValue(modal.name);
        // 对话内容
        text.text = StringInternational.GetValue(content);
    }

    public override ObjectPool.ElementType GetPoolTypeId()
    {
        return ObjectPool.ElementType.Dialog;
    }

    public override bool RecycleSelf()
    {
        return RecycleSelf<ChatDlg>();
    }

    public override string ResourcePath {
        get {
            return Constant.DIALOG_DIR + (isTop?TOP_PREFAB_DIR:BOTTOM_PREFAB_DIR);
        }
    }

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

    private GameObject speaker;
    private UnityEngine.UI.Text speakerText;
    private UnityEngine.UI.Text text;
    private bool isTop = false;
}
