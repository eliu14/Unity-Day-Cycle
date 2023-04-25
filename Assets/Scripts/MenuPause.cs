using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuPause : MonoBehaviour
{
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
    }
}
