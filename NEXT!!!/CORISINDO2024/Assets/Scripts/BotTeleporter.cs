using System.Collections.Generic;
using UnityEngine;

public class BotTeleporter : MonoBehaviour
{
    public Transform botSpawner; // The position where the bot will be teleported
    public RobotInitializer robotInitializer; // Reference to RobotInitializer

    void Start()
    {
        TeleportRandomBot();
    }

    void TeleportRandomBot()
    {
        // Combine both lists from RobotInitializer
        List<GameObject> allBots = new List<GameObject>();
        allBots.AddRange(robotInitializer.acceptedRobots);
        allBots.AddRange(robotInitializer.rejectedRobots);

        // Choose a random bot
        int randomIndex = Random.Range(0, allBots.Count);
        GameObject randomBot = allBots[randomIndex];

        // Teleport the selected bot
        TeleportBot(randomBot);
    }

    void TeleportBot(GameObject bot)
    {
        // Set the bot's position and rotation
        bot.transform.position = botSpawner.position;
        bot.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Optionally, log the action for debugging
        RobotController robotController = bot.GetComponent<RobotController>();
        if (robotController != null)
        {
            Debug.Log($"Teleported Bot ID: {robotController.GetID()} to position: {botSpawner.position}");
        }
    }
}
