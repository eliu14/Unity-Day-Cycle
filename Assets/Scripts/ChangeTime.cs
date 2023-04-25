using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTime : MonoBehaviour
{
    [SerializeField]
    Menu menu;
    DayCycle dayCycle;
    // Start is called before the first frame update
    void Start()
    {
        dayCycle = GameObject.FindObjectOfType<DayCycle>();
    }

    public void sendNewTimetoDayCycle()
    {
        float newTime = menu.newTime;
        if (newTime >= 0)
        {
            // -1 would mean no newTime has been set
            dayCycle.UpdateTimeToGivenTime(menu.newTime);
            menu.newTime = -1f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
