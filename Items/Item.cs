using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all items, contains basic item information
public abstract class Item
{    
    public bool targetPlayer;
    public string itemName;
    public float cost;
    public bool wholeBody;
    public int amount = 1;
    public abstract void Use(Combat target);
}
