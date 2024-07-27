using UnityEngine;
using UnityEngine.SceneManagement;

public class GameValues : MonoBehaviour
{
    public static GameValues Instance { get; private set; }

    public string dayTransitionSceneName = "Day transision";
    public int startingMoney = 200000;

    private float timer;
    private int money;
    private int mistakesCount = 0; // Track the number of mistakes
    private int correctDecisions = 0; // Track the number of correct decisions
    private int day = 1; // Track the current day
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
        ResetTimer();
        if (day == 1)
        {
            money = startingMoney;
        }
        else
        {
            money = PlayerPrefs.GetInt("TotalMoney", startingMoney);
        }

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
            Debug.Log("Timer: " + timer);
            if (timer <= 0)
            {
                Debug.Log("Timer ended, checking robot status...");
                CheckRobotStatus();
            }
        }
    }

    public void ResetTimer()
    {
        timer = 360f;
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
        StartCoroutine(SceneTransition.Instance.FadeAndLoadScene(dayTransitionSceneName));
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
    }

    public void AddMistake()
    {
        mistakesCount++;
    }

    public int GetMistakes()
    {
        return mistakesCount;
    }

    public void AddCorrectDecision()
    {
        correctDecisions++;
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
