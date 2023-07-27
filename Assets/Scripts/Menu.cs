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
    private GameObject clockMainHand;
    [SerializeField]
    private GameObject clockShortHand;

    [SerializeField] 
    TextMeshProUGUI timeText;

    [SerializeField]
    TextMeshProUGUI rotationText;

    [SerializeField]
    TextMeshProUGUI newTimeText;

    [SerializeField]
    DragRotate clockHandRotate;

    [SerializeField]
    private MenuPause menuPause;

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
        menuPause.OnMenuToggled += SetClockHandRotation;
        menuPause.OnMenuToggled += SetRadialFill;
    }

    public void SetClockHandRotation(bool appPaused)
    {
        TimeSpan currentTimeAsTimeSpan = TimeSpan.Parse(timeText.text);
        float currentTimeAsFloat = (float)currentTimeAsTimeSpan.TotalHours;
        currentTimeAsFloat -= 12f;
        Debug.Log($"Current Time As Float: {currentTimeAsFloat}");
        float newAngle = (currentTimeAsFloat / 24) * 360f;
        Debug.Log($"New Angle: {newAngle}");
        //TODO: use new angle to adjust rotation of the clock hands
        clockHandRotate.rotateAttachedObjectByAngle(newAngle);
        clockRadialControl.rotateByAngle(newAngle);
        clockShortHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -(newAngle - 90)));
    }

    public void SetRadialFill(bool appPaused)
    {
        if (appPaused)
        {
            clockRadialControl.SetRatio(0);
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
