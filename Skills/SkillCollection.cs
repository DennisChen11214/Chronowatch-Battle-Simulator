using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillCollection
{
    //List of all the skills in the game
    public static Skill backstab = new Skill(Skill.SkillType.ATTACK, false, 0.4f, 1.5f ,"Backstab", false, Skill.BuffType.NONE, Skill.DebuffType.BLEED);
    public static Skill restoration = new Skill(Skill.SkillType.HEALING, false, 0.5f, 1.0f ,"Restoration", true);
    public static Skill acceleration = new Skill(Skill.SkillType.BUFF, false, 0.2f, 1.5f ,"Acceleration", false, Skill.BuffType.ACCELERATION);
    public static Skill deceleration = new Skill(Skill.SkillType.DEBUFF, false, 0.1f, 1.5f ,"Deceleration", false, Skill.BuffType.NONE, Skill.DebuffType.DECELERATION);
    public static Skill swordWave = new Skill(Skill.SkillType.ATTACK, true, 0.4f, 1.0f ,"Sword Wave", false);
    public static Skill hawkEyeShot = new Skill(Skill.SkillType.ATTACK, false, 0.4f, 2.0f ,"Hawkeye Shot", false);
    public static Skill protect = new Skill(Skill.SkillType.BUFF, true, 0.4f, 1.0f ,"Protect", false, Skill.BuffType.PROTECT);
    public static Skill hitokiriSlash = new Skill(Skill.SkillType.ATTACK, false, 0.4f, 1.5f ,"Hitokiri Slash", false, Skill.BuffType.NONE, Skill.DebuffType.BLEED);
    public static Skill berserk = new Skill(Skill.SkillType.BUFF, false, 0.4f, 1.5f ,"Berserk", false, Skill.BuffType.BERSERK);
    public static Skill partyHeal = new Skill(Skill.SkillType.HEALING, true, 0.4f, 1.0f ,"Party Heal", true);
    public static List<Skill> skillList = new List<Skill>{backstab, restoration, acceleration, deceleration, swordWave, 
                                                             hawkEyeShot, protect, hitokiriSlash, berserk, partyHeal};

    //Returns a skill given the name
    public static Skill ReturnSkill(string skillName){
        foreach(Skill skill in skillList){
            if(skill.name == skillName){
                return skill;
            }
        }
        return null;
    }

}
