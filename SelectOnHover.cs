using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    //Selects a button when hovered over if it's interactable
    public void OnPointerEnter(PointerEventData eventData){
        if(!GetComponent<Button>().interactable){
            return;
        }
        else{
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
