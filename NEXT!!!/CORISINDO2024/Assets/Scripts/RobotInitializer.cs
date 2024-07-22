using System.Collections.Generic;
using UnityEngine;

public class RobotInitializer : MonoBehaviour
{
    public List<GameObject> acceptedRobots;
    public List<GameObject> rejectedRobots;

    void Start()
    {
        AssignIDs(acceptedRobots, "Accepted");
        AssignIDs(rejectedRobots, "Rejected");
    }

    private void AssignIDs(List<GameObject> robots, string category)
    {
        for (int i = 0; i < robots.Count; i++)
        {
            RobotController robotController = robots[i].GetComponent<RobotController>();
            if (robotController != null)
            {
                robotController.SetID(i + 1, category);
            }
        }
    }
}
