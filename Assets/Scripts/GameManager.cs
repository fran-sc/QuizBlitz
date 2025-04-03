using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Canvas quizCanvas;
    [SerializeField] Canvas gameOverCanvas;

    QuestionLoader questionLoader;

    void Awake()
    {
        questionLoader = GetComponent<QuestionLoader>();
    }

    void Start()
    {
        quizCanvas.gameObject.SetActive(true);
        gameOverCanvas.gameObject.SetActive(false);
    }

    public List<QuestionSO> GetQuestions()
    {
        return questionLoader.LoadedQuestions;
    }

    public void GameOver()
    {
        quizCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
