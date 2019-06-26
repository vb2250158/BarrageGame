using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[TrackColor(0.9f, 0.1f, 0.8f)]
[TrackClipType(typeof(BaseDanmuClip))]
[TrackBindingType(typeof(BarrageLauncher))]
public class DanmuTrack : TrackAsset
{
    public void SetData(DanmuShoot data, GroupTrack itemGroupTrack)
    {
        TimelineClip danmuShootClip = CreateClip<DanmuShootClip>();
        danmuShootClip.start = data.startTime;
        danmuShootClip.duration =  data.shootTime;
    }
}
