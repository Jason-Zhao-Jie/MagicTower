using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.DataEditor {
    public class LanguageSettingPanel : MonoBehaviour {
        private void Awake() {
            foreach(var i in Game.Config.languages) {
                languageSetControls[i.Value.id - 1].keyInput.text = i.Value.key;
                languageSetControls[i.Value.id - 1].nameInput.text = i.Value.name;
                languageSetControls[i.Value.id - 1].pathInput.text = i.Value.path;
            }
        }

        public void SaveValues(int index = 0) {
            if(index <= 0) {
                for(var i = 1; i <= Game.Config.languages.Count; ++i) {
                    SaveValues(i);
                }
            } else {
                Game.Config.languages[index].key = languageSetControls[index - 1].keyInput.text;
                Game.Config.languages[index].name = languageSetControls[index - 1].nameInput.text;
                Game.Config.languages[index].path = languageSetControls[index - 1].pathInput.text;
            }
        }

        public void OnSaveExit() {
            SaveValues();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        [System.Serializable]
        public struct LanguageSetControls {
            public InputField keyInput;
            public InputField nameInput;
            public InputField pathInput;
        }
        public LanguageSetControls[] languageSetControls;
    }
}
