using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    GameObject currentSelected;
    int row = 1;
    int column = 0;

    private void Start() {
        //Calculate how many rows and columns are in the panel
        float initialY = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y < initialY){
                row++;
                initialY = transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y;
            }
        }
        column = Mathf.CeilToInt(transform.childCount / row);
    }

    private void Update() {
        if(EventSystem.current)
            currentSelected = EventSystem.current.currentSelectedGameObject;
        //Navigation is set for each button each frame to account for when buttons become uninteractable due to time gauge constraints
        for(int i = 0; i < transform.childCount; i++){
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            for(int l = i - 1; l >= 0; l--){
                if(transform.GetChild(l).GetComponent<Button>().interactable){
                    nav.selectOnLeft = transform.GetChild(l).GetComponent<Button>();
                    break;
                }
            }
            for(int r = i + 1; r < transform.childCount; r++){
                if(transform.GetChild(r).GetComponent<Button>().interactable){
                    nav.selectOnRight = transform.GetChild(r).GetComponent<Button>();
                    break;
                }
            }
            for(int u = i - column; u >= 0; u -= column){
                if(transform.GetChild(u).GetComponent<Button>().interactable){
                    nav.selectOnUp = transform.GetChild(u).GetComponent<Button>();
                    break;
                }
            }
            for(int d = i + column; d < transform.childCount; d += column){
                if(transform.GetChild(d).GetComponent<Button>().interactable){
                    nav.selectOnDown = transform.GetChild(d).GetComponent<Button>();
                    break;
                }
            }
            transform.GetChild(i).GetComponent<Button>().navigation = nav;
        }
    }

    //If the current selected button becomes uninteractable, this is called to select the next button that's interactable
    public void SelectNext(){
        if(!currentSelected){
            currentSelected = transform.GetChild(0).gameObject;
        }
        if(currentSelected.GetComponent<Button>().interactable){
            return;
        }
        //Loops back to the first button if the uninteractable button is the last one
        bool around = false; 
        for(int i = currentSelected.transform.GetSiblingIndex(); i < transform.childCount; i++){
            if(!around && i == transform.childCount - 1){
                i = 0;
                around = true;
            }
            if(transform.GetChild(i).GetComponent<Button>().interactable){
                EventSystem.current.SetSelectedGameObject(transform.GetChild(i).gameObject);
                currentSelected = EventSystem.current.currentSelectedGameObject;
                break;
            }
        }
    }

    //Used to select the first button that's interactable
    public void SelectFirst(){
        if(!currentSelected){
            currentSelected = transform.GetChild(0).gameObject;
        }
        if(currentSelected.GetComponent<Button>().interactable){
            return;
        }
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).GetComponent<Button>().interactable){
                EventSystem.current.SetSelectedGameObject(transform.GetChild(i).gameObject);
                currentSelected = EventSystem.current.currentSelectedGameObject;
                break;
            }
        }
    }

    //Set the current selected button on enable to the first interactable one
    private void OnEnable() {
        for(int i = 0; i < transform.childCount; i++){
            if(transform.GetChild(i).GetComponent<Button>().interactable){
                EventSystem.current.SetSelectedGameObject(transform.GetChild(i).gameObject);
                currentSelected = EventSystem.current.currentSelectedGameObject;
                return;
            }
        }
        currentSelected = transform.GetChild(0).gameObject;
    }
}
