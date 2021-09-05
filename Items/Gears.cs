using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gears : Consumable
{
    public Gears() : base(){
        amountIncreased = 0.5f;
        itemName = "Gears";
        cost = 0.1f;
        wholeBody = true;
    }

    //Increase the amount of time gauge the player has by 50%
    public override void Use(Combat playerCombat)
    {
        CombatManager.Instance.IncreaseTimeGauge(amountIncreased);
        amount -= 1;
    }
}
