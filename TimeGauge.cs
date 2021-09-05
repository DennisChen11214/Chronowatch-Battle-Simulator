using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeGauge : MonoBehaviour
{
    Slider timeSlider;
    float changeMultiplier;
    public bool timeFrozen;
    bool mindBuffed;
    bool mindDebuffed;

    private void Start() {
        timeSlider = GetComponent<Slider>();
        changeMultiplier = 0.1f;
    }

    private void Update() {
        //Increase/decrease the time gauge
        timeSlider.value += Time.unscaledDeltaTime * changeMultiplier;
        //Time goes back to normal if the time gauge is depleted
        if(timeSlider.value <= 0){
            timeSlider.value = 0;
            InRealTime();
            UIManager.Instance.ResetCombat();
        }
    }

    public void DecreaseTime(float amnt){
        timeSlider.value -= amnt;
    }

    public void IncreaseTime(float amnt){
        timeSlider.value += amnt;
    }

    public float GetValue(){
        return timeSlider.value;
    }

    //Time stopped
    public void InTimeMode(){
        Time.timeScale = 0;
        if(mindBuffed){
            changeMultiplier = -0.0125f;
        }
        else if(mindDebuffed){
            changeMultiplier = -0.05f;
        }
        else{
            changeMultiplier = -0.025f;
        }
        timeFrozen = true;
    }

    //Time resumes
    public void InRealTime(){
        Time.timeScale = 1;
        if(mindBuffed){
            changeMultiplier = 0.2f;
        }
        else if(mindDebuffed){
            changeMultiplier = 0.05f;
        }
        else{
            changeMultiplier = 0.1f;
        }
        timeFrozen = false;
    }

    //Increases how fast the time gauge increases, decreases how fast it depletes
    public void ToggleMindBuff(bool buffed){
        mindBuffed = buffed;
    }
    //Increases how fast the time gauge decreases, decreases how fast it increases
    public void ToggleMindDebuff(bool debuffed){
        mindDebuffed = debuffed;
    }

    //Resumes time and resets gauge
    public void Reset(){
        timeFrozen = false;
        timeSlider.value = 0;
        mindBuffed = false;
        mindDebuffed = false;
    }
}
