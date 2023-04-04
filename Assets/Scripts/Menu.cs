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

    [SerializeField] TextMeshProUGUI timeText;

    private DayCycle timeController;
    // Start is called before the first frame update
    void Awake()
    {
        clockRadialControl.OnValueChange += OnTimeChange;
        if (!timeController)
        {
            timeController = GameObject.FindObjectOfType<DayCycle>();
        }
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
}
