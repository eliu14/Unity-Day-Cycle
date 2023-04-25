using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class Menu : MonoBehaviour
{
    [SerializeField]
    private RadialFill clockRadialControl;

    [SerializeField] 
    TextMeshProUGUI timeText;

    [SerializeField]
    TextMeshProUGUI rotationText;

    [SerializeField]
    TextMeshProUGUI newTimeText;

    [SerializeField]
    DragRotate clockHandRotate;

    private DayCycle timeController;

    public float newTime = -1f; // -1 would mean no newTime has been set
    // Start is called before the first frame update
    void Awake()
    {
        //clockRadialControl.OnValueChange += OnTimeChange;
        clockHandRotate.OnAngleChanged += OnRotationChange;
        if (!timeController)
        {
            timeController = GameObject.FindObjectOfType<DayCycle>();
        }

    }

    private void OnRotationChange(float rotationAngle)
    {
        rotationText.text = rotationAngle.ToString("F2");

        newTime = 0f;
        if (rotationAngle <= 360)
        {
            newTime = (rotationAngle / 360f) * 24f;
        }
        else
        {
            newTime = (rotationAngle - 360f)/360f * 24f;
        }
        //Adjustment for clock orientation
        newTime += 12f;
        //Debug.Log($"New Time Float: {newTime}");
        TimeSpan newTimeSpan = TimeSpan.FromHours(newTime);
        DateTime newTimeDateTime = DateTime.Now.Date + newTimeSpan;
        newTimeText.text = newTimeDateTime.ToString("HH:mm");
        //Debug.Log($"New Time Text: {newTimeText.text}");
    }

    private void OnTimeChange(float ratio)
    {
        float newTime = ratio * 24;
        timeController.UpdateTimeToGivenTime(newTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        clockRadialControl.OnValueChange -= OnTimeChange;
        clockHandRotate.OnAngleChanged -= OnRotationChange;
    }
}
