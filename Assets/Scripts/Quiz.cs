using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Clase Quiz
// Componente principal del juego de preguntas. Gestiona la lógica completa del quiz:
// selección aleatoria de preguntas, visualización, evaluación de respuestas,
// control del tiempo y transición al fin de partida.
//
// Campos:
// - questionText: texto de la UI donde se muestra la pregunta o el resultado del intento
// - scoreText: texto de la UI donde se muestra la puntuación actual
// - progressBar: barra de progreso que indica cuántas preguntas se han respondido
// - answerButtons: array de botones de respuesta mostrados al jugador
// - defaultAnswerSprite: sprite por defecto de los botones de respuesta
// - correctAnswerSprite: sprite que resalta el botón con la respuesta correcta
// - gameManager: referencia al GameManager para obtener preguntas y gestionar el fin de partida
// - timer: referencia al componente Timer para controlar el tiempo por pregunta
// - gotAnswered: indica si la pregunta actual ya fue respondida por el jugador
// - question: pregunta actualmente mostrada al jugador
// - score: referencia al componente Score para registrar los resultados
// - questions: lista dinámica de preguntas pendientes de mostrar
public class Quiz : MonoBehaviour
{
    //[SerializeField] List<QuestionSO> questions;
    [SerializeField] TextMeshProUGUI questionText;      // Texto de la UI para la pregunta o el resultado
    [SerializeField] TextMeshProUGUI scoreText;         // Texto de la UI para la puntuación actual
    [SerializeField] Slider progressBar;                // Barra de progreso del quiz

    [SerializeField] GameObject[] answerButtons;        // Array de botones de respuesta

    [SerializeField] Sprite defaultAnswerSprite;        // Sprite por defecto de los botones
    [SerializeField] Sprite correctAnswerSprite;        // Sprite que resalta la respuesta correcta
    [SerializeField] GameManager gameManager;           // Referencia al GameManager

    Timer timer;            // Componente Timer para controlar el tiempo por pregunta
    bool gotAnswered;       // Indica si la pregunta actual ya fue respondida
    QuestionSO question;    // Pregunta actualmente mostrada al jugador
    Score score;            // Componente Score para registrar los resultados

    List<QuestionSO> questions;    // Lista dinámica de preguntas pendientes

    // Método Start
    // Inicializa los componentes, carga las preguntas y arranca la primera ronda.
    void Start()
    {
        // Obtener la lista de preguntas desde el GameManager
        questions = gameManager.GetQuestions();
        Debug.Log("Preguntas cargadas: " + questions.Count);

        // Obtener referencias a los componentes necesarios
        timer = GetComponent<Timer>();
        score = GetComponent<Score>();

        // Configurar la barra de progreso e iniciar la primera pregunta
        InitalizeProgressBar();
        StartQuestion();
    }

    // Método InitalizeProgressBar
    // Configura la barra de progreso con el total de preguntas y la resetea a cero.
    void InitalizeProgressBar()
    {
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    // Método StartQuestion
    // Prepara y muestra una nueva pregunta: la obtiene, inicia el temporizador y marca como no respondida.
    void StartQuestion()
    {
        GetNextQuestion();
        timer.StartTimer();
        gotAnswered = false;
    }

    // Método GetNextQuestion
    // Obtiene la siguiente pregunta si quedan disponibles; de lo contrario, termina la partida.
    void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            // Seleccionar y mostrar una pregunta aleatoria de las restantes
            GetRandomQuestion();
            DisplayQuestion();      
            score.AddQuestionVisited();
            progressBar.value = score.QuestionsVisited;
        }
        else
        {
            // No quedan preguntas: activar la pantalla de fin de partida
            gameManager.GameOver();
        }  
    }
    
    // Método GetRandomQuestion
    // Selecciona una pregunta aleatoria de la lista y la elimina para evitar repeticiones.
    void GetRandomQuestion()
    {
        // Obtener un índice aleatorio dentro de las preguntas disponibles
        int randomIndex = Random.Range(0, questions.Count);
        
        // Guardar la pregunta seleccionada y eliminarla de la lista pendiente
        question = questions[randomIndex];
        questions.RemoveAt(randomIndex);
    }

    // Método DisplayQuestion
    // Actualiza la UI con el texto de la pregunta, las respuestas y el estado inicial de los botones.
    void DisplayQuestion()
    {
        // Texto de la pregunta
        questionText.text = question.Question;

        // Mostrar cada opción de respuesta en su botón correspondiente
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.GetAnswer(i);
        }

        // Resetear los sprites de las respuestas al estado por defecto
        SetDefaultAnswerSprites();

        // Habilitar los botones de respuesta para que el jugador pueda interactuar
        SetButtonState(true);
    }

    // Método OnAnswerSelected
    // Evalúa la respuesta elegida por el jugador, actualiza el feedback y cancela el temporizador.
    // Parámetros:
    // - index: índice del botón de respuesta pulsado por el jugador
    public void OnAnswerSelected(int index)
    {
        if (index == question.CorrectAnswerIndex)
        {
            // Respuesta correcta: informar al jugador y registrar el acierto
            questionText.text = "¡Correcto!";
            score.AddCorrectAnswer();
        }
        else
        {
            // Respuesta incorrecta: informar al jugador
            questionText.text = "Ohhh... ¡Incorrecto!";            
        }
        
        // Mostrar la respuesta correcta, deshabilitar botones y detener el temporizador
        ShowCorrectAnswer();
        SetButtonState(false);
        gotAnswered = true;
        timer.CancelTimer();
    }

    // Método SetButtonState
    // Habilita o deshabilita la interactividad de todos los botones de respuesta.
    // Parámetros:
    // - state: true para habilitar los botones, false para deshabilitarlos
    void SetButtonState(bool state)
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Button>().interactable = state;
        }
    }

    // Método SetDefaultAnswerSprites
    // Restablece el sprite de todos los botones de respuesta al sprite por defecto.
    void SetDefaultAnswerSprites()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponentInChildren<Image>().sprite = defaultAnswerSprite;
        }
    }

    // Método Update
    // Comprueba cada fotograma el estado del temporizador y reacciona en consecuencia:
    // muestra "tiempo finalizado" si se agotó sin respuesta, o avanza a la siguiente pregunta.
    void Update()
    {
        if (!gotAnswered && timer.State == Timer.TimerState.Reviewing)
        {
            // El tiempo se agotó sin que el jugador respondiera
            questionText.text = "¡Tiempo finalizado!";
            SetButtonState(false);
            ShowCorrectAnswer();
        }
        else if (timer.State == Timer.TimerState.ReviewEnded)
        {
            // La revisión terminó: pasar a la siguiente pregunta
            StartQuestion();
        }   

        // Actualizar la puntuación en la UI en cada fotograma
        ShowScore();
    }

    // Método ShowCorrectAnswer
    // Cambia el sprite del botón correcto al sprite de respuesta correcta.
    void ShowCorrectAnswer()
    {
        answerButtons[question.CorrectAnswerIndex].GetComponentInChildren<Image>().sprite = correctAnswerSprite;
    }

    // Método ShowScore
    // Actualiza el texto de puntuación en la UI con el número de aciertos sobre el total de preguntas visitadas.
    void ShowScore()
    {
        //scoreText.text = "Puntuación: " + Mathf.RoundToInt(score.CorrectAnswers/(float)score.QuestionsVisited * 100.0f) + "%";
        scoreText.text = "Puntuación: " + score.CorrectAnswers + "/" + score.QuestionsVisited;
    }

}
