using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;
using MagicTower.Components.Unit;
using MagicTower.Present.Manager;

namespace MagicTower.Components.DataEditor {
    public class DataEditor : MonoBehaviour {
        // 各个按钮回调
        public void OnBtnClick(GameObject panel) {
            if(panel != null) {
                Instantiate(panel, transform.parent);
                Game.HideUI(UIType.DataEditor);
            }
        }

        // btnSave 按钮回调
        public void OnSave() {
            string result = Game.Config.SaveData();
            var path = ArmyAnt.Manager.IOManager.SaveToFile(System.Text.Encoding.UTF8.GetBytes(result), "GameData.json");
            Game.ShowTip("已保存成功！路径：" + path);
        }

        // btnExit 按钮回调
        public void OnExit() {
            Game.StopAndBackToStart();
            Game.HideUI(UIType.DataEditor);
        }
    }

}
