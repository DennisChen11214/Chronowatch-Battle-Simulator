using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCapsule : Consumable
{
    public HealthCapsule() : base(){
        amountIncreased = 20;
        itemName = "Health Capsule";
        cost = 0.3f;
        wholeBody = false;
    }

    //Heals a single body part for 20 health, can revive dead body parts
    public override void Use(Combat playerCombat)
    {
        BodyPart bp = new BodyPart(CombatManager.Instance.currentBodyPart);
        playerCombat.Heal(amountIncreased, bp, false, true);
        amount -= 1;
    }
}
