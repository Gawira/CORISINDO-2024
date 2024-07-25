using UnityEngine;
using UnityEngine.SceneManagement;

public class GameValues : MonoBehaviour
{
    public float timerDuration = 360f; // 6 minutes in seconds
    public string dayTransitionSceneName = "Day Transition";
    public int startingMoney = 200000;

    private float timer;
    private int money;

    void Start()
    {
        timer = timerDuration;
        money = startingMoney;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeScene();
            }
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(dayTransitionSceneName);
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

    public float GetRemainingTime()
    {
        return timer;
    }
}
