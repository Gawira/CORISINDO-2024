using System.Collections.Generic;
using UnityEngine;

public class BotTeleporter : MonoBehaviour
{
    public Transform botSpawner; // The position where the bot will be teleported
    public RobotInitializer robotInitializer; // Reference to RobotInitializer

    private GameObject teleportedBot; // Reference to the teleported bot
    private List<GameObject> availableBots; // List of bots that can still be called

    void Start()
    {
        // Initialize the available bots list with all bots
        availableBots = new List<GameObject>();
        availableBots.AddRange(robotInitializer.acceptedRobots);
        availableBots.AddRange(robotInitializer.rejectedRobots);

        // Teleport a random bot at the start if the timer is still running
        if (GameValues.Instance.GetRemainingTime() > 0)
        {
            TeleportRandomBot();
        }
    }

    public void TeleportRandomBot()
    {
        if (availableBots.Count == 0)
        {
            Debug.LogError("No more available bots to teleport.");
            return;
        }

        // Choose a random bot from the available bots list
        int randomIndex = Random.Range(0, availableBots.Count);
        teleportedBot = availableBots[randomIndex];

        // Remove the selected bot from the available bots list
        availableBots.RemoveAt(randomIndex);

        // Teleport the selected bot
        TeleportBot(teleportedBot);
    }

    private void TeleportBot(GameObject bot)
    {
        // Set the bot's position and rotation
        bot.transform.position = botSpawner.position;
        bot.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Enable the RobotController script
        RobotController robotController = bot.GetComponent<RobotController>();
        if (robotController != null)
        {
            robotController.enabled = true;
            Debug.Log($"Teleported Bot ID: {robotController.GetID()} to position: {botSpawner.position}");
        }
        GameValues.Instance.RobotSpawned();

    }

    public GameObject GetTeleportedBot()
    {
        return teleportedBot;
    }

    public void SpawnNewBot()
    {
        if (GameValues.Instance.GetRemainingTime() > 0)
        {
            TeleportRandomBot();
        }
    }
}
