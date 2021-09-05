using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents an individual body part of the player/enemy with a certain amount of health
public class BodyPart
{
    public string body_Part;
    public float bPartHealth;
    public float maxHealth;
    public bool broken = false;

    public BodyPart(string bPart, float health = 0, float max = 0){
        body_Part = bPart;
        bPartHealth = health;
        if(max == 0)
            maxHealth = health;
        else
            maxHealth = max;
    }

    // 2 body parts are equal if they're the same part, health doesn't matter
    public override bool Equals(object obj)
    {
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        return body_Part == ((BodyPart)obj).body_Part;
    }
    
    public override int GetHashCode()
    {
        return body_Part.GetHashCode();
    }
}
