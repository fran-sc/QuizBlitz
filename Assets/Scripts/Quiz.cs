using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    //[SerializeField] List<QuestionSO> questions;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Slider progressBar;

    [SerializeField] GameObject[] answerButtons;

    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    [SerializeField] GameManager gameManager;

    Timer timer;
    bool gotAnswered;
    QuestionSO question;
    Score score;

    List<QuestionSO> questions;

    void Start()
    {
        questions = gameManager.GetQuestions();
        Debug.Log("Preguntas cargadas: " + questions.Count);

        timer = GetComponent<Timer>();

        score = GetComponent<Score>();

        InitalizeProgressBar();

        StartQuestion();
    }

    void InitalizeProgressBar()
    {
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    void StartQuestion()
    {
        GetNextQuestion();

        timer.StartTimer();

        gotAnswered = false;
    }

    void GetNextQuestion()
    {
        if (questions.Count > 0)
        {
            GetRandomQuestion();
            DisplayQuestion();      
            score.AddQuestionVisited();
            progressBar.value = score.QuestionsVisited;
        }
        else
        {
            ///questionText.text = "¡No hay más preguntas!";
            gameManager.GameOver();
        }  
    }
    
    void GetRandomQuestion()
    {
        int randomIndex = Random.Range(0, questions.Count);
        
        question = questions[randomIndex];
        
        questions.RemoveAt(randomIndex);
    }

    void DisplayQuestion()
    {
        // Texto de la pregunta
        questionText.text = question.Question;

        // Respuestas
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.GetAnswer(i);
        }

        // Resetea los sprites de las respuestas
        SetDefaultAnswerSprites();

        // Resetea el estado de los botones
        SetButtonState(true);
    }

    public void OnAnswerSelected(int index)
    {
        if (index == question.CorrectAnswerIndex)
        {
            questionText.text = "¡Correcto!";

            score.AddCorrectAnswer();
        }
        else
        {
            questionText.text = "Ohhh... ¡Incorrecto!";            
        }
        
        ShowCorrectAnswer();

        SetButtonState(false);

        gotAnswered = true;

        timer.CancelTimer();
    }

    void SetButtonState(bool state)
    {
        foreach (var button in answerButtons)
        {
            button.GetComponent<Button>().interactable = state;
        }
    }

    void SetDefaultAnswerSprites()
    {
        foreach (var button in answerButtons)
        {
            button.GetComponentInChildren<Image>().sprite = defaultAnswerSprite;
        }
    }

    void Update()
    {
        if (!gotAnswered && timer.State == Timer.TimerState.Reviewing)
        {
            questionText.text = "¡Tiempo finalizado!";

            SetButtonState(false);

            ShowCorrectAnswer();
        }
        else if (timer.State == Timer.TimerState.ReviewEnded)
        {
            StartQuestion();
        }   

        ShowScore();
    }

    void ShowCorrectAnswer()
    {
        answerButtons[question.CorrectAnswerIndex].GetComponentInChildren<Image>().sprite = correctAnswerSprite;
    }

    void ShowScore()
    {
        //scoreText.text = "Puntuación: " + Mathf.RoundToInt(score.CorrectAnswers/(float)score.QuestionsVisited * 100.0f) + "%";
        scoreText.text = "Puntuación: " + score.CorrectAnswers + "/" + score.QuestionsVisited;
    }

}
