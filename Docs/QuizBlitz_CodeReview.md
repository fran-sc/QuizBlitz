#### 💻 Fragmentos de código relevantes

Este documento revisa con más detalle fragmentos de código clave del proyecto QuizBlitz.

---

##### 1. Carga Dinámica de Preguntas desde JSON ⭐⭐⭐

**Ubicación:** `QuestionLoader.cs` - método `LoadQuestionsFromJSON()`

**Descripción:**
Sistema que lee un archivo JSON ubicado en `StreamingAssets`, deserializa su contenido y genera instancias de `QuestionSO` en tiempo de ejecución, desacoplando completamente los datos del código.

**Código:**

```csharp
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

    string jsonData = File.ReadAllText(jsonFilePath);
    QuestionList questionList = JsonUtility.FromJson<QuestionList>(jsonData);

    // Crear una instancia de QuestionSO por cada entrada del JSON
    LoadedQuestions = new List<QuestionSO>();
    foreach (var questionData in questionList.questions)
    {
        QuestionSO questionSO = ScriptableObject.CreateInstance<QuestionSO>();
        questionSO.SetData(questionData.question, questionData.answers, questionData.correctAnswerIndex);
        LoadedQuestions.Add(questionSO);
    }
}
```

**Lo interesante:**

- **StreamingAssets como origen de datos:** Usar `Application.streamingAssetsPath` permite editar el archivo JSON sin recompilar el proyecto, facilitando la actualización del banco de preguntas sin tocar el código
- **Instanciación en tiempo de ejecución:** `ScriptableObject.CreateInstance<QuestionSO>()` crea objetos de datos sin necesidad de assets en disco, habilitando contenido completamente dinámico
- **Validación defensiva en cadena:** Comprueba existencia del archivo, contenido vacío y parseo correcto antes de continuar, cerrando la aplicación ante cualquier fallo crítico en lugar de operar con datos corruptos
- **Clases internas serializables:** `QuestionData` y `QuestionList` son clases anidadas privadas que modelan exactamente la estructura del JSON, manteniendo el contrato de datos encapsulado dentro del propio loader

**Impacto en gameplay:** Permite añadir, eliminar o modificar preguntas editando únicamente el archivo JSON, sin necesidad de abrir Unity ni recompilar, lo que agiliza enormemente el proceso de diseño de contenido.

---

##### 2. ScriptableObject como Modelo de Datos de Pregunta ⭐⭐⭐

**Ubicación:** `QuestionSO.cs` - clase completa

**Descripción:**
Diseño del modelo de datos de una pregunta como `ScriptableObject`, que combina edición visual en el Inspector de Unity con instanciación programática desde el cargador JSON.

**Código:**

```csharp
[CreateAssetMenu(fileName = "New QuestionSO", menuName = "Scriptable Objects/QuestionSO")]
public class QuestionSO : ScriptableObject
{
    [SerializeField][TextArea(3, 10)]
    string question = "Añade aquí el texto de la pregunta";
    public string Question => question;

    [SerializeField]
    string[] answers = new string[4];
    public string GetAnswer(int i) => answers[i];

    [SerializeField][Range(0, 3)]
    int correctAnswerIndex = 0;
    public int CorrectAnswerIndex => correctAnswerIndex;

    public void SetData(string question, string[] answers, int correctAnswerIndex)
    {
        this.question = question;
        this.answers = answers;
        this.correctAnswerIndex = correctAnswerIndex;
    }
}
```

**Lo interesante:**

- **Doble vía de creación:** El atributo `[CreateAssetMenu]` permite crear preguntas manualmente desde el Editor, mientras que `SetData()` habilita la creación programática desde el JSON loader, sin cambiar la interfaz de uso
- **Encapsulación con propiedades de solo lectura:** Los campos son `private` con propiedades públicas de lectura (`=> field`), protegiendo la integridad del dato una vez creado fuera del contexto de `SetData`
- **[Range(0,3)] como contrato de datos:** El atributo limita el índice de respuesta correcta a valores válidos en el Inspector, pero también documenta implícitamente que el array tiene exactamente 4 elementos
- **[TextArea] para usabilidad:** Aplicar `[TextArea(3, 10)]` al campo de pregunta hace que el Inspector muestre un área multilínea redimensionable, mejorando la experiencia de edición de contenido largo

**Impacto en gameplay:** Centraliza todos los datos de una pregunta en un único objeto cohesivo y reutilizable, que puede asignarse en el Inspector o generarse por código, sin modificar la lógica del quiz.

---

##### 3. Máquina de Estados del Temporizador con Fases Duales ⭐⭐⭐

**Ubicación:** `Timer.cs` - métodos `UpdateState()` y `StartTimer()`

**Descripción:**
Temporizador que implementa un ciclo de dos fases (respuesta y revisión) mediante una enumeración de estados, controlando tanto la cuenta atrás como los sprites de la UI según la fase activa.

**Código:**

```csharp
public enum TimerState { NotStarted, Answering, Reviewing, ReviewEnded }
TimerState timerState;
public TimerState State => timerState;

public void StartTimer()
{
    timerState = TimerState.Answering;
    timerImage.sprite = defaultTimerSprite;
    ResetTimer(answerTime);
}

void UpdateState()
{
    if (timeLeft == 0)
    {
        if (timerState == TimerState.Answering)
        {
            // Tiempo de respuesta agotado: iniciar fase de revisión
            timerState = TimerState.Reviewing;
            timerImage.sprite = reviewTimerSprite;
            ResetTimer(reviewTime);
        }
        else if (timerState == TimerState.Reviewing)
        {
            // Tiempo de revisión agotado: señalar fin del ciclo
            timerState = TimerState.ReviewEnded;
        }
    }
}
```

**Lo interesante:**

- **Estado como contrato público:** Exponer `State` como propiedad de solo lectura permite que `Quiz.cs` reaccione a las transiciones sin poder manipular el estado directamente, garantizando que solo el propio Timer controla su ciclo
- **Reutilización del temporizador:**  `ResetTimer(reviewTime)` recicla la misma lógica de cuenta atrás para la segunda fase, evitando duplicar código y asegurando comportamiento uniforme entre fases
- **Cambio visual sincronizado con el estado:** Cambiar `timerImage.sprite` en el mismo lugar donde cambia el estado garantiza que la UI siempre refleja la fase real, sin posibles desincronizaciones
- **CancelTimer como interrupción externa:** Poner `timeLeft = 0` desde fuera dispara de forma natural la transición de estado en `UpdateState()`, sin necesidad de métodos adicionales para cambiar el estado explícitamente

**Impacto en gameplay:** Separa limpiamente el tiempo de respuesta del jugador del tiempo de retroalimentación visual, creando una experiencia de quiz con ritmo claro: presión temporal seguida de momento de aprendizaje.

---

##### 4. Selección Aleatoria sin Repetición por Extracción ⭐⭐⭐

**Ubicación:** `Quiz.cs` - métodos `GetRandomQuestion()` y `GetNextQuestion()`

**Descripción:**
Algoritmo que garantiza que ninguna pregunta se repite durante la partida eliminándola de la lista activa tras seleccionarla, en lugar de marcarla como usada o barajar el conjunto completo.

**Código:**

```csharp
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
        // Sin preguntas restantes: fin de partida
        gameManager.GameOver();
    }
}

void GetRandomQuestion()
{
    // Obtener un índice aleatorio dentro de las preguntas disponibles
    int randomIndex = Random.Range(0, questions.Count);

    // Guardar la pregunta seleccionada y eliminarla de la lista pendiente
    question = questions[randomIndex];
    questions.RemoveAt(randomIndex);
}
```

**Lo interesante:**

- **Extracción en lugar de marcado:** Eliminar la pregunta de la lista es más eficiente y simple que mantener un conjunto de preguntas usadas; la lista vacía actúa automáticamente como condición de fin de partida
- **`List<T>` como cola aleatoria:** Usar una `List<QuestionSO>` dinámica permite que `Random.Range(0, questions.Count)` siempre opere sobre índices válidos conforme la lista se reduce, sin validaciones adicionales
- **Progreso y fin de partida acoplados a la lista:** La condición `questions.Count > 0` hace que el progreso y el Game Over dependan del mismo estado (la lista), simplificando la lógica y evitando contadores adicionales
- **Actualización inmediata de la barra:** Actualizar `progressBar.value` justo tras incrementar el contador garantiza que el jugador siempre ve el progreso real de forma sincronizada

**Impacto en gameplay:** El jugador experimenta todas las preguntas del banco exactamente una vez por partida, en orden impredecible, asegurando variedad y completitud sin posibilidad de repetición accidental.

---

##### 5. Evaluación de Respuesta con Feedback Inmediato ⭐⭐

**Ubicación:** `Quiz.cs` - método `OnAnswerSelected()`

**Descripción:**
Manejador de respuesta que en una sola llamada evalúa la selección del jugador, proporciona feedback textual, resalta la respuesta correcta, bloquea los botones y transfiere el control al temporizador.

**Código:**

```csharp
public void OnAnswerSelected(int index)
{
    if (index == question.CorrectAnswerIndex)
    {
        // Respuesta correcta: feedback positivo y registro del acierto
        questionText.text = "¡Correcto!";
        score.AddCorrectAnswer();
    }
    else
    {
        // Respuesta incorrecta: feedback negativo
        questionText.text = "Ohhh... ¡Incorrecto!";
    }

    // Mostrar la opción correcta, bloquear interacción e iniciar revisión
    ShowCorrectAnswer();
    SetButtonState(false);
    gotAnswered = true;
    timer.CancelTimer();
}
```

**Lo interesante:**

- **`timer.CancelTimer()` como transición de fase:** Forzar `timeLeft = 0` en el Timer desencadena la transición al estado `Reviewing` de forma natural, sin que `Quiz.cs` tenga que gestionar el estado del temporizador directamente
- **`gotAnswered` como guardia de estado:** El flag previene que el bloque de tiempo agotado en `Update()` sobreescriba el feedback de respuesta con "¡Tiempo finalizado!", garantizando que el mensaje correcto permanezca en pantalla hasta el final de la revisión
- **Feedback visual doble:** Mostrar texto en `questionText` y cambiar el sprite del botón correcto mediante `ShowCorrectAnswer()` ofrece retroalimentación simultánea en dos canales visuales distintos
- **Desacoplamiento mediante índice:** El botón de respuesta pasa su índice como parámetro entero, evitando que la UI tenga acceso directo a los datos de la pregunta y manteniendo la separación vista-lógica

**Impacto en gameplay:** El jugador recibe retroalimentación instantánea y clara tras cada respuesta, con tiempo suficiente para ver cuál era la opción correcta si se equivocó, favoreciendo el aprendizaje durante la partida.

---

##### 6. Gestión del Flujo de Partida mediante Canvas Switching ⭐⭐

**Ubicación:** `GameManager.cs` - métodos `Start()`, `GameOver()` y `RestartGame()`

**Descripción:**
Controlador central de flujo que gestiona las transiciones entre pantallas activando y desactivando Canvas completos, y ofrece a otros scripts una interfaz centralizada para iniciar el Game Over o reiniciar la partida.

**Código:**

```csharp
void Start()
{
    // Estado inicial: quiz activo, pantalla de fin de partida oculta
    quizCanvas.gameObject.SetActive(true);
    gameOverCanvas.gameObject.SetActive(false);
}

public void GameOver()
{
    // Ocultar el quiz y mostrar la pantalla de resultados
    quizCanvas.gameObject.SetActive(false);
    gameOverCanvas.gameObject.SetActive(true);
}

public void RestartGame()
{
    // Recargar la escena completa para reiniciar el estado del juego
    SceneManager.LoadScene(0);
}
```

**Lo interesante:**

- **Canvas completo como unidad de pantalla:** Activar y desactivar Canvas enteros (en lugar de elementos individuales) simplifica la gestión de la UI y garantiza que todos los elementos de cada pantalla cambian de visibilidad de forma atómica
- **Reinicio por recarga de escena:** Usar `SceneManager.LoadScene(0)` para reiniciar recrea desde cero todos los objetos y estados, eliminando la necesidad de métodos de reset en cada componente y asegurando un estado inicial limpio
- **Punto de entrada centralizado:** `GameManager` actúa como único responsable de las transiciones de pantalla; `Quiz.cs` llama a `gameManager.GameOver()` sin saber cómo se implementa el cambio, respetando el principio de responsabilidad única
- **Orden de activación en Start:** Establecer explícitamente ambos canvas en `Start()` asegura el estado inicial correcto independientemente de cómo estén configurados en el Editor

**Impacto en gameplay:** Transiciones de pantalla instantáneas y sin artefactos visuales, con un flujo de partida reiniciable que preserva la integridad del estado del juego en cada nueva sesión.

---

##### 7. Puntuación con Retroalimentación Personalizada en Game Over ⭐⭐

**Ubicación:** `GameOver.cs` - método `Start()` + `Score.cs` - clase completa

**Descripción:**
Sistema que combina un contador de resultados minimalista (`Score.cs`) con un generador de mensajes finales adaptativo, que selecciona el texto motivacional en función del rendimiento relativo del jugador.

**Código:**

```csharp
// Score.cs - contador de resultados
public class Score : MonoBehaviour
{
    int correctAnswers = 0;
    public int CorrectAnswers => correctAnswers;

    int questionsVisited = 0;
    public int QuestionsVisited => questionsVisited;

    public void AddCorrectAnswer()   { correctAnswers++; }
    public void AddQuestionVisited() { questionsVisited++; }
    public void ResetScore()         { correctAnswers = 0; questionsVisited = 0; }
}

// GameOver.cs - mensaje final adaptativo
void Start()
{
    msgText.text = "¡Juego terminado! \n"
        + "¡Has respondido " + score.CorrectAnswers + " de "
        + score.QuestionsVisited + " preguntas correctamente.\n";

    if (score.CorrectAnswers == score.QuestionsVisited)
        msgText.text += "¡Felicidades! Has respondido todas las preguntas correctamente.";
    else if (score.CorrectAnswers > score.QuestionsVisited / 2)
        msgText.text += "¡Buen trabajo! Has respondido más de la mitad de las preguntas correctamente.";
    else
        msgText.text += "¡Sigue practicando! Puedes hacerlo mejor la próxima vez.";
}
```

**Lo interesante:**

- **Umbral relativo en lugar de absoluto:** Usar `score.QuestionsVisited / 2` como referencia hace que el umbral de "buen trabajo" se adapte automáticamente al número total de preguntas del JSON, sin constantes a mantener
- **Score como componente independiente:** Separar el seguimiento de datos en `Score.cs` propio permite que tanto `Quiz.cs` (escritura) como `GameOver.cs` (lectura) accedan a los resultados sin acoplamiento directo entre ellos
- **Construcción de texto en Start:** Calcular el mensaje en el primer frame del canvas de Game Over garantiza que los datos de `Score` ya están actualizados cuando se construye el texto, evitando condiciones de carrera
- **Tres segmentos de mensaje:** El texto final está compuesto por una parte fija (resultado numérico) más una variable (valoración cualitativa), lo que permite modificar los mensajes motivacionales sin alterar la lógica de evaluación

**Impacto en gameplay:** El jugador recibe un cierre emocional claro al finalizar la partida, con reconocimiento diferenciado de su rendimiento que refuerza tanto el logro (acierto total) como la mejora progresiva (más de la mitad).
