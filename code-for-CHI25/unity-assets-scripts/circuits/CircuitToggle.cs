using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircuitToggle : MonoBehaviour
{
    public GameObject circuit;
    
    private ToggleActiveState togglefunction;

    void Start()
    {

        togglefunction = circuit.GetComponent(typeof(ToggleActiveState)) as ToggleActiveState;
        
    }

    // Trigger detection
    private void OnTriggerEnter(Collider other)
    {
        //Check if colliding with airspace
        if (other.tag == "AirportArea")
        {
            //Debug.Log("NAME: " + other.gameObject.name + "TAG" + other.gameObject.tag);   
            togglefunction.ToggleActive();
        }
        
        
    }

    // To handle objects staying within the trigger
    private void OnTriggerStay(Collider other)
    {
        //Check if colliding with airspace
        if (other.tag == "AirportArea")
        {
            
        }
    }

    // To handle when the object exits the trigger
    private void OnTriggerExit(Collider other)
    {
        //Check if colliding with airspace
        if (other.tag == "AirportArea")
        {
            //Debug.Log("EXTITED NAME: " + other.gameObject.name + "TAG" + other.gameObject.tag);
            togglefunction.ToggleActive();
        }

    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
