﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.DataEditor.EventEditorPanels
{
    public class PlaySoundBeforeEventEditor : MonoBehaviour
    {
        // param0: soundId, param1: lastEventId, param2: lastEventDataLength, param3~paramN: lastEventData
        void Awake()
        {
            
        }

        Dropdown soundsDropdown;
        Dropdown lastEventDropdown;
    }

}
