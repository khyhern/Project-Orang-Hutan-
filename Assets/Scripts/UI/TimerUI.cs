using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float remainingTime;
    private bool isRunning;

    public void StartTimer(float duration)
    {
        remainingTime = duration;
        isRunning = true;
        gameObject.SetActive(true);
    }

    public void StopTimer()
    {
        isRunning = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0, remainingTime);

        if (timerText != null)
        {
            timerText.text = FormatTime(remainingTime);
        }

        if (remainingTime <= 0)
        {
            StopTimer();
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00} {milliseconds:000}";
    }
}
