using TMPro;
using UnityEngine;

// Clase GameOver
// Muestra la pantalla de fin de partida con el resultado del jugador.
// Construye un mensaje personalizado en función del porcentaje de aciertos obtenidos.
//
// Campos:
// - score: referencia al componente Score que contiene los resultados de la partida
// - msgText: campo de texto de la UI donde se muestra el mensaje de resultado
public class GameOver : MonoBehaviour
{
    [SerializeField] Score score;                 // Referencia al componente Score con los resultados de la partida
    [SerializeField] TextMeshProUGUI msgText;      // Campo de texto de la UI para mostrar el mensaje de resultado

    // Método Start
    // Construye y muestra el mensaje de resultado al iniciarse la pantalla de fin de partida.
    // Personaliza el mensaje según si el jugador acertó todas, más de la mitad o menos de la mitad.
    void Start()
    {
        // Mostrar el resultado básico: número de aciertos sobre el total de preguntas
        msgText.text = "¡Juego terminado! \n" 
            + "¡Has respondido " + score.CorrectAnswers + " de " +
            score.QuestionsVisited + " preguntas correctamente.\n";    

        // Seleccionar el mensaje motivacional según el porcentaje de aciertos
        if (score.CorrectAnswers == score.QuestionsVisited)
        {
            // El jugador respondió todas las preguntas correctamente
            msgText.text += "¡Felicidades! Has respondido todas las preguntas correctamente.";
        }
        else if (score.CorrectAnswers > score.QuestionsVisited / 2)
        {
            // El jugador superó la mitad de las preguntas
            msgText.text += "¡Buen trabajo! Has respondido más de la mitad de las preguntas correctamente.";
        }
        else
        {
            // El jugador no llegó a la mitad de las preguntas
            msgText.text += "¡Sigue practicando! Puedes hacerlo mejor la próxima vez.";
        }
    }
}
