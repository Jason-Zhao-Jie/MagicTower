using System.Collections.Generic;

/// <summary>
/// 处理游戏中全部的音乐播放和音效播放的类。
/// 由于游戏框架相对简单，为了防止创建过多生命周期太短的音效组件，本游戏采用全局UI音效，界面固有UI绑上若干个AudioClip，然后用本类管理
/// </summary>
public class AudioManager
{
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
    /// 设置或获取音乐播放源组件，音乐播放源组件只能有一个，新设置的将代替原来的
    /// </summary>
    /// <value>要播放音乐的声音组件</value>
    public UnityEngine.AudioSource MusicSource
    {
        get { return musicSource; }
        set { musicSource = value; musicId = -1; }
    }
    /// <summary>
    /// 添加音效组件，音效组件有多个，调用此方法添加新的音效播放组件
    /// </summary>
    /// <param name="source">Source.</param>
    public void AddSoundSource(UnityEngine.AudioSource source) {
        soundSource.Add(source);
        source.volume = soundVolume;
        source.mute = soundMute;
    }
    /// <summary>
    /// 清除所有的音效播放组件，仅用于切换场景时调用
    /// </summary>
    public void ClearSoundSource() {
        soundSource.Clear();
    }
    /// <summary>
    /// 循环播放背景音乐，这将覆盖之前的背景音乐
    /// </summary>
    /// <returns><c>true</c>, if music loop was played, <c>false</c> otherwise.</returns>
    /// <param name="id">音乐id</param>
    public bool PlayMusicLoop(int id)
    {
        if (id == musicId)
            return true;
        musicSource.clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(Constant.AUDIO_DIR + Game.Config.audios[id].path);
        musicSource.Play();
        musicId = id;
        return true;
    }
    /// <summary>
    /// 按列表播放背景音乐 （未实现，暂不需要此功能）
    /// </summary>
    /// <returns><c>true</c>, if music songs was played, <c>false</c> otherwise.</returns>
    /// <param name="idList">音乐id列表</param>
    /// <param name="loopTimes">要循环的次数，不为正数表示一直循环播放</param>
    public bool PlayMusicList(int[] idList, int loopTimes = 1)
    {
        // TODO: Realize the list playing function
        return false;
    }
    /// <summary>
    /// 停止播放背景音乐
    /// </summary>
    /// <returns><c>true</c>, if music was stoped, <c>false</c> otherwise.</returns>
    public bool StopMusic()
    {
        musicSource.Stop();
        return true;
    }
    /// <summary>
    /// 播放音效，不影响背景音乐和其他音效，除非同时播放的音效数量多于注册的音效组件数量
    /// </summary>
    /// <returns><c>true</c>, if sound was played, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
    /// <param name="times">Times.</param>
    public bool PlaySound(int id, int times = 1)
	{
        if (soundSource.Count <= index)
            index = 0;
        if (soundSource.Count <= 0)
            return false;
        soundSource[index].clip = UnityEngine.Resources.Load<UnityEngine.AudioClip>(Constant.AUDIO_DIR + Game.Config.audios[id].path);
        soundSource[index].Play();
        ++index;
        return true;
    }
    /// <summary>
    /// 使背景音乐静音，这不会令音乐停止
    /// </summary>
    /// <param name="mute">If set to <c>true</c> mute.</param>
    public void SetMusicMute(bool mute)
    {
        musicSource.mute = mute;
    }
    /// <summary>
    /// 使所有音效静音
    /// </summary>
    /// <param name="mute">If set to <c>true</c> mute.</param>
    public void SetSoundMute(bool mute)
    {
        soundMute = mute;
        foreach(var v in soundSource) {
            v.mute = mute;
        }
    }
    /// <summary>
    /// 设定背景音乐音量
    /// </summary>
    /// <param name="volume">Volume.</param>
    public void SetMusicVolume(int volume)
    {
        musicSource.volume = volume;
    }
    /// <summary>
    /// 设定音效音量
    /// </summary>
    /// <param name="volume">Volume.</param>
    public void SetSoundVolume(int volume) {
        soundVolume = volume;
        foreach (var v in soundSource) {
            v.volume = volume;
        }
    }

    private UnityEngine.AudioSource musicSource;
    private List<UnityEngine.AudioSource> soundSource = new List<UnityEngine.AudioSource>();
    private bool soundMute = false;
    private int soundVolume = 100;
    private int index = 0;
    private int musicId = -1;
}
