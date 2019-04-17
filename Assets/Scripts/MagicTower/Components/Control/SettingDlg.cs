using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control {
    public class SettingDlg : ObjectPool.AViewUnit {
        private const string PREFAB_DIR = "SettingDlg";
        private const int PREFAB_ID = 9;

        private const string OK_STR_KEY = "str_ui_ok";
        private const string CANCEL_STR_KEY = "str_ui_cancel";
        private const string AUDIO_SETTING_TITLE_STR_KEY = "str_ui_audioSettingTitle";
        private const string MUSIC_VOLUME_STR_KEY = "str_ui_musicVolume";
        private const string SOUND_VOLUME_STR_KEY = "str_ui_soundVolume";
        private const string ADS_SETTING_TITLE_STR_KEY = "str_ui_adsSettingTitle";
        private const string ADS_SETTING_README_STR_KEY = "str_ui_adsSettingReadme";
        private const string POP_ADS_TITLE_STR_KEY = "str_ui_popAdsTitle";
        private const string POP_ADS_HIGH_STR_KEY = "str_ui_popAdsHigh";
        private const string POP_ADS_MIDIUM_STR_KEY = "str_ui_popAdsMidium";
        private const string POP_ADS_LOW_STR_KEY = "str_ui_popAdsLow";
        private const string POP_ADS_ONLYONCE_STR_KEY = "str_ui_popAdsOnlyOnce";
        private const string POP_ADS_NONE_STR_KEY = "str_ui_popAdsNone";
        private const string REWARD_ADS_TITLE_STR_KEY = "str_ui_rewardAdsTitle";
        private const string REWARD_ADS_OFF_STR_KEY = "str_ui_rewardAdsOff";
        private const string ANALYTICS_SETTING_TITLE_STR_KEY = "str_ui_analyticsSettingTitle";
        private const string ANALYTICE_SETTING_README_STR_KEY = "str_ui_analyticsSettingReadme";
        private const string ANALYTICS_ON_STR_KEY = "str_ui_analyticsOn";

        public static SettingDlg ShowDialog(GameObject parent) {
            // 弹出战斗框
            var ret = Game.ObjPool.GetAnElement<SettingDlg>(PREFAB_ID, ObjectPool.ElementType.Dialog, GetResourcePath());
            ret.transform.SetParent(parent.transform, false);
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
                i.onValueChanged.AddListener((bool value) => {
                    OnTooglePopAdsChanged(toggle, value);
                });
            }
        }

        // Start is called before the first frame update
        void OnEnable() {
            txtOK.text = Game.Config.StringInternational.GetValue(OK_STR_KEY);
            txtCancel.text = Game.Config.StringInternational.GetValue(CANCEL_STR_KEY);
            txtAudioSettingTitle.text = Game.Config.StringInternational.GetValue(AUDIO_SETTING_TITLE_STR_KEY);
            txtMusicVolume.text = Game.Config.StringInternational.GetValue(MUSIC_VOLUME_STR_KEY);
            txtSoundVolume.text = Game.Config.StringInternational.GetValue(SOUND_VOLUME_STR_KEY);
            txtAdsSettingTitle.text = Game.Config.StringInternational.GetValue(ADS_SETTING_TITLE_STR_KEY);
            txtAdsSettingReadme.text = Game.Config.StringInternational.GetValue(ADS_SETTING_README_STR_KEY);
            txtPopAdsTitle.text = Game.Config.StringInternational.GetValue(POP_ADS_TITLE_STR_KEY);
            txtPopHigh.text = Game.Config.StringInternational.GetValue(POP_ADS_HIGH_STR_KEY);
            txtPopMidium.text = Game.Config.StringInternational.GetValue(POP_ADS_MIDIUM_STR_KEY);
            txtPopLow.text = Game.Config.StringInternational.GetValue(POP_ADS_LOW_STR_KEY);
            txtPopOnlyOnce.text = Game.Config.StringInternational.GetValue(POP_ADS_ONLYONCE_STR_KEY);
            txtPopNone.text = Game.Config.StringInternational.GetValue(POP_ADS_NONE_STR_KEY);
            txtRewardAdsTitle.text = Game.Config.StringInternational.GetValue(REWARD_ADS_TITLE_STR_KEY);
            txtRewardAdsOff.text = Game.Config.StringInternational.GetValue(REWARD_ADS_OFF_STR_KEY);
            txtAnalyticsSettingTitle.text = Game.Config.StringInternational.GetValue(ANALYTICS_SETTING_TITLE_STR_KEY);
            txtAnalyticsSettingReadme.text = Game.Config.StringInternational.GetValue(ANALYTICE_SETTING_README_STR_KEY);
            txtAnalyticsOn.text = Game.Config.StringInternational.GetValue(ANALYTICS_ON_STR_KEY);

            sliderMusicVolume.value = Game.Settings.Settings.musicVolume;
            sliderSoundVolume.value = Game.Settings.Settings.soundVolume;
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
        async System.Threading.Tasks.Task Update() {
            if(task != null) {
                await task;
                task = null;
                RecycleSelf();
            }
        }

        public void OnOK() {
            if (task == null) {
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
                    musicVolume = sliderMusicVolume.value,
                    soundVolume = sliderSoundVolume.value,
                    popAdsFreq = (sbyte)pop,
                    rewardAdsOn = !toggleRewardAdsOff.isOn,
                    analyticsOn = toggleAnalyticsOn.isOn,
                };
                task = Game.Settings.Save();
            }
        }

        public void OnCancel() {
            if (task == null) {
                Present.Manager.AudioManager.MusicVolume = Game.Settings.Settings.musicVolume;
                Present.Manager.AudioManager.SoundVolume = Game.Settings.Settings.soundVolume;
                RecycleSelf();
            }
        }

        public void OnVolumeChanged(bool music) {
            if (task == null) {
                if (music) {
                    Present.Manager.AudioManager.MusicVolume = sliderMusicVolume.value;
                } else {
                    Present.Manager.AudioManager.SoundVolume = sliderSoundVolume.value;
                }
            } else {
                if (music) {
                    sliderMusicVolume.value = Present.Manager.AudioManager.MusicVolume;
                } else {
                    sliderSoundVolume.value = Present.Manager.AudioManager.SoundVolume;
                }
            }
        }

        public void OnTooglePopAdsChanged(Toggle toggle, bool value) {
            if (task == null) {
                if (value) {
                    foreach (var i in popAdsToggles) {
                        if (toggle != i) {
                            i.enabled = true;
                            i.isOn = false;
                        }
                    }
                    toggle.enabled = false;
                }
            } else {
                toggle.isOn = false;
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

        // Private Data
        private System.Threading.Tasks.Task task = null;
    }

}
