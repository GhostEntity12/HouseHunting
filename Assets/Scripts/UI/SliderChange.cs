using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderChange : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderText;

    // Update is called once per frame
    void Update()
    {
        sliderText.text = slider.value.ToString();
    }
}
