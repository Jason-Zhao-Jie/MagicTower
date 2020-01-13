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
        private void Awake()
        {
            var eventName = (EventManager.EventName)EventId;
            switch(eventName)
            {
                case EventManager.EventName.None:
                    // no parameter
                case EventManager.EventName.NormalBattle:
                    // param0: canFail, param1: selfUuid
                case EventManager.EventName.OpenFreeDoor:
                    // no parameter
                case EventManager.EventName.NormalSend:
                    // param0: mapId, param1: posx, param2: posy
                case EventManager.EventName.NormalChat:
                    // param0: chatDataIndex
                case EventManager.EventName.NormalChoice:
                    // param0: choiceDataIndex
                case EventManager.EventName.CallGame:
                    // param0: gameId ( uncompleted )
                case EventManager.EventName.GetBaseResourceItem:
                    // param0: resourceType, param1: resourceItemCount, param2:(internal)successEventId, param3:(internal)successEventDataLength, param4~paramN: successEventData, paramN+1:(internal)costType, paramN+2:costCount(paramN+3==0)orCostIncreaseRecordId, paramN+3:(internal)priceIncreaseType(VariablePriceType), paramN+4:(internal)otherConditionLength, paramN+5/paramN+6~paramN+M-1/paramN+M:conditionType/conditionMinValue, paramN+M+1:(internal)faildEventId, paramN+M+2:(internal)faildEventDataLength, paramN+M+3~paramN+M+P: faildEventData, 
                case EventManager.EventName.PlaySoundBefore:
                    // param0: soundId, param1: lastEventId, param2: lastEventDataLength, param3~paramN: lastEventData
                case EventManager.EventName.OpenNormalDoor:
                    // param0: keyId, see EventManager.OpenNormalDoor
                case EventManager.EventName.RemoveEvent:
                    // param0: mapId, param1: posx, param2: posy
                case EventManager.EventName.RemoveSelf:
                    // no parameter
                default:
                    break;
            }
        }

        public void OnClickConfirm()
        {
            // 退出窗口
            ApplyCallback(SelectedKey);
            Destroy(gameObject);
        }

        public int EventId {
            get;set;
        }

        public long[] SelectedKey {
            get; set;
        }

        public System.Action<long[]> ApplyCallback { get; set; }

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
    }

}
