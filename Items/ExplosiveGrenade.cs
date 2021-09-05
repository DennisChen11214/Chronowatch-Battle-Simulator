using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveGrenade : Grenade
{
    public ExplosiveGrenade() : base(){
        damage = 10;
        itemName = "Explosive Grenade";
        cost = 0.5f;
    }

    //Do 10 damage to every bodypart of an enemy
    public override void Use(Combat enemy){
        enemy.TakeDamage(null, damage, false, true, false);
        amount -= 1;
    }
}
