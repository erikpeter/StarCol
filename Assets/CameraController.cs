using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CameraController : MonoBehaviour
{
    public Slider distance_slider;
    public Slider angle_slider;

    // Start is called before the first frame update
    void Start()
    {
        SetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        float heightAngle = angle_slider.value;
        float distance = distance_slider.value;
        float y = -1.0f*Mathf.Cos(Mathf.PI * heightAngle / 180.0f) * distance;
        float z = 1.0f*Mathf.Sin(Mathf.PI*heightAngle/180.0f) * distance;
        float x = 0.0f;
        transform.position = new(x, y, z);
        transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        //var fixview = transform.eulerAngles;
        //if (fixview.y == 180.0f) fixview.y = 0.0f;
        //transform.eulerAngles = fixview; 
    }
}
