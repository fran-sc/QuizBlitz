using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] Score score;
    [SerializeField] TextMeshProUGUI msgText;

    void Start()
    {
        msgText.text = "¡Juego terminado! \n" 
            + "¡Has respondido " + score.CorrectAnswers + " de " +
            score.QuestionsVisited + " preguntas correctamente.\n";    

        if (score.CorrectAnswers == score.QuestionsVisited)
        {
            msgText.text += "¡Felicidades! Has respondido todas las preguntas correctamente.";
        }
        else if (score.CorrectAnswers > score.QuestionsVisited / 2)
        {
            msgText.text += "¡Buen trabajo! Has respondido más de la mitad de las preguntas correctamente.";
        }
        else
        {
            msgText.text += "¡Sigue practicando! Puedes hacerlo mejor la próxima vez.";
        }
    }
}
