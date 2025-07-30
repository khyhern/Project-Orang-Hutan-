using System.Collections;
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
        PlayCutscene(successTimeline);
    }

    public void PlayFail()
    {
        PlayCutscene(failTimeline);
    }

    private void PlayCutscene(TimelineAsset timeline)
    {
        if (director == null || timeline == null) return;

        InputBlocker.IsInputBlocked = true;

        director.playableAsset = timeline;
        director.Play();

        StartCoroutine(WaitForCutsceneToEnd());
    }

    private IEnumerator WaitForCutsceneToEnd()
    {
        yield return new WaitUntil(() => director.state != PlayState.Playing);
        InputBlocker.IsInputBlocked = false;
    }
}
