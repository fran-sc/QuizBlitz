using UnityEngine;

[CreateAssetMenu(fileName = "New QuestionSO", menuName = "Scriptable Objects/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    // Pregunta
    [SerializeField][TextArea(3, 10)]
    string question = "Añade aquí el texto de la pregunta";
    public string Question => question;

    // Respuestas
    [SerializeField]
    string[] answers = new string[4];
    public string GetAnswer(int i) => answers[i];

    // Índice de la respuesta correcta
    [SerializeField][Range(0, 3)]
    int correctAnswerIndex = 0;
    public int CorrectAnswerIndex => correctAnswerIndex;

}
