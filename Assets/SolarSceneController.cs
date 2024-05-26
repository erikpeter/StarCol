using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SolarSceneController : MonoBehaviour
{
    const double AU = 1.496e+11; // Astronimical unit;
    public double TimeSinceStart { get; set; }
    public double TimeMultiplier { get; set; }

    public float APerS { get; set; }

    public UnityOrbit current_for_state;
    public TMP_Text statevec_text;

    public double AUMultiplier { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        TimeMultiplier = 10.0*24.0*60.0*60.0;
        TimeSinceStart = 0.0;
        AUMultiplier = 10.0;
        APerS = 1000.0f;
    }

    public double GetMultiplier()
    {
        return (1.0/AU) * AUMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
       if (current_for_state != null)
        {
            statevec_text.text = current_for_state.GetStateString();
        }
       else
        {
            statevec_text.text = "";
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Debug.Log("PLUS (Keypad)");
            if (current_for_state != null)
            {
                current_for_state.Accelerate(Time.deltaTime, APerS);
            }
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Debug.Log("Minus (Keypad)");
            if (current_for_state != null)
            {
                current_for_state.Deaccelerate(Time.deltaTime, APerS);
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W");
            if (current_for_state != null)
            {
                current_for_state.MoveNormal(Time.deltaTime, APerS);
                //current_for_state.Accelerate(Time.deltaTime, APerS);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("S");
            if (current_for_state != null)
            {
                current_for_state.MoveAntiNormal(Time.deltaTime, APerS);
                //current_for_state.Deaccelerate(Time.deltaTime, APerS);
            }
        }


    }

    private void LateUpdate()
    {
        TimeSinceStart += TimeMultiplier * Time.deltaTime;
    }
}
