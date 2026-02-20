using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Clase QuestionLoader
// Carga las preguntas del quiz desde un archivo JSON ubicado en StreamingAssets.
// Convierte cada entrada del JSON en una instancia de QuestionSO y las almacena en una lista accesible.
// Si el archivo no existe, está vacío o no es válido, registra un error y cierra la aplicación.
//
// Campos:
// - LoadedQuestions: lista pública de QuestionSO cargadas desde el archivo JSON
// - jsonFileName: nombre del archivo JSON dentro de la carpeta StreamingAssets
public class QuestionLoader : MonoBehaviour
{
    // Clase interna QuestionData
    // Representa la estructura de datos de una pregunta individual en el JSON.
    // Campos:
    // - question: texto de la pregunta
    // - answers: array de respuestas posibles
    // - correctAnswerIndex: índice de la respuesta correcta
    [System.Serializable]
    private class QuestionData
    {
        public string question;          // Texto de la pregunta
        public string[] answers;         // Array de respuestas posibles
        public int correctAnswerIndex;   // Índice de la respuesta correcta
    }

    // Clase interna QuestionList
    // Contenedor raíz del JSON que agrupa todas las preguntas.
    // Campos:
    // - questions: lista de objetos QuestionData deserializados desde el JSON
    [System.Serializable]
    private class QuestionList
    {
        public List<QuestionData> questions;  // Lista de preguntas deserializadas desde el JSON
    }

    public List<QuestionSO> LoadedQuestions { get; private set; }    // Lista de QuestionSO listos para usar

    [SerializeField] string jsonFileName = "questions.json";          // Nombre del archivo JSON en StreamingAssets

    // Método Awake
    // Inicia la carga del JSON antes de que comience el primer fotograma.
    void Awake()
    {
        LoadQuestionsFromJSON();
    }

    // Método LoadQuestionsFromJSON
    // Lee el archivo JSON, lo deserializa y crea instancias de QuestionSO para cada pregunta.
    // Registra errores y cierra la aplicación si el archivo no existe, está vacío o es inválido.
    void LoadQuestionsFromJSON()
    {
        // Construir la ruta al archivo JSON dentro de StreamingAssets
        string jsonFilePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file not found at: " + jsonFilePath);
            Application.Quit();
            return;
        }

        // Leer el contenido del archivo como texto
        string jsonData = File.ReadAllText(jsonFilePath);
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.LogError("JSON file is empty or not valid.");
            Application.Quit();
            return;
        }

        // Deserializar el JSON en la estructura QuestionList
        QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonData);
        if (questionList == null || questionList.questions.Count == 0)
        {
            Debug.LogError("Failed to parse JSON data or no questions found.");
            Application.Quit();
            return;
        }

        // Crear una instancia de QuestionSO por cada entrada del JSON
        LoadedQuestions = new List<QuestionSO>();
        foreach (var questionData in questionList.questions)
        {
            QuestionSO questionSO = ScriptableObject.CreateInstance<QuestionSO>();
            questionSO.SetData(questionData.question, questionData.answers, questionData.correctAnswerIndex);

            LoadedQuestions.Add(questionSO);
        }
    }
}
