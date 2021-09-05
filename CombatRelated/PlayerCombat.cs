using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : Combat
{
    //Event to update player body diagram when damage is taken/healed
    public delegate void OnDamageTaken(List<BodyPart> bodyParts);
    public static OnDamageTaken damageTaken;

    private void Start() {
        bodyPartsHP = new List<BodyPart>();
        skillList = new List<Skill>();
        pStats = new Stats(200, 20, 15, 10, 5, 5);
        BodyPart head = new BodyPart("Head", 40);
        BodyPart body = new BodyPart("Body", 40);
        BodyPart rLeg = new BodyPart("Right Leg", 30);
        BodyPart lLeg = new BodyPart("Left Leg", 30);
        BodyPart rArm = new BodyPart("Right Arm", 30);
        BodyPart lArm = new BodyPart("Left Arm", 30);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
        skillList.Add(SkillCollection.backstab);
        skillList.Add(SkillCollection.restoration);
        skillList.Add(SkillCollection.acceleration);
        skillList.Add(SkillCollection.deceleration);
        id = -1;
    }

    //Takes damage and if the player dies, go to the menu, returns whether the player was hit or dodged
    public override bool TakeDamage(BodyPart bodyPart, float damage, bool crit, bool wholebody = false, bool dodgeable = true){
        bool hit = base.TakeDamage(bodyPart, damage, crit, wholebody, dodgeable);
        damageTaken(bodyPartsHP);
        for(int i = 0; i < bodyPartsHP.Count; i++){
            if(bodyPartsHP[i].bPartHealth == 0 && (bodyPartsHP[i].body_Part == "Head" || bodyPartsHP[i].body_Part == "Body")){
                GameManager.Instance.GoToMenu();
            }
        }
        return hit;
    }

    //Return a copy of the player's list of skills
    public List<Skill> GetSkills(){
        List<Skill> skillListCopy = new List<Skill>();
        foreach(Skill skill in skillList){
            Skill skillCopy = new Skill(skill.type, skill.multiTarget, skill.timeCost, skill.multiplier, skill.name, 
                                        skill.wholeBody, skill.buffType, skill.debuffType, skill.regenHeal);
            skillListCopy.Add(skillCopy);
        }
        return skillListCopy;
    }

    //Return a list of the names of the player's body parts
    public List<string> GetBodyParts(){
        List<string> parts = new List<string>();
        foreach(BodyPart bp in bodyPartsHP){
            parts.Add(bp.body_Part);
        }
        return parts;
    }
    //Returns the health of a given body part
    public float GetBodyPartHealth(string bpName){
        foreach(BodyPart bp in bodyPartsHP){
            if(bp.body_Part.Equals(bpName)){
                return bp.bPartHealth;
            }
        }
        return 0;
    }

    //Buff the player's time gauge rates for 10 seconds
    protected override IEnumerator BuffHead(){
        if(!pStats.headDebuffed){
            CombatManager.Instance.ToggleMindBuff(true);
        }
        yield return new WaitForSeconds(10);
        CombatManager.Instance.ToggleMindBuff(false);
    }

    //Debuff the player's time gauge rates for 10 seconds
    protected override IEnumerator DebuffHead(){
        if(!pStats.headDebuffed){
            CombatManager.Instance.ToggleMindDebuff(true);
        }
        yield return new WaitForSeconds(10);
        CombatManager.Instance.ToggleMindDebuff(false);
    }

    //Heal and update the body diagram
    public override void Heal(float heal, BodyPart bodyPart, bool wholebody = false, bool canReviveDeadParts = false){
        base.Heal(heal, bodyPart, wholebody, canReviveDeadParts);
        damageTaken(bodyPartsHP);
    }

    //Resets all the stats and the health of the player, also stops all the buffs
    public void ResetHealth(){
        for(int i = 0; i < bodyPartsHP.Count; i++){
            bodyPartsHP[i].bPartHealth = bodyPartsHP[i].maxHealth;
        }
        StopAllCoroutines();
        pStats.atk = pStats.maxAtk;
        pStats.def = pStats.maxDef;
        pStats.spd = pStats.maxSpd;
        pStats.damageReduction = 1.5f;
        damageTaken(bodyPartsHP);
    }

}
