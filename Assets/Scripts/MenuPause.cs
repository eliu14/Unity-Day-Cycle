using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuPause : MonoBehaviour
{
    public event Action<bool> OnMenuToggled;
    static bool applicationPaused;
    [SerializeField]
    Button[] buttons;

    private void OnMouseUpAsButton()
    {
        
    }
    private void Awake()
    {
        applicationPaused = false;
        if (buttons != null)
        {
            foreach(Button b in buttons)
            {
                b.onClick.AddListener(TogglePause);
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("p"))
        {
            if (buttons != null)
            {
                if (applicationPaused)
                {
                    buttons[0].onClick.Invoke();
                }
                else
                {
                    buttons[1].onClick.Invoke();
                }
            }
        }
    }
    public void TogglePause()
    {
        Debug.Log("TogglePause: activated");
        if (applicationPaused)
        {
            Time.timeScale = 1;
            applicationPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            applicationPaused = true;
        }
        Debug.Log($"TogglePause: App Paused: {applicationPaused}");
        if (OnMenuToggled != null)
        {
            OnMenuToggled(applicationPaused);
        }
    }
}
