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

    public string[] bgmNameList;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){PlayBGM(0);}
        if (Input.GetKeyDown(KeyCode.Alpha2)){PlayBGM(1);}
        if (Input.GetKeyDown(KeyCode.Alpha3)){PlayBGM(2);}
        if (Input.GetKeyDown(KeyCode.Alpha4)){PlayBGM(3);}
        if (Input.GetKeyDown(KeyCode.Alpha5)){PlayBGM(4);}
        if (Input.GetKeyDown(KeyCode.Alpha6)){PlayBGM(5);}
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
        if (s.source.isPlaying == false)
            s.source.Play();
        Debug.Log("Play" + s.name);
    }

    public void Play(string name, float time)
    {
        AudioElement s = SearchSound(name);
        if (SearchSound(name) == null)
        {
            return;
        }
        if (s.source.isPlaying == false)
            s.source.PlayDelayed(time);
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
        if (!isFadeOut)
        {
            Play(s);
        }
        StartCoroutine(FadeAudio(s, isFadeOut, second,s.volume));
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
        foreach (AudioElement s in sounds)
        {
            s.source.Stop();
        }
    }

    public void FadeAllBGM()
    {
        foreach (var bgm in bgmNameList)
        {
            if (SearchSound(bgm).source.isPlaying)
            {
                Fade(bgm, true, 1.0f);
            }
        }
    }
    void PlayBGM(int bgmNo)
    {
        FadeAllBGM();
        switch (bgmNo)
        {
            case 0:
                {
                    Fade("World1", false, 1.0f);
                    Fade("World1Envior", false, 1.0f);
                }                break;
            case 1:
                {
                    Fade("World2", false, 1.0f);
                    Fade("World2Envior", false, 1.0f);
                }                break;
            case 2:
                {
                    Fade("World3", false, 1.0f);
                    Fade("World3Envior", false, 1.0f);
                }
                break;
            case 3:
                {
                    Fade("World4", false, 1.0f);
                    Fade("World4Envior", false, 1.0f);
                }
                break;
            case 4:
                {
                    Fade("Title", false, 1.0f);
                }
                break;
            case 5:
                {
                    Fade("StageSelect"  ,false,1.0f);
                }
                break;
        }
    }

    IEnumerator FadeAudio(AudioElement sound, bool isFadeOut, float second,float origin)
    {
        float t = 0.0f;
        while (t < second)
        {
            if (!isFadeOut)
            {     //フェード院... 「私も同行する」
                sound.source.volume = Mathf.Lerp(0.0f, origin, t / second);
            }
            else
            {              //フェードアウト
                sound.source.volume = Mathf.Lerp(origin, 0.0f, t / second);
                
            }
            t += Time.deltaTime;
            if (t>second&&isFadeOut)
            {
                sound.source.Stop();
            }
            yield return null;
        }
    }

    IEnumerator ResetAudioPitch(AudioElement sound, float original)
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
