using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGrenade : Grenade
{
    public WebGrenade() : base(){
        damage = 6;
        itemName = "Web Grenade";
        cost = 0.5f;
    }

    //Deals 6 damage to all the body parts of an enemy and slows them
    public override void Use(Combat enemy){
        enemy.TakeDamage(null, damage, false, true, false);
        enemy.Debuff(SkillCollection.deceleration, new BodyPart("Right Leg"));
        amount -= 1;
    }
}
