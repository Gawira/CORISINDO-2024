using UnityEngine;
using UnityEngine.SceneManagement;

public class GameValues : MonoBehaviour
{
    public static GameValues Instance { get; private set; }

    public string dayTransitionSceneName = "Day transision";
    public int startMoney = 200000;

    private float timer;
    private int money;
    private int mistakesCount = 0; // Track the number of mistakes
    private int correctDecisions = 0; // Track the number of correct decisions
    public int day = 0; // Track the current day
    private bool robotActive = false; // Flag to track if a robot is currently active
    private bool transitionPending = false; // Flag to track if a scene transition is pending

    public bool RobotActive { get { return robotActive; } }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        day = PlayerPrefs.GetInt("Days1"); // Default to Day 1 if not set, adjust to 0-based internally

        ResetTimer();
        if (day == 0)
        {
            money = startMoney;
        }
        else
        {
            money = PlayerPrefs.GetInt("TotalMoney", startMoney);
        }
        Debug.Log($"Initial Money (Day {day}): {money}");

        // Attempt to spawn a bot at the start
        BotTeleporter botTeleporter = FindObjectOfType<BotTeleporter>();
        if (botTeleporter != null && timer > 0)
        {
            botTeleporter.SpawnNewBot();
        }
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("Timer ended, checking robot status...");
                CheckRobotStatus();
            }
        }
    }

    public void ResetTimer()
    {
        timer = 10f;
    }

    private void CheckRobotStatus()
    {
        if (!robotActive)
        {
            Debug.Log("No robot active, transitioning scene...");
            ChangeScene();
        }
        else
        {
            transitionPending = true;
        }
    }

    public void RobotSpawned()
    {
        robotActive = true;
    }

    public void RobotDestroyed()
    {
        robotActive = false;
        Debug.Log("RobotDestroyed called");
        if (transitionPending)
        {
            ChangeScene();
        }
        else if (timer > 0)
        {
            BotTeleporter botTeleporter = FindObjectOfType<BotTeleporter>();
            if (botTeleporter != null)
            {
                botTeleporter.SpawnNewBot();
            }
        }
    }

    public void ChangeScene()
    {
        Debug.Log("Changing scene to: " + dayTransitionSceneName);
        PlayerPrefs.SetInt("TotalMoney", money); // Save the total money
        day++;
        Debug.Log($"Saving Total Money: {money} for Day: {day}");
        StartCoroutine(SceneTransition.Instance.FadeAndLoadScene(dayTransitionSceneName));
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log($"Adding Money: {amount}, New Balance: {money}");
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
        Debug.Log($"Subtracting Money: {amount}, New Balance: {money}");
    }

    public void AddMistake()
    {
        mistakesCount++;
        Debug.Log($"Adding Mistake, Total Mistakes: {mistakesCount}, Money After Penalty: {money}");
    }

    public int GetMistakes()
    {
        return mistakesCount;
    }

    public void AddCorrectDecision()
    {
        correctDecisions++;
        Debug.Log($"Adding Correct Decision, Total Correct Decisions: {correctDecisions}, Money After Addition: {money}");
    }

    public int GetCorrectDecisions()
    {
        return correctDecisions;
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    public int GetDay()
    {
        return day;
    }
}
