using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains all the current stats, max stats, and if the character is buffed
public class Stats
{
    //Stats of the player/enemy
    public float health;
    public float atk;
    public float def;
    public float spd;
    public float maxHealth;
    public float maxAtk;
    public float maxDef;
    public float maxSpd;
    public float dodgeChance;
    public float critChance;
    public float damageReduction;
    //If the character is buffed/debuffed
    public bool atkBuffed;
    public bool defBuffed;
    public bool spdBuffed;
    public bool headBuffed;
    public bool atkDebuffed;
    public bool defDebuffed;
    public bool spdDebuffed;
    public bool headDebuffed;
    public bool berserk;
    public bool protecting;
    public Stats(float hp, float attack, float defense, float speed, float dodge, float crit){
        health = hp;
        atk = attack;
        def = defense;
        spd = speed;
        maxHealth = health;
        maxAtk = atk;
        maxDef = def;
        maxSpd = spd;
        dodgeChance = dodge;
        critChance = crit;
        damageReduction = 1.5f;
    }
}
