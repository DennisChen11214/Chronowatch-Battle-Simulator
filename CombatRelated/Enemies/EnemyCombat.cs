using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : Combat
{
    public string enemyName{get;protected set;}
    public bool confused{get; private set;}
    public bool protecting;

    //Tell the combat manager that this enemy has died and set it inactive
    protected virtual void Die(){
        CombatManager.Instance.EnemyDied(this);
        print(enemyName + " died");
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    //Returns a list of the enemy's body parts, choice to include the broken parts
    public List<BodyPart> GetParts(bool includeDeadParts = false){
        List<BodyPart> parts = new List<BodyPart>();
        foreach(BodyPart bp in bodyPartsHP){
            if(bp.bPartHealth > 0 || includeDeadParts){
                BodyPart newBP = new BodyPart(bp.body_Part, bp.bPartHealth, bp.maxHealth);
                parts.Add(newBP);
            }
        }
        return parts;
    }

    //After taking damage, check if the head or body is broken, if so then die
    public override bool TakeDamage(BodyPart bodyPart, float damage, bool crit, bool wholebody = false, bool dodgeable = true){
        bool hit = base.TakeDamage(bodyPart, damage, crit, wholebody, dodgeable);
        for(int i = 0; i < bodyPartsHP.Count; i++){
            if(bodyPartsHP[i].bPartHealth == 0 && (bodyPartsHP[i].body_Part == "Head" || bodyPartsHP[i].body_Part == "Body")){
                Die();
            }
        }
        return hit;
    }

    //Returns the only skill that the enemy has
    public string GetSkill(){
        return skillList[0].name;
    }

    //Tank enemy's skill that makes it take the damage of any enemy
    protected IEnumerator Protect(){
        if(!pStats.protecting){
            protecting = true;
        }
        ToggleBuffUI(true, "Protect", 10, 0);
        yield return new WaitForSeconds(10);
        protecting = false;
        ToggleBuffUI(true, "Protect", -10, 0);
    }

    //Claymore enemy's skill that greatly increases atk and crit in exchange for defense and dodge chance
    protected IEnumerator Berserk(){
        if(!pStats.berserk){
            float originalAtk = pStats.atk;
            float originalDef = pStats.def;
            pStats.atk *= 2.5f;
            pStats.critChance *= 2.5f;
            pStats.def /= 2;
            pStats.dodgeChance = 0;
            pStats.atkBuffed = true;
            pStats.defDebuffed = true;
            ToggleBuffUI(true, "Atk", 15, originalAtk);
            ToggleBuffUI(false, "Def", 15, originalDef);
        }
        yield return new WaitForSeconds(15);
        pStats.atk /= 2.5f;
        pStats.critChance /= 2.5f;
        pStats.def *= 2;
        pStats.dodgeChance = 5;
        pStats.atkBuffed = false;
        pStats.defDebuffed = false;
        ToggleBuffUI(true, "Atk", -15, 0);
        ToggleBuffUI(false, "Def", -15, 0);
    }

    //Doubles the enemy's dodge chance for 10 seconds
    protected override IEnumerator BuffHead(){
        if(!pStats.headDebuffed){
            pStats.dodgeChance *= 2;
        }
        yield return new WaitForSeconds(10);
        pStats.dodgeChance /= 2;
    }

    //Stuns the enemy for 10 seconds
    protected override IEnumerator DebuffHead(){
        if(!pStats.headDebuffed){
            confused = true;
        }
        yield return new WaitForSeconds(10);
        confused = false;
    }

    //Enemy heals and updates its diagram
    public override void Heal(float heal, BodyPart bodyPart, bool wholebody = false, bool canReviveDeadParts = false){
        base.Heal(heal, bodyPart, wholebody, canReviveDeadParts);
        CombatManager.Instance.UpdateEnemyParts();
    }
}
