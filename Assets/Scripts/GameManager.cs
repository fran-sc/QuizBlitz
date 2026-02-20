using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Clase GameManager
// Gestiona el flujo general de la partida: activación de pantallas y navegación entre escenas.
// Controla la transición entre el canvas del quiz y el canvas de fin de partida.
// Proporciona acceso a la lista de preguntas cargadas por QuestionLoader.
//
// Campos:
// - quizCanvas: canvas de la UI que contiene el juego de preguntas
// - gameOverCanvas: canvas de la UI que muestra la pantalla de fin de partida
// - questionLoader: referencia al componente QuestionLoader para acceder a las preguntas
public class GameManager : MonoBehaviour
{
    [SerializeField] Canvas quizCanvas;        // Canvas con el juego de preguntas
    [SerializeField] Canvas gameOverCanvas;    // Canvas con la pantalla de fin de partida

    QuestionLoader questionLoader;             // Referencia al componente QuestionLoader

    // Método Awake
    // Obtiene la referencia al componente QuestionLoader del mismo GameObject.
    void Awake()
    {
        questionLoader = GetComponent<QuestionLoader>();
    }

    // Método Start
    // Inicializa el estado de los canvas: muestra el quiz y oculta el fin de partida.
    void Start()
    {
        quizCanvas.gameObject.SetActive(true);
        gameOverCanvas.gameObject.SetActive(false);
    }

    // Método GetQuestions
    // Devuelve la lista de preguntas cargadas por QuestionLoader.
    // Retorna: lista de objetos QuestionSO disponibles para la partida
    public List<QuestionSO> GetQuestions()
    {
        return questionLoader.LoadedQuestions;
    }

    // Método GameOver
    // Oculta el canvas del quiz y muestra el canvas de fin de partida.
    public void GameOver()
    {
        quizCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    // Método RestartGame
    // Recarga la escena principal (índice 0) para reiniciar la partida desde cero.
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    // Método QuitGame
    // Cierra la aplicación.
    public void QuitGame()
    {
        Application.Quit();
    }
}
