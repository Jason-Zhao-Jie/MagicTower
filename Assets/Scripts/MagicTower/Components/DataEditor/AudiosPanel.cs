using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor {

    public class AudiosPanel : MonoBehaviour {
        void Awake() {
            SetAudioList.selectedFunc = OnAudioSelected;
            UnusedAudioFilesList.selectedFunc = OnResourceFileSelected;
            var files = ArmyAnt.Algorithm.ExtendUtils.GetList(Game.GetAudio().Keys);
            foreach(var i in Game.Config.audios) {
                var item = SetAudioList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i.Value.path;
                item.AddOnclickEvent(() => { SetAudioList.Select(item.GetComponent<RectTransform>()); });
                files.Remove(i.Value.path);
            }
            foreach(var i in files) {
                var item = UnusedAudioFilesList.PushbackDefaultItem<DefaultSelectableElement>();
                item.text.text = i;
                item.AddOnclickEvent(() => { UnusedAudioFilesList.Select(item.GetComponent<RectTransform>()); });
            }
            ResetButtonState();
        }

        public void OnClickUpDown(bool up) {
            var thisItem = SetAudioList.SelectedItem.GetComponent<DefaultSelectableElement>();
            DefaultSelectableElement upperItem;
            if(up) {
                upperItem = SetAudioList[SetAudioList.SelectedIndex - 1].GetComponent<DefaultSelectableElement>();
            } else {
                upperItem = SetAudioList[SetAudioList.SelectedIndex + 1].GetComponent<DefaultSelectableElement>();
            }
            (thisItem.text.text, upperItem.text.text) = (upperItem.text.text, thisItem.text.text);
            SetAudioList.Select(upperItem.GetComponent<RectTransform>());
            ResetButtonState();
        }

        public void OnClickAdd() {
            var item = SetAudioList.PushbackDefaultItem<DefaultSelectableElement>();
            item.text.text = UnusedAudioFilesList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
            item.AddOnclickEvent(() => { SetAudioList.Select(item.GetComponent<RectTransform>()); });
            Destroy(UnusedAudioFilesList.DeleteItem(UnusedAudioFilesList.SelectedIndex));
            if(UnusedAudioFilesList.ItemCount > 0) {
                UnusedAudioFilesList.Select(0);
            }
            ResetButtonState();
        }

        public void OnClickRemove() {
            var item = UnusedAudioFilesList.PushbackDefaultItem<DefaultSelectableElement>();
            item.text.text = SetAudioList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text;
            item.AddOnclickEvent(() => { UnusedAudioFilesList.Select(item.GetComponent<RectTransform>()); });
            Destroy(SetAudioList.DeleteItem(SetAudioList.SelectedIndex));
            if(SetAudioList.ItemCount > 0) {
                SetAudioList.Select(0);
            }
            ResetButtonState();
        }

        public void OnClickPlay(bool unused) {
            Present.Manager.AudioManager.StopAllSounds();
            string path;
            if(unused) {
                path = UnusedAudioFilesList.SelectedItem?.GetComponent<DefaultSelectableElement>()?.text?.text;
            } else {
                path = SetAudioList.SelectedItem?.GetComponent<DefaultSelectableElement>()?.text?.text;
            }
            if(path != null) {
                Present.Manager.AudioManager.PlaySound(path);
            }
        }

        public void OnClickExit() {
            Game.Config.audios.Clear();
            var index = 0;
            foreach(var i in SetAudioList) {
                Game.Config.audios.Add(++index, new Model.Audio() { id = index, path = i.GetComponent<DefaultSelectableElement>().text.text });
            }
            Present.Manager.AudioManager.StopAllSounds();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        private void OnAudioSelected(int index, bool select) {
            leftSelectedId.text = "ID: " + (index + 1);
            SetAudioList[index].GetComponent<DefaultSelectableElement>().Selected = select;
            ResetButtonState();
        }

        private void OnResourceFileSelected(int index, bool select) {
            UnusedAudioFilesList[index].GetComponent<DefaultSelectableElement>().Selected = select;
        }

        private void ResetButtonState() {
            btnUp.interactable = SetAudioList.ItemCount > 0 && SetAudioList.SelectedIndex != 0;
            btnDown.interactable = SetAudioList.ItemCount > 0 && SetAudioList.SelectedIndex != SetAudioList.ItemCount - 1;
            btnAdd.interactable = UnusedAudioFilesList.ItemCount > 0;
            btnRemove.interactable = SetAudioList.ItemCount > 0;
            btnPlayUsed.interactable = SetAudioList.ItemCount > 0;
            btnPlayUnUsed.interactable = UnusedAudioFilesList.ItemCount > 0;
        }

        public SelectListView SetAudioList;
        public SelectListView UnusedAudioFilesList;

        public Text leftSelectedId;
        public Button btnUp;
        public Button btnDown;
        public Button btnAdd;
        public Button btnRemove;
        public Button btnPlayUsed;
        public Button btnPlayUnUsed;
    }

}
