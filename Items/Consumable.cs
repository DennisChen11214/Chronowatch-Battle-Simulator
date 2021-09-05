using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for consumables, enhances the player in some way
public abstract class Consumable : Item
{
    public float amountIncreased;
    public Consumable(){
        targetPlayer = true;
    }
}
