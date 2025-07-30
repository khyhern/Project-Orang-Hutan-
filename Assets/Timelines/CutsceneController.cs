using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset successTimeline;
    [SerializeField] private TimelineAsset failTimeline;

    private bool shouldUnblockInputAfterCutscene = false;

    public void PlaySuccess()
    {
        shouldUnblockInputAfterCutscene = true;
        PlayCutscene(successTimeline);
    }

    public void PlayFail()
    {
        shouldUnblockInputAfterCutscene = false;
        PlayCutscene(failTimeline);
    }

    private void PlayCutscene(TimelineAsset timeline)
    {
        if (director == null || timeline == null)
            return;

        InputBlocker.IsInputBlocked = true;

        director.playableAsset = timeline;
        director.Play();

        StartCoroutine(WaitForCutsceneToEnd());
    }

    private IEnumerator WaitForCutsceneToEnd()
    {
        yield return new WaitUntil(() => director.state != PlayState.Playing);

        if (shouldUnblockInputAfterCutscene)
            InputBlocker.IsInputBlocked = false;
    }
}
