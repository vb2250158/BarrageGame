using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[RequireComponent(typeof(PlayableDirector))]
public class DanmuPlayableDirector : MonoBehaviour
{
    public BarrageLauncher barrageLauncher;
    public PlayableDirector playableDirector;

    private void Reset()
    {
        playableDirector = GetComponent<PlayableDirector>();
        barrageLauncher = GetComponent<BarrageLauncher>();

    }

    [ContextMenu("LoadBarrageLauncherData")]
    public void LoadBarrageLauncherData()
    {
        //获取timeline
        TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;

        //创建父轨道进行分组
        GroupTrack groupTrack = null;
        groupTrack = timelineAsset.CreateTrack<GroupTrack>(null, "ShootGroup");

        //创建轨道
        List<DanmuTrack> danmuTracks = new List<DanmuTrack>();
        for (int i = 0; i < barrageLauncher.danmus.Count; i++)
        {
            danmuTracks.Add(timelineAsset.CreateTrack<DanmuTrack>(groupTrack, "Shoot"));
        }


        //遍历所有弹幕轨道进行分组
        for (int i = 0; i < danmuTracks.Count; i++)
        {
            var danmuTrack = danmuTracks[i];
            var data = barrageLauncher.danmus[i];
            playableDirector.SetGenericBinding(danmuTrack, barrageLauncher);

            //分组
            GroupTrack itemGroupTrack = timelineAsset.CreateTrack<GroupTrack>(groupTrack as TrackAsset, "DanmuShoot");
            danmuTrack.SetGroup(itemGroupTrack);
            danmuTrack.SetData(data, itemGroupTrack);
        }
    }
}
