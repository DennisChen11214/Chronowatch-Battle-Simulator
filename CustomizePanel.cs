using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomizePanel : MonoBehaviour
{
    [SerializeField] GameObject numEnemies;
    [SerializeField] GameObject enemy1Type;
    [SerializeField] GameObject enemy2Type;
    [SerializeField] GameObject enemy3Type;
    [SerializeField] GameObject randomize;
    [SerializeField] GameObject save;
    [SerializeField] GameObject cancel;

    //When the number of enemies is changed in the dropdown, turn on/off enemy dropdowns accordingly
    public void SetEnemyOnOff(){
        if(GetNumEnemies() == 1){
            enemy2Type.transform.parent.gameObject.SetActive(false);
            enemy3Type.transform.parent.gameObject.SetActive(false);
        }
        else if(GetNumEnemies() == 2){
            enemy3Type.transform.parent.gameObject.SetActive(false);
            enemy2Type.transform.parent.gameObject.SetActive(true);
        }
        else{
            enemy2Type.transform.parent.gameObject.SetActive(true);
            enemy3Type.transform.parent.gameObject.SetActive(true);
        }
    }

    //Gets the number of enemies from the dropdown
    public int GetNumEnemies(){
        Dropdown dropdown = numEnemies.GetComponent<Dropdown>();
        return Int32.Parse(dropdown.options[dropdown.value].text);
    }

    //Gets the type of the first enemy from the dropdown
    public string GetEnemy1Type(){
        Dropdown dropdown = enemy1Type.GetComponent<Dropdown>();
        return dropdown.options[dropdown.value].text;
    }

    //Gets the type of the second enemy from the dropdown
    public string GetEnemy2Type(){
        Dropdown dropdown = enemy2Type.GetComponent<Dropdown>();
        return dropdown.options[dropdown.value].text;
    }

    //Gets the type of the third enemy from the dropdown
    public string GetEnemy3Type(){
        Dropdown dropdown = enemy3Type.GetComponent<Dropdown>();
        return dropdown.options[dropdown.value].text;
    }

    //Randomizes the types of enemies
    public void Randomize(){
        int numTypes = enemy1Type.GetComponent<Dropdown>().options.Count;
        enemy1Type.GetComponent<Dropdown>().value = UnityEngine.Random.Range(0, numTypes);
        enemy2Type.GetComponent<Dropdown>().value = UnityEngine.Random.Range(0, numTypes);
        enemy3Type.GetComponent<Dropdown>().value = UnityEngine.Random.Range(0, numTypes);
    }

    //Start a new fight with the chosen amount of enemies and enemy types when the save button is clicked
    public void StartNewFight(){
        CombatManager.Instance.StartNewFight(GetNumEnemies(), GetEnemy1Type(), GetEnemy2Type(), GetEnemy3Type());
    }

    //Return back to combat when the cancel button is clicked
    public void ResumeFight(){
        cancel.SetActive(false);
        CombatManager.Instance.ResumeFight();
    }

    //Resets the player's health
    public void ResetPlayerHealth(){
        CombatManager.Instance.ResetPlayerHealth();
    }

    //Resets the player's inventory
    public void ResetInventory(){
        Inventory.inventory.Clear();
        Inventory.InitializeInventory();
    }
}
