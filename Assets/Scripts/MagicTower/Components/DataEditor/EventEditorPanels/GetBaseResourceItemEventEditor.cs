using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicTower.Present.Manager;
using System;
using System.Linq;

namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    // param0: resourceType, 
    // param1: resourceItemCount, 

    // param2:(internal)successEventId, 
    // param3:(internal)successEventDataLength, 
    // param4~paramN: successEventData, 

    // paramN+1:(internal)costType, 
    // paramN+2:costCount(paramN+3==0)orCostIncreaseRecordId, 
    // paramN+3:(internal)priceIncreaseType(VariablePriceType), 

    // paramN+4:(internal)otherConditionLength, 
    // paramN+5/paramN+6~paramN+M-1/paramN+M:conditionType/conditionMinValue, 

    // paramN+M+1:(internal)faildEventId, 
    // paramN+M+2:(internal)faildEventDataLength, 
    // paramN+M+3~paramN+M+P: faildEventData, 

    public class GetBaseResourceItemEventEditor : MonoBehaviour
    {
        void Awake() {
            resourceTypeSelector.AddOptions(ExtendUtils.GetList(Model.ResourceType.Unknown, Model.ResourceType.GreenKey));
            successEvent.AddOptions(ExtendUtils.GetList(EventManager.EventName.None, EventManager.EventName.RemoveSelf));
            successEventData.selectedFunc = OnSuccessEventDataSelected;
            costType.AddOptions(ExtendUtils.GetList(Model.ResourceType.Unknown, Model.ResourceType.GreenKey));
            costIncreaseType.AddOptions(ExtendUtils.GetList(Game.VariablePriceType.NoChange, Game.VariablePriceType.KeyStoreDoubling));
            listConditions.selectedFunc = OnConditionSelected;
            conditionType.AddOptions(ExtendUtils.GetList(Model.ResourceType.Unknown, Model.ResourceType.GreenKey));
            failureEvent.AddOptions(ExtendUtils.GetList(EventManager.EventName.None, EventManager.EventName.RemoveSelf));
            failureEventData.selectedFunc = OnFailureEventDataSelected;
            SetData(GetComponent<Common_EventEditor>().parent.SelectedKey);
        }

        private void OnSuccessEventDataSelected(int index, bool select) {
            var item = successEventData[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        private void OnFailureEventDataSelected(int index, bool select) {
            var item = failureEventData[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        private void OnConditionSelected(int index, bool select) {
            btnRemoveCondition.interactable = select;
            btnChangeCondition.interactable = select;
            if(select) {
                var itemData = listConditions[index].GetComponent<UserData>();
                conditionType.value = itemData.GetIntegerData(KEY_CONDITION_TYPE);
                conditionMinValue.text = itemData.GetIntegerData(KEY_CONDITION_VALUE).ToString();
            }
        }

        public void OnEditSuccessEventData() {
            var panel = Instantiate(eventEditor, transform.parent).GetComponent<EventEditorPanel>();
            panel.EventId = successEvent.value;
            panel.SelectedKey = SuccessEventData;
            panel.ApplyCallback = (long[] value) => {
                SuccessEventData = value;
            };
        }

        public void OnEditFailureEventData() {
            var panel = Instantiate(eventEditor, transform.parent).GetComponent<EventEditorPanel>();
            panel.EventId = failureEvent.value;
            panel.SelectedKey = FailureEventData;
            panel.ApplyCallback = (long[] value) => {
                FailureEventData = value;
            };
        }

        public void OnAddCondition() {
            var item = listConditions.PushbackDefaultItem();
            var data = item.gameObject.AddComponent<UserData>();
            data.SetIntegerData(KEY_CONDITION_TYPE, conditionType.value);
            data.SetIntegerData(KEY_CONDITION_VALUE, Convert.ToInt32(conditionMinValue.text));
            listConditions.Select(item);
        }

        public void OnRemoveCondition() {
            var index = listConditions.SelectedIndex;
            listConditions.DeleteItem(index);
        }

        public void OnChangeCondition() {
            var data = listConditions.SelectedItem.GetComponent<UserData>();
            data.SetIntegerData(KEY_CONDITION_TYPE, conditionType.value);
            data.SetIntegerData(KEY_CONDITION_VALUE, Convert.ToInt32(conditionMinValue.text));
        }

        public void OnClearConditions() {
            listConditions.Clear();
        }

        public void OnDataUpdate() {
            var list = new List<long>();
            list.Add(resourceTypeSelector.value);
            list.Add(Convert.ToInt64(resourceCount.text));
            list.Add(successEvent.value);
            list.Add(successEventData.ItemCount);
            list.AddRange(SuccessEventData);
            list.Add(costType.value);
            list.Add(Convert.ToInt64(costCountOrRecordId.text));
            list.Add(costIncreaseType.value);
            list.Add(listConditions.ItemCount);
            foreach(var i in listConditions) {
                var data = i.GetComponent<UserData>();
                list.Add(data.GetIntegerData(KEY_CONDITION_TYPE));
                list.Add(data.GetIntegerData(KEY_CONDITION_VALUE));
            }
            list.Add(failureEvent.value);
            list.Add(failureEventData.ItemCount);
            list.AddRange(FailureEventData);
            GetComponent<Common_EventEditor>().parent.SelectedKey = list.ToArray();
        }

        public void SetData(long[] data) {
            btnRemoveCondition.interactable = false;
            btnChangeCondition.interactable = false;
            int index = 0;
            resourceTypeSelector.value = Convert.ToInt32(data[index++]);
            resourceCount.text = data[index++].ToString();
            successEvent.value = Convert.ToInt32(data[index++]);
            SuccessEventData = data.Skip(index).Take(Convert.ToInt32(data[index++])).ToArray();
            index += successEventData.ItemCount;
            costType.value = Convert.ToInt32(data[index++]);
            costCountOrRecordId.text = data[index++].ToString();
            costIncreaseType.value = Convert.ToInt32(data[index++]);
            var conditionNum = data[index++];
            for(int i = 0; i < conditionNum; ++i) {
                var item = listConditions.PushbackDefaultItem();
                var itemData = item.gameObject.AddComponent<UserData>();
                itemData.SetIntegerData(KEY_CONDITION_TYPE, Convert.ToInt32(data[index++]));
                itemData.SetIntegerData(KEY_CONDITION_VALUE, Convert.ToInt32(data[index++]));
            }
            failureEvent.value = Convert.ToInt32(data[index++]);
            FailureEventData = data.Skip(index).Take(Convert.ToInt32(data[index++])).ToArray();
        }

        private long[] SuccessEventData {
            get {
                var list = new long[successEventData.ItemCount];
                for(int i = 0; i < list.Length; ++i) {
                    list[i] = Convert.ToInt64(successEventData[i].GetComponent<DefaultSelectableElement>().text.text);
                }
                return list;
            }
            set {
                successEventData.Clear();
                if(value != null) {
                    for(int i=0; i < value.Length; ++i) {
                        successEventData.PushbackDefaultItem<DefaultSelectableElement>().text.text = value[i].ToString();
                    }
                }
            }
        }

        private long[] FailureEventData {
            get {
                var list = new long[failureEventData.ItemCount];
                for(int i = 0; i < list.Length; ++i) {
                    list[i] = Convert.ToInt64(failureEventData[i].GetComponent<DefaultSelectableElement>().text.text);
                }
                return list;
            }
            set {
                failureEventData.Clear();
                if(value != null) {
                    for(int i = 0; i < value.Length; ++i) {
                        failureEventData.PushbackDefaultItem<DefaultSelectableElement>().text.text = value[i].ToString();
                    }
                }
            }
        }

        public Dropdown resourceTypeSelector;
        public InputField resourceCount;

        public Dropdown successEvent;
        public SelectListView successEventData;

        public Dropdown costType;
        public Dropdown costIncreaseType;
        public InputField costCountOrRecordId;

        public SelectListView listConditions;
        public Button btnAddCondition;
        public Button btnRemoveCondition;
        public Button btnChangeCondition;
        public Button btnClearCondition;
        public Dropdown conditionType;
        public InputField conditionMinValue;

        public Dropdown failureEvent;
        public SelectListView failureEventData;

        public GameObject eventEditor;

        private const string KEY_CONDITION_TYPE = "conditionType";
        private const string KEY_CONDITION_VALUE = "conditionvalue";
    }

}
