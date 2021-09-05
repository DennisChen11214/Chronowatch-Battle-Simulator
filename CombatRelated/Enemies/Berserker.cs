using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Berserkers have more attack, but less speed. Can berserk to gain high offense stats in exchange for defense
public class Berserker : EnemyCombat
{
    // Start is called before the first frame update
    void Start()
    {
        pStats = new Stats(200, 30, 15, 7, 0, 10);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Berserker";
        BodyPart head = new BodyPart("Head", 40);
        BodyPart body = new BodyPart("Body", 40);
        BodyPart rLeg = new BodyPart("Right Leg", 40);
        BodyPart lLeg = new BodyPart("Left Leg", 40);
        BodyPart rArm = new BodyPart("Right Arm", 20);
        BodyPart lArm = new BodyPart("Left Arm", 20);
        skillList = new List<Skill>();
        skillList.Add(SkillCollection.berserk);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
    }
}
