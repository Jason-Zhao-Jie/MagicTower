using System.Collections.Generic;

namespace MagicTower.Present.Manager {
    /// <summary>
    /// 处理游戏中全部的音乐播放和音效播放的类。
    /// 由于游戏框架相对简单，为了防止创建过多生命周期太短的音效组件，本游戏采用全局UI音效，界面固有UI绑上若干个AudioClip，然后用本类管理
    /// </summary>
    public static class AudioManager {
        /// <summary>
        /// 获得物品时播放的音效id
        /// </summary>
        public const int itemGetSound = 20;
        /// <summary>
        /// 脚步声音效id
        /// </summary>
        public const int stepSound = 56;
        /// <summary>
        /// 开门音效id
        /// </summary>
        public const int openDoorSound = 53;
        /// <summary>
        /// 上下梯音效id
        /// </summary>
        public const int stairSound = 55;
        /// <summary>
        /// 错误、动作无效的提示音效id
        /// </summary>
        public const int disableSound = 17;
        /// <summary>
        /// Game Over 音效id
        /// </summary>
        public const int gameoverSound = 4;
        /// <summary>
        /// 设置或获取音乐播放源组件，音乐播放源组件只能有一个，新设置的将代替原来的
        /// </summary>
        /// <value>要播放音乐的声音组件</value>
        public static UnityEngine.AudioSource MusicSource {
            get { return musicSource; }
            set {
                musicSource = value;
                musicSource.volume = musicVolume;
                musicSource.mute = musicMute;
                musicId = -1;
            }
        }
        /// <summary>
        /// 添加音效组件，音效组件有多个，调用此方法添加新的音效播放组件
        /// </summary>
        /// <param name="source">Source.</param>
        public static void AddSoundSource(UnityEngine.AudioSource source) {
            soundSource.Add(source);
            source.volume = soundVolume;
            source.mute = soundMute;
        }
        /// <summary>
        /// 清除所有的音效播放组件，仅用于切换场景时调用
        /// </summary>
        public static void ClearSoundSource() {
            soundSource.Clear();
        }
        /// <summary>
        /// 循环播放背景音乐，这将覆盖之前的背景音乐
        /// 如果正在播放的背景音乐与此背景音乐相同, 则不会覆盖也不会重新从头播放
        /// </summary>
        /// <returns><c>true</c>, if music loop was played, <c>false</c> otherwise.</returns>
        /// <param name="id">音乐id</param>
        public static void PlayMusicLoop(int id, bool forceReset = false) {
            if(id == musicId && !forceReset) {
                if(!musicSource.isPlaying) {
                    musicSource.Play();
                }
            } else {
                musicSource.clip = Game.GetAudio(Game.Config.audios[id].path);
                musicSource.Play();
            }
            musicId = id;
        }
        /// <summary>
        /// 按列表播放背景音乐 （未实现，暂不需要此功能）
        /// </summary>
        /// <returns><c>true</c>, if music songs was played, <c>false</c> otherwise.</returns>
        /// <param name="idList">音乐id列表</param>
        /// <param name="loopTimes">要循环的次数，不为正数表示一直循环播放</param>
        public static bool PlayMusicList(int[] idList, int loopTimes = 1) {
            // TODO: Realize the list playing function
            return false;
        }
        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        /// <returns><c>true</c>, if music was stoped, <c>false</c> otherwise.</returns>
        public static void StopMusic() => musicSource.Stop();
        /// <summary>
        /// 播放音效，不影响背景音乐和其他音效，除非同时播放的音效数量多于注册的音效组件数量
        /// </summary>
        /// <returns><c>true</c>, if sound was played, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public static bool PlaySound(int id) => PlaySound(Game.Config.audios[id].path);
        public static bool PlaySound(string path) {
            if(soundSource.Count <= index)
                index = 0;
            if(soundSource.Count <= 0)
                return false;
            soundSource[index].clip = Game.GetAudio(path);
            soundSource[index].Play();
            ++index;
            return true;
        }
        /// <summary>
        /// 停止场景上的全部音效
        /// </summary>
        public static void StopAllSounds() {
            foreach(var i in soundSource) {
                i.Stop();
            }
        }
        /// <summary>
        /// 使背景音乐静音，这不会令音乐停止
        /// </summary>
        /// <param name="mute">If set to <c>true</c> mute.</param>
        public static bool MusicMute {
            get {
                return musicMute;
            }
            set {
                musicMute = value;
                if (musicSource != null) {
                    musicSource.mute = value;
                }
            }
        }
        /// <summary>
        /// 使所有音效静音
        /// </summary>
        /// <param name="mute">If set to <c>true</c> mute.</param>
        public static bool SoundMute{
            get {
                return soundMute;
            }
            set {
                soundMute = value;
                foreach (var v in soundSource) {
                    v.mute = value;
                }
            }
        }
        /// <summary>
        /// 设定背景音乐音量
        /// </summary>
        /// <param name="volume">Volume.</param>
        public static float MusicVolume {
            get {
                return musicVolume;
            }
            set {
                musicVolume = value;
                if (musicSource != null) {
                    musicSource.volume = value;
                }
            }
        }
        /// <summary>
        /// 设定音效音量
        /// </summary>
        /// <param name="volume">Volume.</param>
        public static float SoundVolume {
            get {
                return soundVolume;
            }
            set {
                soundVolume = value;
                foreach (var v in soundSource) {
                    v.volume = value;
                }
            }
        }

        private static UnityEngine.AudioSource musicSource;
        private static List<UnityEngine.AudioSource> soundSource = new List<UnityEngine.AudioSource>();
        private static bool musicMute = false;
        private static bool soundMute = false;
        private static float musicVolume = 100;
        private static float soundVolume = 100;
        private static int index = 0;
        private static int musicId = -1;
    }

}
