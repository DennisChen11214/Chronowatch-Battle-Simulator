using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    //What the skill does
    public enum SkillType{
        ATTACK,
        BUFF,
        DEBUFF,
        HEALING
    }

    //What type of buff the skill applies
    public enum BuffType{
        ACCELERATION,
        FUTURE,
        BERSERK,
        PROTECT,
        NONE
    }

    //What type of debuff the skill applies
    public enum DebuffType{
        DECELERATION,
        BLEED,
        NONE

    }
    
    public SkillType type;
    public BuffType buffType;
    public DebuffType debuffType;
    //Properties of the skill
    public bool multiTarget;
    public bool wholeBody;
    public float timeCost;
    public float multiplier;
    public string name;
    public bool regenHeal;

    public Skill(SkillType sType, bool multi, float cost, float mult, string sName, bool wb, 
                 BuffType bType = BuffType.NONE, DebuffType dbType = DebuffType.NONE, bool regen = false){
        type = sType;
        multiTarget = multi;
        timeCost = cost;
        multiplier = mult;
        name = sName;
        wholeBody = wb;
        buffType = bType;
        debuffType = dbType;
        regenHeal = regen;
    }

}
