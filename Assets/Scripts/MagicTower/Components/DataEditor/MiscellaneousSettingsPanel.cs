using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {
    public class MiscellaneousSettingsPanel : MonoBehaviour {

        private void Awake()
        {
            inputGameOverBackTime.text = Game.Config.gameoverBackTime.ToString();
        }

        public void SaveValues()
        {
            Game.ShowTip("目前不能更改这些readonly的值, 请直接修改数据文件");
            // Game.Config.gameoverBackTime = int.Parse(inputGameOverBackTime.text);
        }

        public InputField inputGameOverBackTime;
    }
}
