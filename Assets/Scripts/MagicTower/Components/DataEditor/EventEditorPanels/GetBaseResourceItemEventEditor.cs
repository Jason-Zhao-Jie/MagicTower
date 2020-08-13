using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagicTower.Present.Manager;
using System;

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
            conditionType.AddOptions(ExtendUtils.GetList(Model.ResourceType.Unknown, Model.ResourceType.GreenKey));
            failureEvent.AddOptions(ExtendUtils.GetList(EventManager.EventName.None, EventManager.EventName.RemoveSelf));
            failureEventData.selectedFunc = OnFailureEventDataSelected;
        }

        private void OnSuccessEventDataSelected(int index, bool select) {
            var item = successEventData[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        private void OnFailureEventDataSelected(int index, bool select) {
            var item = failureEventData[index].GetComponent<DefaultSelectableElement>();
            item.Selected = select;
        }

        public void OnDataUpdate() {
            var list = new List<long>();
            list.Add(resourceTypeSelector.value);
            list.Add(Convert.ToInt64(resourceCount.text));
            list.Add(successEvent.value);
            list.Add(successEventData.ItemCount);
            for(int i = 0; i < successEventData.ItemCount; ++i) {
                list.Add(Convert.ToInt64(successEventData[i].GetComponent<DefaultSelectableElement>().text.text));
            }
            list.Add(costType.value);
            list.Add(Convert.ToInt64(costCountOrRecordId.text));
            list.Add(costIncreaseType.value);
            // todo: 这里现在只支持1个条件, 需要后续扩展为不定数个
            list.Add(1);
            list.Add(conditionType.value);
            list.Add(Convert.ToInt64(conditionMinValue.text));
            list.Add(failureEvent.value);
            list.Add(failureEventData.ItemCount);
            for(int i = 0; i < failureEventData.ItemCount; ++i) {
                list.Add(Convert.ToInt64(failureEventData[i].GetComponent<DefaultSelectableElement>().text.text));
            }
            GetComponent<Common_EventEditor>().parent.SelectedKey = list.ToArray();
        }

        public Dropdown resourceTypeSelector;
        public InputField resourceCount;

        public Dropdown successEvent;
        public SelectListView successEventData;

        public Dropdown costType;
        public Dropdown costIncreaseType;
        public InputField costCountOrRecordId;

        public Dropdown conditionType;
        public InputField conditionMinValue;

        public Dropdown failureEvent;
        public SelectListView failureEventData;
    }

}
