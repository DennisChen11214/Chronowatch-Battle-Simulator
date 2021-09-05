using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    public Stats pStats{get;protected set;}
    public int id;
    protected List<BodyPart> bodyPartsHP; // 0 is head, 1 is body, 2 is left arm, 3 is right arm, 4 is left leg, 5 is right leg
    protected List<Skill> skillList;
    //List of all the buffs on the character
    protected List<BuffInfo> buffList = new List<BuffInfo>();
    protected int armsBroken = 0;
    protected int legsBroken = 0;
    //How long the buffs last for
    float buffDuration = 10;

    public void SetBuffs(List<BuffInfo> buffs){
        buffList = buffs;
    }

    //Attack the target at the body part with a chance to do double damage if a crit occurs
    public void Attack(Combat target, BodyPart bodyPart){
        float damage = Mathf.RoundToInt((pStats.atk * pStats.atk) / (pStats.atk + target.pStats.def));
        float crit = Random.Range(0,100);
        bool isCrit = false;
        if(crit <= pStats.critChance){
            isCrit = true;
        }
        target.TakeDamage(bodyPart, damage, isCrit);
    }

    //All the body parts or the selected body part takes a certain amount of damage, chance to take no damage if dodge is successful
    public virtual bool TakeDamage(BodyPart bodyPart, float damage, bool crit, bool wholebody = false, bool dodgeable = true){
        //Damage reduced by damage reduction
        if(wholebody){
            bool hit = false;
            foreach(BodyPart bp in bodyPartsHP){
                if(TakeDamage(bp, damage, crit, false, dodgeable))
                    hit = true;
            }
            return hit;
        }
        if(bodyPart.body_Part == "Body"){
            damage = Mathf.RoundToInt(damage / pStats.damageReduction);
        }
        float dodge = Random.Range(0,100);
        float dodgeChance = pStats.dodgeChance;
        if(bodyPart.body_Part == "Head"){
            dodgeChance = pStats.dodgeChance + (80 - pStats.dodgeChance) / 4 * (4 - armsBroken - legsBroken);
        }
        if(dodgeable){
            if(dodge <= dodgeChance){
                CombatManager.Instance.SetHit(id, "Miss");
                return false;
            }
        }
        if(crit){
            damage *= 2;
            CombatManager.Instance.SetHit(id, "Crit");
        }
        else{
            CombatManager.Instance.SetHit(id, "Hit");
        }
        print("Took " + damage + " damage on " + bodyPart.body_Part);
        for(int i = 0; i < bodyPartsHP.Count; i++){
            if(bodyPartsHP[i].Equals(bodyPart)){
                //Make sure health doesn't go negative
                if(bodyPartsHP[i].bPartHealth - damage <= 0){
                    pStats.health -= bodyPartsHP[i].bPartHealth;
                    bodyPartsHP[i].bPartHealth = 0;
                    bodyPartsHP[i].broken = true;
                }
                else{
                    bodyPartsHP[i].bPartHealth -= damage;
                    pStats.health -= damage;
                }
            }
        }
        CheckBodyParts();
        return true;
    }

    //Checks if any body parts are broken and decreases stats accordingly
    protected virtual void CheckBodyParts(){
        armsBroken = 0;
        legsBroken = 0;
        foreach(BodyPart bp in bodyPartsHP){
            if(bp.bPartHealth <= 0){
                switch (bp.body_Part)
                {
                    case "Head":
                    case "Body":
                        break;
                    case "Left Arm":
                    case "Right Arm":
                        if(bp.broken){
                            armsBroken++;
                            break;
                        }
                        if(armsBroken == 1){
                            pStats.atk = 0.3f * pStats.maxAtk;
                            print("Broke arm atk: " + pStats.atk);
                        }
                        else{
                            pStats.atk = 0.7f * pStats.maxAtk;
                        }
                        break;
                    case "Left Leg":
                    case "Right Leg":
                        if(bp.broken){
                            legsBroken++;
                            break;
                        }
                        if(legsBroken == 1){
                            pStats.spd = 0.3f * pStats.maxSpd;
                        }
                        else{
                            pStats.spd = 0.7f * pStats.maxSpd;
                        }
                        break;
                }
            }
        }
        //Damage reduction decreased based on the amount of body parts broken
        if(armsBroken + legsBroken == 0){
            pStats.damageReduction = 1.5f;
        }
        else if(armsBroken + legsBroken == 1){
            pStats.damageReduction = 1.1f;
        }
        else if(armsBroken + legsBroken == 2){
            pStats.damageReduction = 0.8f;
        }
        else{
            pStats.damageReduction = 0.5f;
        }
    }

    //Heals all body parts or a specific one
    public virtual void Heal(float heal, BodyPart bodyPart, bool wholebody = false, bool canReviveDeadParts = false){
        if(wholebody){
            for(int i = 0; i < bodyPartsHP.Count; i++){
                BodyPart bp = bodyPartsHP[i];
                //Can't heal broken body parts normally
                if(bp.bPartHealth == 0 && !canReviveDeadParts){
                    continue;
                }
                if(bp.bPartHealth + heal > bp.maxHealth){
                    pStats.health += bp.maxHealth - bp.bPartHealth;
                    bp.bPartHealth = bp.maxHealth;
                }
                else{
                    pStats.health += heal;
                    bp.bPartHealth += heal;
                }
            }
        }
        else{
            for(int i = 0; i < bodyPartsHP.Count; i++){
                BodyPart bp = bodyPartsHP[i];
                if(bodyPart.Equals(bp)){
                    if(bp.bPartHealth == 0 && !canReviveDeadParts){
                        continue;
                    }
                    //Doesn't heal above a body part's max health
                    if(bp.bPartHealth + heal > bp.maxHealth){
                        pStats.health += bp.maxHealth - bp.bPartHealth;
                        bp.bPartHealth = bp.maxHealth;
                    }
                    else{
                        pStats.health += heal;
                        bp.bPartHealth += heal;
                    }
                }
            }
        }
    }

    //Acts based on what type of debuff the skill is and what it's targeting
    public void Debuff(Skill debuff, BodyPart bodyPart, bool wholeBody = false){
        for(int i = 0; i < bodyPartsHP.Count; i++){
            if((wholeBody || bodyPart.Equals(bodyPartsHP[i])) && bodyPartsHP[i].bPartHealth > 0){
                switch (debuff.debuffType)
                {
                    //Stat type debuff
                    case Skill.DebuffType.DECELERATION:
                        switch (bodyPart.body_Part)
                        {
                            case "Head":
                                if(pStats.headDebuffed) { StopCoroutine("DebuffHead");}
                                StartCoroutine("DebuffHead");
                                break;
                            case "Body":
                                if(pStats.defDebuffed) { StopCoroutine("DebuffBody");}
                                StartCoroutine("DebuffBody");
                                break;
                            case "Right Arm":
                            case "Left Arm":
                                if(pStats.atkDebuffed) { StopCoroutine("DebuffArm");}
                                StartCoroutine("DebuffArm");
                                break;
                            case "Right Leg":
                            case "Left Leg":
                                if(pStats.spdDebuffed) { StopCoroutine("DebuffLeg");}
                                StartCoroutine("DebuffLeg");
                                break;
                        }
                        break;
                    //Bleeding debuff
                    case Skill.DebuffType.BLEED:
                        StartCoroutine(Bleed(bodyPartsHP[i]));
                        break;
                }
            }
        }
    }

    protected virtual IEnumerator DebuffHead(){
        yield break;
    }
    
    //Decreases defense by half for a certain amount of time
    protected virtual IEnumerator DebuffBody(){
        if(!pStats.defDebuffed){
            float originalDef = pStats.def;
            pStats.def = pStats.def / 2.0f;
            pStats.defDebuffed = true;
            ToggleBuffUI(false, "Def", buffDuration, originalDef);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.def = pStats.def * 2.0f;
        pStats.defDebuffed = false;
        ToggleBuffUI(false, "Def", -buffDuration, 0);
    }

    //Decreases strength and crit change by half for a certain amount of time
    protected virtual IEnumerator DebuffArm(){
        if(!pStats.atkDebuffed) {
            float originalAtk = pStats.atk; 
            pStats.atk = pStats.atk / 2.0f;
            pStats.critChance = pStats.critChance / 2.0f;
            pStats.atkDebuffed = true;
            ToggleBuffUI(false, "Atk", buffDuration, originalAtk);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.atk = pStats.atk * 2.0f;
        pStats.critChance = pStats.critChance * 2.0f;
        pStats.atkDebuffed = false;
        ToggleBuffUI(false, "Atk", -buffDuration, 0);
    }

    //Decreases speed and dodge chance by half for a certain amount of time
    protected virtual IEnumerator DebuffLeg(){
        if(!pStats.spdDebuffed){
            float originalSpd = pStats.spd; 
            pStats.spd = pStats.spd / 2.0f;
            pStats.dodgeChance = pStats.dodgeChance / 2.0f;
            pStats.spdDebuffed = true;
            ToggleBuffUI(false, "Spd", buffDuration, originalSpd);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.spd = pStats.spd * 2.0f;
        pStats.dodgeChance = pStats.dodgeChance * 2.0f;
        pStats.spdDebuffed = false;
        ToggleBuffUI(false, "Spd", -buffDuration, 0);
    }

    //The body part takes 2 damage every 2 seconds for a total of 10 seconds
    protected IEnumerator Bleed(BodyPart bp){
        int timePassed = 0;
        ToggleBuffUI(false, "Bleed", 10, 0);
        while(timePassed < 10){
            if(bp.bPartHealth == 0){
                yield break;
            }
            yield return new WaitForSeconds(2);
            timePassed += 2;
            bp.bPartHealth -= 2;
            CheckBodyParts();
            CombatManager.Instance.UpdateEnemyParts();
        }
        ToggleBuffUI(false, "Bleed", -10, 0);
    }

    //Acts based on what type of buff the skill is and what it's targeting
    public void Buff(Skill buff, BodyPart bodyPart, bool wholeBody = false){
        for(int i = 0; i < bodyPartsHP.Count; i++){
            if((wholeBody || bodyPart.Equals(bodyPartsHP[i])) && bodyPartsHP[i].bPartHealth > 0){
                switch (buff.buffType)
                {
                    //Stat increases
                    case Skill.BuffType.ACCELERATION:
                        switch (bodyPart.body_Part)
                        {
                            case "Head":
                                if(pStats.headBuffed) { StopCoroutine("BuffHead");}
                                StartCoroutine("BuffHead");
                                break;
                            case "Body":
                                if(pStats.defBuffed) { StopCoroutine("BuffBody");}
                                StartCoroutine("BuffBody");
                                break;
                            case "Right Arm":
                            case "Left Arm":
                                if(pStats.atkBuffed) { StopCoroutine("BuffArm");}
                                StartCoroutine("BuffArm");
                                break;
                            case "Right Leg":
                            case "Left Leg":
                                if(pStats.spdBuffed) { StopCoroutine("BuffLeg");}
                                StartCoroutine("BuffLeg");
                                break;
                        }
                        break;
                    //Berserk buff for the claymore enemy
                    case Skill.BuffType.BERSERK:
                        if(pStats.berserk) {StopCoroutine("Berserk");}
                        StartCoroutine("Berserk");
                        break;
                    //Protect buff for the tank enemy
                    case Skill.BuffType.PROTECT:
                        if(pStats.protecting) {StopCoroutine("Protect");}
                        StartCoroutine("Protect");
                        break;
                    case Skill.BuffType.FUTURE:
                        break;

                }
            }
        }
    }

    protected virtual IEnumerator BuffHead(){
        yield break;
    }

    //Increases defense by 50% for a certain amount of time
    protected virtual IEnumerator BuffBody(){
        if(!pStats.defBuffed){
            float originalDef = pStats.def;
            pStats.def = pStats.def * 1.5f;
            pStats.defBuffed = true;
            ToggleBuffUI(true, "Def", buffDuration, originalDef);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.def = pStats.def * 2.0f / 3;
        pStats.defBuffed = false;
        ToggleBuffUI(true, "Def", -buffDuration, 0);
    }

    //Increases attack and crit by 50% for a certain amount of time
    protected virtual IEnumerator BuffArm(){
        if(!pStats.atkBuffed) {
            float originalAtk = pStats.atk; 
            pStats.atk = pStats.atk * 1.5f;
            pStats.critChance = pStats.critChance * 1.5f;
            pStats.atkBuffed = true;
            ToggleBuffUI(true, "Atk", buffDuration, originalAtk);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.atk = pStats.atk * 2.0f / 3;
        pStats.critChance = pStats.critChance * 2.0f / 3;
        pStats.atkBuffed = false;
        ToggleBuffUI(true, "Atk", -buffDuration, 0);
    }

    //Increases speed and dodge chance by 50% for a certain amount of time
    protected virtual IEnumerator BuffLeg(){
        if(!pStats.spdBuffed) {
            float originalSpd = pStats.spd;
            pStats.spd = pStats.spd * 1.5f;
            pStats.dodgeChance = pStats.dodgeChance * 1.5f;
            pStats.spdBuffed = true;
            ToggleBuffUI(true, "Spd", buffDuration, originalSpd);
        }
        yield return new WaitForSeconds(buffDuration);
        pStats.spd = pStats.spd * 2.0f / 3;
        pStats.dodgeChance = pStats.dodgeChance * 2.0f / 3;
        pStats.spdBuffed = false;
        ToggleBuffUI(true, "Spd", -buffDuration, 0);
    }

    //Turns on/off buff UI according to the type of buff and if it's an increase/decrease
    //Also works for if the same stat is buffed and debuffed at once
    protected void ToggleBuffUI(bool increase, string buff, float timeChange, float originalStat){
        for(int i = 0; i < buffList.Count; i++){ 
            if(buffList[i].buffText.text == buff){
                //How much time the buff has left
                buffList[i].timeRemaining += timeChange;
                //Buff ran out
                if(buffList[i].timeRemaining == 0){
                    buffList[i].buffObject.SetActive(false);
                    buffList[i].buffText.text = "";
                    return;
                }
                switch (buff)
                {
                    case "Atk":
                        //If we have less attack than we should
                        if(buffList[i].originalAtk > pStats.atk){
                            buffList[i].SetToDecrease();
                        }
                        //If we have more attack than we should
                        else if(buffList[i].originalAtk < pStats.atk){
                            buffList[i].SetToIncrease();
                        }
                        else{
                            //If out attack returned to normal
                            buffList[i].buffObject.SetActive(false);
                            buffList[i].buffText.text = "";
                        }
                        break;
                    case "Def":
                        if(buffList[i].originalDef > pStats.def){
                            buffList[i].SetToDecrease();
                        }
                        else if(buffList[i].originalDef < pStats.def){
                            buffList[i].SetToIncrease();
                        }
                        else{
                            buffList[i].buffObject.SetActive(false);
                            buffList[i].buffText.text = "";
                        }
                        break;
                    case "Spd":
                        if(buffList[i].originalSpd > pStats.spd){
                            buffList[i].SetToDecrease();
                        }
                        else if(buffList[i].originalSpd < pStats.spd){
                            buffList[i].SetToIncrease();
                        }
                        else{
                            buffList[i].buffObject.SetActive(false);
                            buffList[i].buffText.text = "";
                        }
                        break;
                }
                return;
            }
        }
        int buffIndex = 0;
        //Finds the first inactive buff in the UI
        for(int i = 0; i < buffList.Count; i++){    
            if(!buffList[i].buffObject.activeSelf){
                buffIndex = i;
                break;
            }
        }
        //Set what the attack was before the buff
        switch (buff)
        {
            case "Atk":
                buffList[buffIndex].originalAtk = originalStat;
                break;
            case "Def":
                buffList[buffIndex].originalDef = originalStat;
                break;
            case "Spd":
                buffList[buffIndex].originalSpd = originalStat;
                break;
        }
        buffList[buffIndex].timeRemaining += timeChange;
        buffList[buffIndex].SetBuff(buff);
        if(increase){
            buffList[buffIndex].SetToIncrease();
        }
        else{
            buffList[buffIndex].SetToDecrease();
        }
        buffList[buffIndex].buffObject.SetActive(true);
    }

    //Returns true if the head or body is destroyed, false otherwise
    public bool CheckDead(){
        foreach(BodyPart bp in bodyPartsHP){
            if((bp.body_Part == "Head" || bp.body_Part == "Body") && bp.bPartHealth == 0){
                return true;
            }
        }
        return false;
    }
}
