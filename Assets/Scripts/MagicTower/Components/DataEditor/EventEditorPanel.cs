using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicTower.Present.Manager;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor
{
    public class EventEditorPanel : MonoBehaviour
    {
        public int EventId {
            get; set;
        }

        public long[] SelectedKey {
            get; set;
        }

        public System.Action<long[]> ApplyCallback { get; set; }

        private void Awake()
        {
            toggleParam0Text.gameObject.SetActive(false);
            toggleParam0.gameObject.SetActive(false);
            txtParam1.gameObject.SetActive(false);
            dropParam1.gameObject.SetActive(false);
            txtParam2.gameObject.SetActive(false);
            inputParam2.gameObject.SetActive(false);
            txtParam3.gameObject.SetActive(false);
            inputParam3.gameObject.SetActive(false);
            selectEventDataList.gameObject.SetActive(false);
            dropCostType.gameObject.SetActive(false);
            inputCostIdCount.gameObject.SetActive(false);
            dropIncreaseType.gameObject.SetActive(false);
            selectConditionList.gameObject.SetActive(false);
            dropConditionType.gameObject.SetActive(false);
            inputConditionMinValue.gameObject.SetActive(false);
            dropFailedEventType.gameObject.SetActive(false);
            selectFailedEventDataList.gameObject.SetActive(false);

            var eventName = (EventManager.EventName)EventId;
            List<string> options;
            switch (eventName)
            {
                case EventManager.EventName.None:
                    // no parameter
                    break;
                case EventManager.EventName.NormalBattle:
                    // param0: canFail, param1: selfUuid
                    toggleParam0Text.gameObject.SetActive(true);
                    toggleParam0Text.text = "can fail";
                    toggleParam0.gameObject.SetActive(true);
                    toggleParam0.isOn = SelectedKey[0] != 0;
                    txtParam2.gameObject.SetActive(true);
                    txtParam2.text = "self uuid";
                    inputParam2.gameObject.SetActive(true);
                    inputParam2.text = SelectedKey[1].ToString();
                    break;
                case EventManager.EventName.OpenFreeDoor:
                    // no parameter
                    break;
                case EventManager.EventName.NormalSend:
                    // param0: mapId, param1: posx, param2: posy
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "map id";
                    dropParam1.gameObject.SetActive(true);
                    options = new List<string>(Game.Config.MapsCount);
                    for (var i = 0; i < Game.Config.MapsCount; ++i)
                    {
                        options.Add((i + 1).ToString());
                    }
                    dropParam1.AddOptions(options);
                    dropParam1.value = System.Convert.ToInt32(SelectedKey[0]) - 1;
                    txtParam2.gameObject.SetActive(true);
                    txtParam2.text = "pos x";
                    inputParam2.gameObject.SetActive(true);
                    inputParam2.text = SelectedKey[1].ToString();
                    txtParam3.gameObject.SetActive(true);
                    txtParam3.text = "pos y";
                    inputParam3.gameObject.SetActive(true);
                    inputParam3.text = SelectedKey[2].ToString();
                    break;
                case EventManager.EventName.NormalChat:
                    // param0: chatDataIndex
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "chat data";
                    dropParam1.gameObject.SetActive(true);
                    options = new List<string>(Game.Config.chats.Count);
                    foreach (var i in Game.Config.chats)
                    {
                        options.Add(i.Key.ToString());
                    }
                    dropParam1.AddOptions(options);
                    // TODO : 这里chat用下拉列表不太好, 应该添加一个按钮选择辅助, 但弹出的chat选择框必须只读, 否则就无限循环导致同步问题了, 下同
                    // TODO : 也可以考虑专门做一个 chat / choice 的选择框, 像 modal selector 那样, 只列出内容供查看, 然后提供一个可以禁用的修改按钮用于打开对应的修改窗口
                    dropParam1.value = System.Convert.ToInt32(SelectedKey[0]) - 1;      // TODO : 这里的序号
                    break;
                case EventManager.EventName.NormalChoice:
                    // param0: choiceDataIndex
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "choice data";
                    dropParam1.gameObject.SetActive(true);
                    options = new List<string>(Game.Config.choices.Count);
                    foreach (var i in Game.Config.choices)
                    {
                        options.Add(i.Key.ToString());
                    }
                    dropParam1.AddOptions(options);
                    // TODO : 同上
                    dropParam1.value = System.Convert.ToInt32(SelectedKey[0]) - 1;      // TODO : 这里的序号
                    break;
                case EventManager.EventName.CallGame:
                    // param0: gameId ( uncompleted )
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "game";
                    dropParam1.gameObject.SetActive(true);
                    break;
                case EventManager.EventName.GetBaseResourceItem:
                    // param0: resourceType, param1: resourceItemCount, param2:(internal)successEventId, param3:(internal)successEventDataLength, param4~paramN: successEventData, paramN+1:(internal)costType, paramN+2:costCount(paramN+3==0)orCostIncreaseRecordId, paramN+3:(internal)priceIncreaseType(VariablePriceType), paramN+4:(internal)otherConditionLength, paramN+5/paramN+6~paramN+M-1/paramN+M:conditionType/conditionMinValue, paramN+M+1:(internal)faildEventId, paramN+M+2:(internal)faildEventDataLength, paramN+M+3~paramN+M+P: faildEventData, 
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "resource type";
                    dropParam1.gameObject.SetActive(true);
                    txtParam2.gameObject.SetActive(true);
                    txtParam2.text = "resource item count";
                    inputParam2.gameObject.SetActive(true);
                    txtParam3.gameObject.SetActive(true);
                    txtParam3.text = "success event id";
                    inputParam3.gameObject.SetActive(true);
                    selectEventDataList.gameObject.SetActive(true);
                    dropCostType.gameObject.SetActive(true);
                    inputCostIdCount.gameObject.SetActive(true);
                    dropIncreaseType.gameObject.SetActive(true);
                    selectConditionList.gameObject.SetActive(true);
                    dropConditionType.gameObject.SetActive(true);
                    inputConditionMinValue.gameObject.SetActive(true);
                    dropFailedEventType.gameObject.SetActive(true);
                    selectFailedEventDataList.gameObject.SetActive(true);
                    break;
                case EventManager.EventName.PlaySoundBefore:
                    // param0: soundId, param1: lastEventId, param2: lastEventDataLength, param3~paramN: lastEventData
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "sound";
                    dropParam1.gameObject.SetActive(true);
                    options = new List<string>(Game.Config.audios.Count);
                    foreach (var i in Game.Config.audios)
                    {
                        options.Add(i.Value.path.ToString());
                    }
                    dropParam1.AddOptions(options);
                    // TODO : 同 chat / choice
                    dropParam1.value = System.Convert.ToInt32(SelectedKey[0]) - 1;      // TODO : 这里的序号
                    txtParam3.gameObject.SetActive(true);
                    txtParam3.text = "last event";
                    inputParam2.gameObject.SetActive(true);
                    inputParam2.text = SelectedKey[1].ToString();   // TODO : 这里应当嵌套打开另一个 event editor 来选择
                    txtBtnEditEventData.text = "";
                    selectEventDataList.gameObject.SetActive(true);
                    break;
                case EventManager.EventName.OpenNormalDoor:
                    // param0: keyId, see EventManager.OpenNormalDoor
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "key type";
                    dropParam1.gameObject.SetActive(true);
                    break;
                case EventManager.EventName.RemoveEvent:
                    // param0: mapId, param1: posx, param2: posy
                    txtParam1.gameObject.SetActive(true);
                    txtParam1.text = "map id";
                    dropParam1.gameObject.SetActive(true);
                    txtParam2.gameObject.SetActive(true);
                    txtParam2.text = "pos x";
                    inputParam2.gameObject.SetActive(true);
                    txtParam3.gameObject.SetActive(true);
                    txtParam3.text = "pos y";
                    inputParam3.gameObject.SetActive(true);
                    break;
                case EventManager.EventName.RemoveSelf:
                    // no parameter
                    break;
                default:
                    break;
            }
        }

        public void OnClickConfirm()
        {
            var eventName = (EventManager.EventName)EventId;
            switch (eventName)
            {
                case EventManager.EventName.None:
                    // no parameter
                    break;
                case EventManager.EventName.NormalBattle:
                    // param0: canFail, param1: selfUuid
                    break;
                case EventManager.EventName.OpenFreeDoor:
                    // no parameter
                    break;
                case EventManager.EventName.NormalSend:
                    // param0: mapId, param1: posx, param2: posy
                    break;
                case EventManager.EventName.NormalChat:
                    // param0: chatDataIndex
                    break;
                case EventManager.EventName.NormalChoice:
                    // param0: choiceDataIndex
                    break;
                case EventManager.EventName.CallGame:
                    // param0: gameId ( uncompleted )
                    break;
                case EventManager.EventName.GetBaseResourceItem:
                    // param0: resourceType, param1: resourceItemCount, param2:(internal)successEventId, param3:(internal)successEventDataLength, param4~paramN: successEventData, paramN+1:(internal)costType, paramN+2:costCount(paramN+3==0)orCostIncreaseRecordId, paramN+3:(internal)priceIncreaseType(VariablePriceType), paramN+4:(internal)otherConditionLength, paramN+5/paramN+6~paramN+M-1/paramN+M:conditionType/conditionMinValue, paramN+M+1:(internal)faildEventId, paramN+M+2:(internal)faildEventDataLength, paramN+M+3~paramN+M+P: faildEventData, 
                    break;
                case EventManager.EventName.PlaySoundBefore:
                    // param0: soundId, param1: lastEventId, param2: lastEventDataLength, param3~paramN: lastEventData
                    break;
                case EventManager.EventName.OpenNormalDoor:
                    // param0: keyId, see EventManager.OpenNormalDoor
                    break;
                case EventManager.EventName.RemoveEvent:
                    // param0: mapId, param1: posx, param2: posy
                    break;
                case EventManager.EventName.RemoveSelf:
                    // no parameter
                    break;
                default:
                    break;
            }
            // 退出窗口
            ApplyCallback(SelectedKey);
            Destroy(gameObject);
        }

        public Text toggleParam0Text;
        public Toggle toggleParam0;
        public Text txtParam1;
        public Dropdown dropParam1;
        public Text txtParam2;
        public InputField inputParam2;
        public Text txtParam3;
        public InputField inputParam3;
        public Text txtBtnEditEventData;
        public SelectListView selectEventDataList;

        public Dropdown dropCostType;
        public InputField inputCostIdCount;
        public Dropdown dropIncreaseType;
        public SelectListView selectConditionList;
        public Dropdown dropConditionType;
        public InputField inputConditionMinValue;
        public Dropdown dropFailedEventType;
        public SelectListView selectFailedEventDataList;

        public Text txtTitle;
    }

}
