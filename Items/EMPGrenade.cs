using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPGrenade : Grenade
{
    public EMPGrenade() : base(){
        damage = 4;
        itemName = "EMP Grenade";
        cost = 0.5f;
    }

    //Deals 4 damage to all the body parts of an enemy and stuns them
    public override void Use(Combat enemy){
        enemy.TakeDamage(null, damage, false, true, false);
        enemy.Debuff(SkillCollection.deceleration, new BodyPart("Head"));
        amount -= 1;
    }
}
