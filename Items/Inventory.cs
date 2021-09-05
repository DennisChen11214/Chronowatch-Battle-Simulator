using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player's inventory, contains all the usable items they have
public static class Inventory
{
    public static List<Item> inventory = new List<Item>();

    //Adds 1 of each item for the player
    public static void InitializeInventory(){
        inventory.Add(new EMPGrenade());
        inventory.Add(new ExplosiveGrenade());
        inventory.Add(new WebGrenade());
        inventory.Add(new HealthCapsule());
        inventory.Add(new Gears());
        inventory.Add(new HealthDrone());
    }

    //Returns an item from the inventory given an item name
    public static Item GetItem(string itemName){
        for(int i = 0; i < inventory.Count; i++){
            if(inventory[i].itemName == itemName){
                return inventory[i];
            }
        }   
        return null;
    }

}
