using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayValues : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI mistakesText;
    public TextMeshProUGUI totalText;

    private GameValues gameValues;

    void Start()
    {
        gameValues = GameValues.Instance;

        // Initially, set all the texts to empty or placeholders
        dayText.text = "";
        savingsText.text = "";
        salaryText.text = "";
        mistakesText.text = "";
        totalText.text = "";

        StartCoroutine(DisplayValuesCoroutine());
    }

    private IEnumerator DisplayValuesCoroutine()
    {
        yield return new WaitForSeconds(1f);
        dayText.text = "" + (gameValues.GetDay());
        Debug.Log("Current Day: " + gameValues.GetDay());

        yield return new WaitForSeconds(1f);
        int savings;
        if (gameValues.GetDay() == 1)
        {
            savings = gameValues.startMoney;
        }
        else
        {
            savings = PlayerPrefs.GetInt("TotalMoney", gameValues.startMoney);
        }
        savingsText.text = "" + savings.ToString("N0");
        Debug.Log("Current Savings: " + savings);

        yield return new WaitForSeconds(1f);
        int salary = gameValues.GetCorrectDecisions() * 50000;
        salaryText.text = "" + salary.ToString("N0");
        Debug.Log("Current Salary: " + salary);

        yield return new WaitForSeconds(1f);
        int penalty = gameValues.GetMistakes() * 50000;
        mistakesText.text = "" + penalty.ToString("N0");
        Debug.Log("Current Penalty: " + penalty);

        yield return new WaitForSeconds(1f);
        int total = savings + salary - penalty;
        totalText.text = "" + total.ToString("N0");
        Debug.Log("Total Savings: " + total);

        // Save the total for the next day
        PlayerPrefs.SetInt("TotalMoney", total);
        Debug.Log($"Saved Total Money for Next Day: {total}");
    }
}
