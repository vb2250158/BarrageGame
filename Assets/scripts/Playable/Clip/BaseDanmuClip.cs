using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class BaseDanmuClip : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        Playable playable = ScriptPlayable<DanmuBehaviour>.Create(graph);
        return playable;
    }
}
