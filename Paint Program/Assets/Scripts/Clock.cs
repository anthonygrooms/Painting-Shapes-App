using System;
using UnityEngine;
public class Clock : MonoBehaviour {

    const float
        degreesPerHour = 30f,
        degreesPerMinute = 6f,
        degreesPerSecond = 6f;

    public Transform
        hoursTransform,
        minutesTransform,
        secondsTransform;

    public bool
        continuous;
    private bool
        reversePeriod = false;

    public LightingManager lightingManager;

    void UpdateContinuous() {
        TimeSpan time = DateTime.Now.TimeOfDay;
        hoursTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalHours * degreesPerHour, 0f);
        minutesTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalMinutes * degreesPerMinute, 0f);
        secondsTransform.localRotation =
            Quaternion.Euler(0f, (float)time.TotalSeconds * degreesPerSecond, 0f);
        lightingManager.TimeofDay = ((float)time.TotalHours + (reversePeriod ? 12 : 0)) % 24;
    }

    void UpdateDiscrete()
    {
        DateTime time = DateTime.Now;
        hoursTransform.localRotation =
            Quaternion.Euler(0f, time.Hour * degreesPerHour, 0f);
        minutesTransform.localRotation =
            Quaternion.Euler(0f, time.Minute * degreesPerMinute, 0f);
        secondsTransform.localRotation =
            Quaternion.Euler(0f, time.Second * degreesPerSecond, 0f);
        lightingManager.UpdateLighting(time.Hour / 24);
    }

    void Update () {
        if (continuous) {
            UpdateContinuous();
        }
        else {
            UpdateDiscrete();
        }
    }

    //Destroy all primitives
    public void ChangeTimePeriod()
    {
        reversePeriod = !reversePeriod;
    }
}