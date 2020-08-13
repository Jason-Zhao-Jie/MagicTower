using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicTower.Components.DataEditor.EventEditorPanels
{

    public class Common_EventEditor : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public void DispatchUpdateKey() {
            if(GetComponent<GetBaseResourceItemEventEditor>() is var child && child != null) {
                child.OnDataUpdate();
            }
        }

        [HideInInspector]
        public EventEditorPanel parent;
    }

}
