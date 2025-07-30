using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset successTimeline;
    [SerializeField] private TimelineAsset failTimeline;

    public void PlaySuccess()
    {
        director.playableAsset = successTimeline;
        director.Play();
    }

    public void PlayFail()
    {
        director.playableAsset = failTimeline;
        director.Play();
    }
}
