using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DanmuBehaviour : PlayableBehaviour
{
    

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
    }
    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
    }
    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
    }
    public override void OnPlayableDestroy(Playable playable)
    {
        base.OnPlayableDestroy(playable);
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        //Debug.Log("OnBehaviourPlay:" + info.deltaTime);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        //Debug.Log("OnBehaviourPause" + info.deltaTime);
    }
    public override void PrepareData(Playable playable, FrameData info)
    {
        base.PrepareData(playable, info);
        //Debug.Log("PrepareData" + info.deltaTime);
    }

    /// <summary>
    /// 播放时
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        Debug.Log(string.Format("id:{0}(PrepareFrame) \n weight:{1} Time:{2}", info.frameId, info.weight, playable.GetTime()));
    }

}
