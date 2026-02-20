
using UnityEngine;
using UnityEngine.UI;

// Clase Timer
// Controla el flujo de tiempo del quiz entre las fases de respuesta y revisión.
// Gestiona dos temporizaciones: una para que el jugador responda y otra para revisar la respuesta correcta.
// Actualiza visualmente el relleno circular de la imagen del temporizador.
//
// Campos:
// - answerTime: segundos permitidos para responder una pregunta
// - reviewTime: segundos permitidos para revisar la respuesta correcta
// - timerImage: imagen de UI usada como relleno circular del temporizador
// - defaultTimerSprite: sprite mostrado durante la fase de respuesta
// - reviewTimerSprite: sprite mostrado durante la fase de revisión
// - timeLeft: segundos restantes de la fase activa
// - totalTime: segundos totales de la fase activa, usado para calcular el relleno
// - timerState: estado actual del ciclo de vida del temporizador
public class Timer : MonoBehaviour
{
    [SerializeField] float answerTime;          // Segundos permitidos para responder una pregunta
    [SerializeField] float reviewTime;          // Segundos permitidos para revisar la respuesta correcta
    [SerializeField] Image timerImage;          // Imagen de UI usada como relleno circular del temporizador

    [SerializeField] Sprite defaultTimerSprite; // Sprite mostrado durante la fase de respuesta
    [SerializeField] Sprite reviewTimerSprite;  // Sprite mostrado durante la fase de revisión

    float timeLeft;   // Segundos restantes de la fase activa
    float totalTime;  // Segundos totales de la fase activa, usado para calcular el relleno

    // Enumeración que representa los posibles estados del ciclo de vida del temporizador
    public enum TimerState
    {
        NotStarted,    // El temporizador aún no ha comenzado
        Answering,     // El jugador está en fase de respuesta
        Reviewing,     // Se está mostrando la respuesta correcta
        ReviewEnded    // La fase de revisión ha terminado
    }
    TimerState timerState;            // Estado actual del ciclo de vida del temporizador
    public TimerState State => timerState;

    // Método Awake
    // Inicializa el estado del temporizador antes de que comience el juego.
    void Awake()
    {
        timerState = TimerState.NotStarted;
    }

    // Método StartTimer
    // Inicia la fase de respuesta y reinicia los valores visuales del temporizador.
    public void StartTimer()
    {
        timerState = TimerState.Answering;

        timerImage.sprite = defaultTimerSprite;

        ResetTimer(answerTime);
    }

    // Método Update
    // Actualiza el temporizador, la imagen y el estado en cada fotograma.
    void Update()
    {
        UpdateTimer();
        UpdateTimerImage();
        UpdateState();
    }

    // Método ResetTimer
    // Reinicia los valores de cuenta atrás y el relleno de la UI para una nueva fase.
    // Parámetros:
    // - time: duración en segundos para la nueva fase
    void ResetTimer(float time)
    {
        timeLeft = time;
        totalTime = time;
        timerImage.fillAmount = 1;
    }

    // Método CancelTimer
    // Detiene el temporizador inmediatamente estableciendo el tiempo restante a cero.
    public void CancelTimer()
    {
        timeLeft = 0;
    }

    // Método UpdateTimer
    // Reduce el tiempo restante en cada fotograma asegurando que nunca sea negativo.
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

    // Método UpdateTimerImage
    // Actualiza el relleno radial de la imagen en función del tiempo restante.
    void UpdateTimerImage()
    {
        float fillAmount = timeLeft / totalTime;

        timerImage.fillAmount = fillAmount;
    }

    // Método UpdateState
    // Gestiona las transiciones entre los estados del temporizador cuando el tiempo se agota.
    // Al acabar la fase de respuesta, inicia la fase de revisión.
    // Al acabar la fase de revisión, marca el temporizador como finalizado.
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


