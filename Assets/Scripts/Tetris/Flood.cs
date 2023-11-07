using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flood : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public float speed = 2.0f;
    public float countdownDuration = 3.0f;
    public bool countdownEnabled = true; // Control countdown with a boolean

    private float journeyLength;
    private float startTime;
    private bool isMoving = false;

    private void Start()
    {
        if (startTransform != null && endTransform != null)
        {
            journeyLength = Vector3.Distance(startTransform.position, endTransform.position);
        }
        else
        {
            Debug.LogError("Please assign start and end transforms in the Inspector.");
        }

        if (countdownEnabled)
        {
            StartCoroutine(StartCountdown());
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startTransform.position, endTransform.position, fractionOfJourney);

            if (fractionOfJourney >= 1.0f)
            {
                isMoving = false;
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        float remainingTime = countdownDuration;
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            remainingTime -= 1.0f;
        }

        StartMoving();
    }

    public void StartMoving()
    {
        if (!isMoving)
        {
            isMoving = true;
            startTime = Time.time;
        }
    }
}