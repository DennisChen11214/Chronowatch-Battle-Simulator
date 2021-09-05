using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mechanics have more health, but average other stats. They can heal the party
public class Mechanic : EnemyCombat
{
    void Start()
    {
        pStats = new Stats(230, 15, 15, 10, 10, 10);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Mechanic";
        BodyPart head = new BodyPart("Head", 45);
        BodyPart body = new BodyPart("Body", 45);
        BodyPart rLeg = new BodyPart("Right Leg", 35);
        BodyPart lLeg = new BodyPart("Left Leg", 35);
        BodyPart rArm = new BodyPart("Right Arm", 35);
        BodyPart lArm = new BodyPart("Left Arm", 35);
        skillList = new List<Skill>();
        skillList.Add(SkillCollection.partyHeal);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
    }
}
