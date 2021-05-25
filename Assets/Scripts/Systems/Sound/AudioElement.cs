using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// 基本のオーディオユニットを定義するサウンドのクラス
/// </summary>
[System.Serializable]
public class AudioElement
{
        #region インスペクタ編集部
    //By Brackeys
    //https://www.youtube.com/watch?v=6OT43pvUyfY

    [Tooltip("名前")]
    public string name;
    [Tooltip("サウンドファイル")]
    public AudioClip clip;
    [Tooltip("音量")]
    [Range(0f, 1f)]
    public float volume;
    [Tooltip("ピッチ")]
    [Range(.1f,3f)]
    public float pitch;
    [Tooltip("ループフラグ")]
    public bool loop;
    [HideInInspector]
    public AudioSource source;
        #endregion
}
