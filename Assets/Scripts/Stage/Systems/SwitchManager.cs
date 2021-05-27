using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour {

    public static SwitchManager instance;

    public bool[] channel = new bool[16];

    public List<bool[]> channels = new List<bool[]>();

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        channel = new bool[16];

        var chan = new bool[16];
        channels.Add(chan);
    }

    public void Goal() {
        var chan = new bool[16];
        for(int i = 0; i < 16; i++) {
            chan[i] = channel[i];
        }
        channels.Add(chan);
    }

    public void ResetStage() {
        if (channels.Count == Player.instance.phase + 1) { //クリアせずにリセット
            for (int i = 0; i < 16; i++) {
                channel[i] = channels[Player.instance.phase][i];
            }
        } else if (channels.Count == Player.instance.phase) { //一個前のフェーズに戻る
            for (int i = 0; i < 16; i++) {
                channel[i] = channels[Player.instance.phase][i];
            }
            channels.RemoveAt(channels.Count - 1);
        }
    }
}
