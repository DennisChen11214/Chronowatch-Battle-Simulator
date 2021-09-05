using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Information about a buff/debuff including the original stat
public class BuffInfo
{
    public float originalAtk;
    public float originalDef;
    public float originalSpd;
    public float timeRemaining;
    public bool increase;
    //What kind of buff this is
    public Text buffText;
    //Increase or decrease arrow
    public Image buffImage;
    public GameObject buffObject;

    public BuffInfo(GameObject bObject){
        buffObject = bObject;
        buffText = buffObject.GetComponent<Text>();
        buffImage = buffObject.transform.GetChild(0).GetComponent<Image>();
    }

    //Buff UI image set to blue increase arrow
    public void SetToIncrease(){
        buffImage.sprite = Resources.Load<Sprite>("Increase");
        buffImage.color = Color.blue;
    }

    //Buff UI image set to red decrease arrow
    public void SetToDecrease(){
        buffImage.sprite = Resources.Load<Sprite>("Decrease");
        buffImage.color = Color.red;
    }

    public void SetBuff(string buff){
        buffText.text = buff;
    }
}
