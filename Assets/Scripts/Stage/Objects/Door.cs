using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Door : ChannelBase
{

    public Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SwitchManager.instance.channel[channel] == true) {
            Color color = render.material.color;
            color.a = 0.6f;
            render.material.color = color;
        } else {
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
        }
    }

    public override string ToFileString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("B_Door");
        sb.AppendLine("pos:" + Mathf.CeilToInt(transform.position.x) + "," + Mathf.CeilToInt(transform.position.z));
        sb.AppendLine("channel:" + channel);
        return sb.ToString();
    }
    public override string ToEditorString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("チャンネル：　" + channel);
        return sb.ToString();
    }
}
