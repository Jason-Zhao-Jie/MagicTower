using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArmyAnt.ViewUtil;

namespace MagicTower.Components.Control {
    public class SettingDlg : MonoBehaviour {
        private const string OK_STR_KEY = "str_ui_ok";
        private const string CANCEL_STR_KEY = "str_ui_cancel";
        private const string AUDIO_SETTING_TITLE_STR_KEY = "str_ui_audioSettingTitle";
        private const string MUSIC_VOLUME_STR_KEY = "str_ui_musicVolume";
        private const string SOUND_VOLUME_STR_KEY = "str_ui_soundVolume";
        private const string OPERATION_SETTING_STR_KEY = "str_ui_operationSetting";
        private const string VIRTUAL_JOYSTICKS_STR_KEY = "str_ui_virtualJoysticks";
        private const string AUTO_FIND_BEST_ROAD_STR_KEY = "str_ui_autoFindBestRoad";
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
            txtOperationSettingTitle.text = Game.Config.StringInternational.GetValue(OPERATION_SETTING_STR_KEY);
            txtVirtualJoysticks.text = Game.Config.StringInternational.GetValue(VIRTUAL_JOYSTICKS_STR_KEY);
            txtAutoFindBestRoad.text = Game.Config.StringInternational.GetValue(AUTO_FIND_BEST_ROAD_STR_KEY);
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
            toggleVirtualJoysticks.isOn = Game.Settings.Settings.virtualJoysticksOn;
            toggleAutoFindBestRoad.isOn = Game.Settings.Settings.autoFindBestRoad;
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

        public void OnOK() {
            Model.PopAdsFreq pop = Model.PopAdsFreq.Low;
            if(togglePopHigh.isOn) {
                pop = Model.PopAdsFreq.High;
            } else if(togglePopMidium.isOn) {
                pop = Model.PopAdsFreq.Midium;
            } else if(togglePopLow.isOn) {
                pop = Model.PopAdsFreq.Low;
            } else if(togglePopOnlyOnce.isOn) {
                pop = Model.PopAdsFreq.OnlyOnce;
            } else if(togglePopNone.isOn) {
                pop = Model.PopAdsFreq.None;
            }
            Game.Settings.Settings = new Model.Setting {
                musicVolume = sliderMusicVolume.value,
                soundVolume = sliderSoundVolume.value,
                virtualJoysticksOn = toggleVirtualJoysticks.isOn,
                autoFindBestRoad = toggleAutoFindBestRoad.isOn,
                popAdsFreq = (sbyte)pop,
                rewardAdsOn = !toggleRewardAdsOff.isOn,
                analyticsOn = toggleAnalyticsOn.isOn,
            };
            Game.Settings.Save();
            Game.HideUI(UIType.SettingDialog);
        }

        public void OnCancel() {
            Present.Manager.AudioManager.MusicVolume = Game.Settings.Settings.musicVolume;
            Present.Manager.AudioManager.SoundVolume = Game.Settings.Settings.soundVolume;
            Game.HideUI(UIType.SettingDialog);
        }

        public void OnVolumeChanged(bool music) {
            if(music) {
                Present.Manager.AudioManager.MusicVolume = sliderMusicVolume.value;
            } else {
                Present.Manager.AudioManager.SoundVolume = sliderSoundVolume.value;
            }
        }

        public void OnTooglePopAdsChanged(Toggle toggle, bool value) {
            if(value) {
                foreach(var i in popAdsToggles) {
                    if(toggle != i) {
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
        public Text txtOperationSettingTitle;
        public Text txtVirtualJoysticks;
        public Text txtAutoFindBestRoad;
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
        public Toggle toggleVirtualJoysticks;
        public Toggle toggleAutoFindBestRoad;
        public Toggle togglePopLow;
        public Toggle togglePopOnlyOnce;
        public Toggle togglePopNone;
        public Toggle toggleRewardAdsOff;
        public Toggle toggleAnalyticsOn;
    }

}
