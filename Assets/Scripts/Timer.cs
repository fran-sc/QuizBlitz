
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] float answerTime;
    [SerializeField] float reviewTime;
    [SerializeField] Image timerImage;

    [SerializeField] Sprite defaultTimerSprite;
    [SerializeField] Sprite reviewTimerSprite;

    float timeLeft;
    float totalTime;

    public enum TimerState
    {
        NotStarted,
        Answering,
        Reviewing,
        ReviewEnded
    }
    TimerState timerState;
    public TimerState State => timerState;

    void Awake()
    {
        timerState = TimerState.NotStarted;
    }

    public void StartTimer()
    {
        timerState = TimerState.Answering;

        timerImage.sprite = defaultTimerSprite;

        ResetTimer(answerTime);
    }

    void Update()
    {
        UpdateTimer();
        UpdateTimerImage();
        UpdateState();
    }

    void ResetTimer(float time)
    {
        timeLeft = time;
        totalTime = time;
        timerImage.fillAmount = 1;
    }

    public void CancelTimer()
    {
        timeLeft = 0;
    }

    void UpdateTimer()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
        }
    }

    void UpdateTimerImage()
    {
        float fillAmount = timeLeft / totalTime;

        timerImage.fillAmount = fillAmount;
    }


    void UpdateState()
    {
        if (timeLeft == 0)
        {
            if (timerState == TimerState.Answering)
            {
                timerState = TimerState.Reviewing;

                timerImage.sprite = reviewTimerSprite;

                ResetTimer(reviewTime);
            }
            else if (timerState == TimerState.Reviewing)
            {
                timerState = TimerState.ReviewEnded;
            }
        }
    }
}


