using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorSelector : MonoBehaviour
{
    //Initial color of the button text
    public Color initialColor;

    private void Start() {
        initialColor = GetComponentInChildren<Text>().color;
    }

    // Change the color of the text to the color that the button is supposed to turn when selected
    void Update()
    {
        if(EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject){
            GetComponentInChildren<Text>().color = GetComponent<Button>().colors.selectedColor;
        }
        else if(EventSystem.current){
            GetComponentInChildren<Text>().color = initialColor;
        }
    }
}
