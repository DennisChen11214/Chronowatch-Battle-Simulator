using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrone : Consumable
{
    public HealthDrone() : base(){
        amountIncreased = 15;
        itemName = "Health Drone";
        cost = 0.5f;
        wholeBody = true;
    }

    //Heals every body part for 15 health, can revive dead body parts
    public override void Use(Combat playerCombat)
    {
        playerCombat.Heal(amountIncreased, null, wholeBody, true);
        amount -= 1;
    }
}
