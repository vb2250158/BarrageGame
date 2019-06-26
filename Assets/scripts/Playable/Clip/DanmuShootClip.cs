using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DanmuShootClip : BaseDanmuClip
{
    public DanmuBehaviour danmuBehaviour;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return base.CreatePlayable(graph, owner);
    }
}
