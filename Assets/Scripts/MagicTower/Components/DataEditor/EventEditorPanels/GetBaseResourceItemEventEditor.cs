using ArmyAnt.ViewUtil.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    // param0: resourceType, param1: resourceItemCount, 
    // param2:(internal)successEventId, param3:(internal)successEventDataLength, param4~paramN: successEventData, 
    // paramN+1:(internal)costType, paramN+2:costCount(paramN+3==0)orCostIncreaseRecordId, paramN+3:(internal)priceIncreaseType(VariablePriceType), 
    // paramN+4:(internal)otherConditionLength, paramN+5/paramN+6~paramN+M-1/paramN+M:conditionType/conditionMinValue, 
    // paramN+M+1:(internal)faildEventId, paramN+M+2:(internal)faildEventDataLength, paramN+M+3~paramN+M+P: faildEventData, 

    public class GetBaseResourceItemEventEditor : MonoBehaviour
    {
        void Awake()
        {

        }

        public Dropdown resourceTypeSelector;
        public Input resourceCount;
        public Dropdown successEvent;
        public SelectListView successEventData;
        public Dropdown costType;
        public Dropdown costIncreaseType;
        public Input costCountOrRecordId;
        public Dropdown conditionType;
        public Input conditionMinValue;
        public Dropdown failureEvent;
        public SelectListView failureEventData;
    }

}
