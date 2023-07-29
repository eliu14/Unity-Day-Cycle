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
        menu.setNewTime();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
