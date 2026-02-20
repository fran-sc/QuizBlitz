using UnityEngine;

// Clase Score
// Lleva el registro de las respuestas correctas y el número de preguntas visitadas durante la partida.
// Permite incrementar los contadores y reiniciarlos para una nueva partida.
//
// Campos:
// - correctAnswers: número acumulado de respuestas correctas del jugador
// - questionsVisited: número total de preguntas que el jugador ha visto en la partida
public class Score : MonoBehaviour
{
    int correctAnswers = 0;       // Número acumulado de respuestas correctas
    public int CorrectAnswers => correctAnswers;

    int questionsVisited = 0;     // Número total de preguntas vistas por el jugador
    public int QuestionsVisited => questionsVisited;

    // Método AddCorrectAnswer
    // Incrementa en uno el contador de respuestas correctas.
    public void AddCorrectAnswer()
    {
        correctAnswers++;
    }

    // Método AddQuestionVisited
    // Incrementa en uno el contador de preguntas visitadas.
    public void AddQuestionVisited()
    {
        questionsVisited++;
    }

    // Método ResetScore
    // Reinicia ambos contadores a cero para comenzar una nueva partida.
    public void ResetScore()
    {
        correctAnswers = 0;
        questionsVisited = 0;
    }
}
