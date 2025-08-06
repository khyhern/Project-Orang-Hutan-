using System.Collections;
using TMPro;
using UnityEngine;

public class SubtitleUI : MonoBehaviour
{
    public static SubtitleUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private float displayDuration = 3f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (subtitleText != null)
            subtitleText.text = "";
    }

    public void ShowSubtitle(string line, float duration = -1f)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowSubtitleRoutine(line, duration > 0 ? duration : displayDuration));
    }

    private IEnumerator ShowSubtitleRoutine(string line, float duration)
    {
        subtitleText.text = line;

        yield return new WaitForSeconds(duration);

        subtitleText.text = "";
        currentRoutine = null;
    }
}
