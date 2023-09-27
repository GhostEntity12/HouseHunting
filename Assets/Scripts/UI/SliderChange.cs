using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System;

public class SliderChange : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderText;

    // Update is called once per frame
    void Update()
    {
        sliderText.text = Math.Round(slider.value).ToString();
    }
}
