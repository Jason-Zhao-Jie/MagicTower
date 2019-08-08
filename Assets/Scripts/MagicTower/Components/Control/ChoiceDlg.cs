using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;
using MagicTower.Components.Unit;

namespace MagicTower.Components.Control
{

    public class ChoiceDlg : MonoBehaviour
    {        
        // Use this for initialization
        void Awake()
        {
            choiceSpeakerText.fontSize = Convert.ToInt32(choiceSpeakerText.fontSize * Game.RealFontSize);
            choiceTitleText.fontSize = Convert.ToInt32(choiceTitleText.fontSize * Game.RealFontSize);
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().text = "";
            firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize = Convert.ToInt32(firstChoiceItem.transform.Find("Text").GetComponent<Text>().fontSize * Game.RealFontSize);
            choiceItems = new List<Button>();
        }

        public void Init(Model.ChoiceData data, Modal choiceMod, Model.EGameStatus nextStatus = Model.EGameStatus.InGame) {
            // 设定信息
            this.data = data;
            this.choiceMod = choiceMod;
            this.nextStatus = nextStatus;

            // 设定游戏状态
            Game.Status = Model.EGameStatus.OnChoice;
            // 显示对话者头像
            if(data.speakerId < 0)
                data.speakerId = Game.Player.PlayerId;
            var modal = Game.Config.modals[data.speakerId];
            choiceSpeaker.OnInit(ObjectPool.ElementType.Image, data.speakerId, modal);
            choiceSpeakerText.text = Game.Config.StringInternational.GetValue(modal.name);
            // 设定选择的标题介绍对话的内容
            choiceTitleText.text = Game.Config.StringInternational.GetValue(data.title);
            // 添加选项
            RefreshChoiceItems();
        }

        public void RefreshChoiceItems()
        {
            ClearChoiceItems();
            firstChoiceItem.enabled = true;
            foreach (var i in data.data)
            {
                if (i.contentData == null)
                {
                    CreateChoiceItem(Game.Config.StringInternational.GetValue(i.content));
                }
                else
                {
                    var data = new string[i.contentData.Length];
                    for (var n = 0; n < data.Length; ++n)
                    {
                        data[n] = i.contentData[n];
                    }
                    if (data.Length > 0 && i.contentType != (int)Game.VariablePriceType.NoChange)
                    {
                        data[0] = Game.GetNumberData(Convert.ToInt32(i.contentData[0])).ToString();
                    }
                    CreateChoiceItem(Game.Config.StringInternational.GetValue(i.content, data));
                }
            }
            firstChoiceItem.Select();   // 选中第一项
            if (data.data != null && data.data.Length > 0)
            {
                // 若上次点选过第一项, 则由于第一项未被销毁, 本次调用select不会使按钮高亮, 这样的话需要做如下处理
                 choiceItems[0].Select();
                 firstChoiceItem.Select();
            }
        }

        public void ClearChoiceItems()
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
                if (index == 1)
                {
                    var lastnav = firstChoiceItem.navigation;
                    lastnav.selectOnDown = clonedItem.GetComponent<Button>();
                    firstChoiceItem.navigation = lastnav;
                    var nav = clonedItem.GetComponent<Button>().navigation;
                    nav.selectOnUp = firstChoiceItem;
                    nav.selectOnDown = null;
                    clonedItem.GetComponent<Button>().navigation = nav;
                }
                else
                {
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

        public void ChooseToItem(UnityEngine.EventSystems.BaseEventData data)
        {
            (data as UnityEngine.EventSystems.PointerEventData).pointerEnter.GetComponent<Button>().Select();
        }

        public void OnItemClicked(int index)
        {
            if (Game.Status != Model.EGameStatus.OnChoice && Game.Status != Model.EGameStatus.Start)
            {
                return;
            }

            if (data.data[index].close)
            {
                Game.Status = nextStatus;
            }
            Present.Manager.EventManager.DispatchEvent(data.data[index].eventId, choiceMod, data.data[index].eventData);
            if (data.data[index].close)
            {
                ClearChoiceItems();
                // 隐藏选择对话框
                Game.HideUI(UIType.ChoiceDialog);
            }
            else
            {
                RefreshChoiceItems();
            }
        }

        public Modal choiceSpeaker;
        public Text choiceTitleText;
        public Text choiceSpeakerText;
        public GameObject choiceItemPanel;
        public Button firstChoiceItem;
        private Modal choiceMod;
        private Model.ChoiceData data;
        private List<Button> choiceItems;
        private Model.EGameStatus nextStatus;
    }

}