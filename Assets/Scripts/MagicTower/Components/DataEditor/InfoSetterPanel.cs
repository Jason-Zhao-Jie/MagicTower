using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.Algorithm;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor
{
    public class InfoSetterPanel : MonoBehaviour
    {

        private void Awake()
        {
            infoTitleList.selectedFunc = OnInfoTitleSelected;
            // 设定列表内容
            foreach (var i in Game.Config.infos)
            {
                var item = infoTitleList.PushbackDefaultItem<DefaultSelectableElement>();
                item.GetComponent<Button>().onClick.AddListener(() => { infoTitleList.Select(item.GetComponent<RectTransform>()); });
                item.text.text = Game.Config.StringInternational.GetValue(i.title);
            }
        }

        private void OnInfoTitleSelected(int index, bool selected)
        {
            var item = infoTitleList[index].GetComponent<DefaultSelectableElement>();
            item.Selected = selected;
            if (selected)
            {
                var data = Game.Config.infos[index];
                titleStringText.text = item.text.text;
                titleStringText.GetComponent<UserData>().SetStringData(data.title);
                contentStringText.text = Game.Config.StringInternational.GetValue(data.content);
                contentStringText.GetComponent<UserData>().SetStringData(data.content);
                objBtnRemove.SetActive(infoTitleList.ItemCount > 1);
                objBtnUp.SetActive(index != 0);
                objBtnDown.SetActive(index != infoTitleList.ItemCount - 1);
            }
            else
            {
                SaveValues();
            }
        }

        public void OnEditTitleString()
        {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = titleStringText.GetComponent<UserData>().GetStringData();
            panel.ApplyCallback = (string value) =>
            {
                titleStringText.text = Game.Config.StringInternational.GetValue(value);
                titleStringText.GetComponent<UserData>().SetStringData(value);
                infoTitleList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text = titleStringText.text;
                SaveValues();
            };
        }

        public void OnEditContentString()
        {
            var panel = Instantiate(stringSelector, transform.parent).GetComponent<StringMakerPanel>();
            panel.SelectedKey = contentStringText.GetComponent<UserData>().GetStringData();
            panel.ApplyCallback = (string value) =>
            {
                contentStringText.text = Game.Config.StringInternational.GetValue(value);
                contentStringText.GetComponent<UserData>().SetStringData(value);
                SaveValues();
            };
        }

        public void OnClickAdd()
        {
            Game.ShowTip("暂不支持, 请直接编辑配置文件");
        }

        public void OnClickRemove()
        {
            Game.ShowTip("暂不支持, 请直接编辑配置文件");
        }

        public void OnClickUp()
        {
            (Game.Config.infos[infoTitleList.SelectedIndex], Game.Config.infos[infoTitleList.SelectedIndex - 1]) = ExtendUtils.GetSwap(Game.Config.infos[infoTitleList.SelectedIndex], Game.Config.infos[infoTitleList.SelectedIndex - 1]);
            (infoTitleList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text, infoTitleList[infoTitleList.SelectedIndex - 1].GetComponent<DefaultSelectableElement>().text.text) = ExtendUtils.GetSwap(infoTitleList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text, infoTitleList[infoTitleList.SelectedIndex - 1].GetComponent<DefaultSelectableElement>().text.text);
            titleStringText.GetComponent<UserData>().SetStringData(Game.Config.infos[infoTitleList.SelectedIndex].title);
            contentStringText.GetComponent<UserData>().SetStringData(Game.Config.infos[infoTitleList.SelectedIndex].content);
            infoTitleList.Select(infoTitleList.SelectedIndex - 1);
        }

        public void OnClickDown()
        {
            (Game.Config.infos[infoTitleList.SelectedIndex], Game.Config.infos[infoTitleList.SelectedIndex + 1]) = ExtendUtils.GetSwap(Game.Config.infos[infoTitleList.SelectedIndex], Game.Config.infos[infoTitleList.SelectedIndex + 1]);
            (infoTitleList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text, infoTitleList[infoTitleList.SelectedIndex + 1].GetComponent<DefaultSelectableElement>().text.text) = ExtendUtils.GetSwap(infoTitleList.SelectedItem.GetComponent<DefaultSelectableElement>().text.text, infoTitleList[infoTitleList.SelectedIndex + 1].GetComponent<DefaultSelectableElement>().text.text);
            titleStringText.GetComponent<UserData>().SetStringData(Game.Config.infos[infoTitleList.SelectedIndex].title);
            contentStringText.GetComponent<UserData>().SetStringData(Game.Config.infos[infoTitleList.SelectedIndex].content);
            infoTitleList.Select(infoTitleList.SelectedIndex + 1);
        }

        public void OnSaveExit()
        {
            SaveValues();
            Game.ShowDataEditor();
            Destroy(gameObject);
        }

        private void SaveValues()
        {
            Game.Config.infos[infoTitleList.SelectedIndex].title = titleStringText.GetComponent<UserData>().GetStringData();
            Game.Config.infos[infoTitleList.SelectedIndex].content = contentStringText.GetComponent<UserData>().GetStringData();
        }

        public SelectListView infoTitleList;
        public Text titleStringText;
        public Text contentStringText;
        public GameObject stringSelector;

        public GameObject objBtnAdd;
        public GameObject objBtnRemove;
        public GameObject objBtnUp;
        public GameObject objBtnDown;
    }
}
