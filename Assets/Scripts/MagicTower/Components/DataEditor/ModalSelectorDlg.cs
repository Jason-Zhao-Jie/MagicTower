using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil.Components;

namespace MagicTower.Components.DataEditor
{

    public class ModalSelectorDlg : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            ModalList.selectedFunc = OnModalSelected;
            foreach (var v in Game.Config.modals)
            {
                var item = ModalList.PushbackDefaultItem();
                item.transform.Find("Image").GetComponent<Image>().sprite = Game.GetMods(v.Value.prefabPath)[0];
                item.transform.Find("Image").GetComponent<Button>().onClick.RemoveAllListeners();
                item.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => ModalList.Select(item));
                item.transform.Find("Text").GetComponent<Text>().text = Game.Config.StringInternational.GetValue(v.Value.name);
                item.GetComponent<UserData>().SetIntegerData(v.Value.id);
                item.GetComponent<UserData>().SetStringData(v.Value.name);
            }
        }

        private void OnModalSelected(int index, bool selected)
        {
            ModalList[index].GetComponent<Image>().color = selected ? Color.blue : whiteHalf;
            if (selected)
            {
                SelectedModal.sprite = ModalList[index].transform.Find("Image").GetComponent<Image>().sprite;
                SelectedModalName.text = Game.Config.StringInternational.GetValue(ModalList[index].GetComponent<UserData>().GetStringData());
            }
        }

        public void OnClickCancel()
        {
            Destroy(gameObject);
        }

        public void OnClickOK()
        {
            // 退出窗口
            ApplyCallback(SelectedKey);
            Destroy(gameObject);
        }

        public int SelectedKey {
            get {
                return ModalList.SelectedItem.GetComponent<UserData>().GetIntegerData();
            }
            set {
                foreach (var i in ModalList)
                {
                    if (i.GetComponent<UserData>().GetIntegerData() == value)
                    {
                        NowModal.sprite = i.transform.Find("Image").GetComponent<Image>().sprite;
                        NowModalName.text = Game.Config.StringInternational.GetValue(i.GetComponent<UserData>().GetStringData());
                        ModalList.Select(i);
                        ModalList.ScrollToItem(i);
                    }
                }
            }
        }

        public System.Action<int> ApplyCallback { get; set; }


        public RectTransform NowShowing;
        public Image NowModal;
        public Text NowModalName;
        public RectTransform SelectedShowing;
        public Image SelectedModal;
        public Text SelectedModalName;
        public SelectListView ModalList;

        private static readonly Color whiteHalf = new Color(1, 1, 1, 0.5f);
    }
}
