using UnityEngine;

// Clase QuestionSO
// ScriptableObject que representa una pregunta del quiz con sus respuestas posibles.
// Almacena el texto de la pregunta, un array de cuatro respuestas y el índice de la respuesta correcta.
// Se crea desde el menú de Unity: Scriptable Objects > QuestionSO.
//
// Campos:
// - question: texto de la pregunta que se muestra al jugador
// - answers: array con las cuatro opciones de respuesta
// - correctAnswerIndex: índice (0-3) que señala la respuesta correcta dentro del array
[CreateAssetMenu(fileName = "New QuestionSO", menuName = "Scriptable Objects/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    // Texto de la pregunta mostrado al jugador
    [SerializeField][TextArea(3, 10)]
    string question = "Añade aquí el texto de la pregunta";
    public string Question => question;

    // Array con las cuatro opciones de respuesta
    [SerializeField]
    string[] answers = new string[4];

    // Método GetAnswer
    // Devuelve el texto de la respuesta en la posición indicada.
    // Parámetros:
    // - i: índice de la respuesta a obtener (0-3)
    public string GetAnswer(int i) => answers[i];

    // Índice (0-3) que señala la respuesta correcta dentro del array
    [SerializeField][Range(0, 3)]
    int correctAnswerIndex = 0;
    public int CorrectAnswerIndex => correctAnswerIndex;

    // Método SetData
    // Asigna todos los datos de una pregunta de forma programática.
    // Usado por QuestionLoader para rellenar instancias creadas en tiempo de ejecución.
    // Parámetros:
    // - question: texto de la pregunta
    // - answers: array de cadenas con las opciones de respuesta
    // - correctAnswerIndex: índice de la respuesta correcta dentro del array
    public void SetData(string question, string[] answers, int correctAnswerIndex)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;
    }

}
