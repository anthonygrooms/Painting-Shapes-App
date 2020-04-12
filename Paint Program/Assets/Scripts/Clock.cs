using System;
using UnityEngine;
using UnityEngine.UI;
public class Clock : MonoBehaviour {

    // Declare variables
    const float
        degreesPerHour = 30f,
        degreesPerMinute = 6f,
        degreesPerSecond = 6f;

    public Transform
        hoursTransform,
        minutesTransform,
        secondsTransform;
    private bool
        reversePeriod = false;

    public LightingManager lightingManager;
    public Text ampmLabel;

    // Update arms of clock
    void UpdateContinuous() {
        TimeSpan time = DateTime.Now.TimeOfDay;

        lightingManager.TimeofDay = ((float)time.TotalHours + (reversePeriod ? 12 : 0)) % 24;
        ampmLabel.text = Convert.ToInt16(lightingManager.TimeofDay) >= 12 ? "P.M." : "A.M.";
        
        hoursTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalHours * degreesPerHour, 0f);
        minutesTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalMinutes * degreesPerMinute, 0f);
        secondsTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalSeconds * degreesPerSecond, 0f);

    }

    void Update () {
        UpdateContinuous();
    }

    // Change the time period
    public void ChangeTimePeriod()
    {
        reversePeriod = !reversePeriod;
    }
}