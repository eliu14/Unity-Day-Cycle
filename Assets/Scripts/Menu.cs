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
    private RadialFill clockColor;

    [SerializeField] TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        clockColor.OnValueChange += OnTimeChange;
    }

    private void OnTimeChange(float ratio)
    {

        //timeText.text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
