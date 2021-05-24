using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音声管理マネージャー
/// 各音声の元、再生、停止、音量やピッチの変更、
/// 及びフェードインフェードアウトなどを管理
/// </summary>
public class AudioManeger : MonoBehaviour
{
    //オーディオコントロールスクリプト
    //Original by Brackeys
    //https://www.youtube.com/watch?v=6OT43pvUyfY

    public AudioElement[] sounds;
    public static AudioManeger instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (AudioElement s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }
    void Start()
    {
        Play("Move");
    }

    private void Update()
    {

    }

    AudioElement SearchSound(string name)
    {
        AudioElement s = Array.Find(sounds, AudioElement => AudioElement.name == name);
        if (s == null)
        {
            Debug.Log("AudioElement:" + name + " not found!");
            return null;
        }
        return s;
    }

    public void Play(string name)
    {
        AudioElement s = SearchSound(name);
        if (SearchSound(name) == null)
        {
            return;
        }
        if(s.source.isPlaying == false)
            s.source.Play();
        Debug.Log("Play" + name);
    }

    public void Play(AudioElement sound)
    {
        sound.source.Play();
    }

    //音量をコントロールする関数
    public void SetVolume(string name, float value)
    {
        AudioElement s = SearchSound(name);
        if (SearchSound(name) == null)
        {
            return;
        }
        s.source.volume = value;
        Play(s);
    }

    //ピッチをコントロールする関数
    public void SetPitch(string name, float value)
    {
        AudioElement s = SearchSound(name);
        if (SearchSound(name) == null)
        {
            return;
        }
        s.source.pitch = value;
        Play(s);
    }

    //再生中のサウンドをフェードインやフェードアウトさせる関数
    public void Fade(string name, bool isFadeOut, float second)
    {
        AudioElement s = SearchSound(name);
        StartCoroutine(FadeAudio(s, isFadeOut, second));
    }

    public void PlayRandom(string name, float pitch)
    {
        AudioElement s = SearchSound(name);
        float origin = s.source.pitch;
        float change = UnityEngine.Random.Range(origin - pitch, origin + pitch);
        s.source.pitch = change;
        Play(s);
        StartCoroutine(ResetAudioPitch(s, origin));
    }
    public void Stop(string name)
    {
        AudioElement s = SearchSound(name);
        if (SearchSound(name) == null)
        {
            return;
        }
        s.source.Stop();
    }

    public void StopAllSound()
    {
        foreach(AudioElement s in sounds)
        {
            s.source.Stop();
        }
    }

    IEnumerator FadeAudio(AudioElement sound, bool isFadeOut, float second)
    {
        float t = 0.0f;
        while (t < second)
        {
            if (!isFadeOut){     //フェード院... 「私も同行する」
                sound.source.volume = Mathf.Lerp(0.0f, 1.0f, t / second);
            }
            else{              //フェードアウト
                sound.source.volume = Mathf.Lerp(1.0f, 0.0f, t / second);
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ResetAudioPitch(AudioElement sound,float original)
    {
        while (true)
        {
            if (sound.source.isPlaying)
            {
                yield return null;
            }
            sound.source.pitch = original;
            yield break;
        }
    }
}
