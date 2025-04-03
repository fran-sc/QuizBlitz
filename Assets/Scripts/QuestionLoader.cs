using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestionLoader : MonoBehaviour
{
    [System.Serializable]
    private class QuestionData
    {
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
    }

    [System.Serializable]
    private class QuestionList
    {
        public List<QuestionData> questions;
    }

    public List<QuestionSO> LoadedQuestions { get; private set; }

    [SerializeField] string jsonFileName = "questions.json";

    void Awake()
    {
        LoadQuestionsFromJSON();
    }

    void LoadQuestionsFromJSON()
    {
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file not found at: " + jsonFilePath);
            Application.Quit();
            return;
        }

        string jsonData = File.ReadAllText(jsonFilePath);
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogError("JSON file is empty or not valid.");
            Application.Quit();
            return;
        }

        QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonData);
        if (questionList == null || questionList.questions.Count == 0)
        {
            Debug.LogError("Failed to parse JSON data or no questions found.");
            Application.Quit();
            return;
        }

        LoadedQuestions = new List<QuestionSO>();
        foreach (var questionData in questionList.questions)
        {
            QuestionSO questionSO = ScriptableObject.CreateInstance<QuestionSO>();
            questionSO.SetData(questionData.question, questionData.answers, questionData.correctAnswerIndex);

            LoadedQuestions.Add(questionSO);
        }
    }
}
