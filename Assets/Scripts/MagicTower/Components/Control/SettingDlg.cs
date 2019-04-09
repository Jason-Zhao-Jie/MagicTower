using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control {
    public class SettingDlg : ObjectPool.AViewUnit {
        public const string PREFAB_DIR = "SettingDlg";
        public const int PREFAB_ID = 9;

        public static SettingDlg ShowDialog(GameObject parent) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<SettingDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.transform.SetParent(parent.transform, false);
            ret.transform.SetSiblingIndex(2);
            ret.transform.localPosition = new Vector3(0, 0, ret.transform.localPosition.z);
            return ret;
        }

        public override string ResourcePath => Model.Dirs.DIALOG_DIR + PREFAB_DIR;
        public static string GetResourcePath() => Model.Dirs.DIALOG_DIR + PREFAB_DIR;

        public override ObjectPool.ElementType GetPoolTypeId() {
            return ObjectPool.ElementType.Dialog;
        }

        public override bool OnCreate(ObjectPool.ElementType tid, int elemId, string resourcePath) {
            return true;
        }

        public override void OnReuse(ObjectPool.ElementType tid, int elemId) {

        }

        public override bool OnUnuse(ObjectPool.ElementType tid, int elemId) {
            return true;
        }

        public override bool RecycleSelf() {
            return Game.ObjPoolRecycleSelf(this);
        }

        void Awake() {
            foreach (var i in popAdsToggles) {
                var toggle = i;
                i.onValueChanged.AddListener( (bool value) => {
                    OnTooglePopAdsChanged(toggle, value);
                });
            }
            // TODO: load international strings
        }

        // Start is called before the first frame update
        void OnEnable() {
            sliderMusicVolume.value = Game.Settings.Settings.musicVolume / 100.0f;
            sliderSoundVolume.value = Game.Settings.Settings.soundVolume / 100.0f;
            togglePopHigh.isOn = false;
            togglePopMidium.isOn = false;
            togglePopLow.isOn = false;
            togglePopOnlyOnce.isOn = false;
            togglePopNone.isOn = false;
            switch ((Model.PopAdsFreq)Game.Settings.Settings.popAdsFreq) {
                case Model.PopAdsFreq.High:
                    togglePopHigh.isOn = true;
                    break;
                case Model.PopAdsFreq.Midium:
                    togglePopMidium.isOn = true;
                    break;
                case Model.PopAdsFreq.Low:
                    togglePopLow.isOn = true;
                    break;
                case Model.PopAdsFreq.OnlyOnce:
                    togglePopOnlyOnce.isOn = true;
                    break;
                case Model.PopAdsFreq.None:
                    togglePopNone.isOn = true;
                    break;
            }
            toggleRewardAdsOff.isOn = !Game.Settings.Settings.rewardAdsOn;
            toggleAnalyticsOn.isOn = Game.Settings.Settings.analyticsOn;
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void OnOK() {
            Model.PopAdsFreq pop = Model.PopAdsFreq.Low;
            if (togglePopHigh.isOn) {
                pop = Model.PopAdsFreq.High;
            } else if (togglePopMidium.isOn) {
                pop = Model.PopAdsFreq.Midium;
            } else if (togglePopLow.isOn) {
                pop = Model.PopAdsFreq.Low;
            } else if (togglePopOnlyOnce.isOn) {
                pop = Model.PopAdsFreq.OnlyOnce;
            } else if (togglePopNone.isOn) {
                pop = Model.PopAdsFreq.None;
            }
            Game.Settings.Settings = new Model.Setting {
                musicVolume = System.Convert.ToInt32(sliderMusicVolume.value * 100),
                soundVolume = System.Convert.ToInt32(sliderSoundVolume.value * 100),
                popAdsFreq = (sbyte)pop,
                rewardAdsOn = !toggleRewardAdsOff.isOn,
                analyticsOn = toggleAnalyticsOn.isOn,
            };
            RecycleSelf();
        }

        public void OnCancel() {
            RecycleSelf();
        }

        public void OnTooglePopAdsChanged(Toggle toggle, bool value) {
            if (value) {
                foreach (var i in popAdsToggles) {
                    if (toggle != i) {
                        i.enabled = true;
                        i.isOn = false;
                    }
                }
                toggle.enabled = false;
            }
        }

        IEnumerable<Toggle> popAdsToggles {
            get {
                yield return togglePopHigh;
                yield return togglePopMidium;
                yield return togglePopLow;
                yield return togglePopOnlyOnce;
                yield return togglePopNone;
            }
        }

        // Out of frame
        public ScrollRect content;
        public Button btnOK;
        public Text txtOK;
        public Button btnCancel;
        public Text txtCancel;

        // Texts
        public Text txtAudioSettingTitle;
        public Text txtMusicVolume;
        public Text txtSoundVolume;
        public Text txtAdsSettingTitle;
        public Text txtAdsSettingReadme;
        public Text txtPopAdsTitle;
        public Text txtPopHigh;
        public Text txtPopMidium;
        public Text txtPopLow;
        public Text txtPopOnlyOnce;
        public Text txtPopNone;
        public Text txtRewardAdsTitle;
        public Text txtRewardAdsOff;
        public Text txtAnalyticsSettingTitle;
        public Text txtAnalyticsSettingReadme;
        public Text txtAnalyticsOn;

        // Settings
        public Slider sliderMusicVolume;
        public Slider sliderSoundVolume;
        public Toggle togglePopHigh;
        public Toggle togglePopMidium;
        public Toggle togglePopLow;
        public Toggle togglePopOnlyOnce;
        public Toggle togglePopNone;
        public Toggle toggleRewardAdsOff;
        public Toggle toggleAnalyticsOn;
    }

}
