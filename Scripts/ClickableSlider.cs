using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickableSlider : MonoBehaviour
{

    private Slider slider;

    public delegate void ToggleHandler();

    public event ToggleHandler Toggle;

    // Use this for initialization
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleSlider()
    {
        if (slider.value == slider.maxValue)
        {
            slider.value = slider.minValue;
        }
        else
        {
            slider.value = slider.maxValue;
        }

        if (Toggle != null)
        {
            Toggle();
        }
    }

}
