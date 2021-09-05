using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base of all grenades, used for attacking all enemies on all their body parts
public abstract class Grenade : Item
{
    public float damage;
    public Grenade(){
        targetPlayer = false;
        wholeBody = true;
    }
}
