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
        dayText.text = "" + gameValues.GetDay();

        yield return new WaitForSeconds(1f);
        int savings = PlayerPrefs.GetInt("TotalMoney", gameValues.startingMoney);
        savingsText.text = "" + savings.ToString("N0");

        yield return new WaitForSeconds(1f);
        int salary = gameValues.GetCorrectDecisions() * 50000;
        salaryText.text = "" + salary.ToString("N0");

        yield return new WaitForSeconds(1f);
        int penalty = gameValues.GetMistakes() * 50000;
        mistakesText.text = "" + penalty.ToString("N0");

        yield return new WaitForSeconds(1f);
        int total = savings + salary - penalty;
        totalText.text = "" + total.ToString("N0");

        // Save the total for the next day
        PlayerPrefs.SetInt("TotalMoney", total);
    }
}
