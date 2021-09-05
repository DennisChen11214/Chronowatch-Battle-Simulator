using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gunners have low health/def, but high attack, speed, crit, and dodge. Skill does high single-target damage
public class Gunner : EnemyCombat
{
    void Start()
    {
        pStats = new Stats(140, 20, 12, 15, 20, 25);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Gunner";
        BodyPart head = new BodyPart("Head", 30);
        BodyPart body = new BodyPart("Body", 30);
        BodyPart rLeg = new BodyPart("Right Leg", 24);
        BodyPart lLeg = new BodyPart("Left Leg", 24);
        BodyPart rArm = new BodyPart("Right Arm", 16);
        BodyPart lArm = new BodyPart("Left Arm", 16);
        skillList = new List<Skill>();
        skillList.Add(SkillCollection.hawkEyeShot);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
    }
}
