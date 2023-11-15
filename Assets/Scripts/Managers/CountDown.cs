using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    public float countdownTime = 3f;

    private void Start()
    {
        // Start the countdown when the script is enabled
        StartCoroutine(StartGameCountdown());
    }

    private IEnumerator StartGameCountdown()
    {
        // Pause the game during the countdown
        Time.timeScale = 0f;

        float currentTime = countdownTime;

        while (currentTime > 0)
        {
            // Display the countdown on the TMP text
            countdownText.text = Mathf.Ceil(currentTime).ToString();

            // Decrease the countdown timer
            currentTime -= Time.unscaledDeltaTime;

            yield return null;
        }

        // Set the countdown text to indicate the game is starting
        countdownText.text = "GO!";

        // Wait for a short moment before starting the game
        yield return new WaitForSecondsRealtime(1f);

        // Resume the game
        Time.timeScale = 1f;

        // Disable the TMP text
        countdownText.gameObject.SetActive(false);
    }
}