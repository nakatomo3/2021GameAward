using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChannelBase : DetailBase {

	public static uint channelMax = 16;

	private uint _channel;
	public uint channel {
		get { return _channel; }
		set { _channel = (value + channelMax) % channelMax; }
	}
}
