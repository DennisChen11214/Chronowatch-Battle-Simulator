using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Swordsmen are well-rounded, can use their skill to inflict bleed and deal single-target damage
public class Swordsman : EnemyCombat
{
    void Start()
    {
        pStats = new Stats(200, 20, 15, 10, 10, 10);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Swordsman";
        BodyPart head = new BodyPart("Head", 40);
        BodyPart body = new BodyPart("Body", 40);
        BodyPart rLeg = new BodyPart("Right Leg", 30);
        BodyPart lLeg = new BodyPart("Left Leg", 30);
        BodyPart rArm = new BodyPart("Right Arm", 30);
        BodyPart lArm = new BodyPart("Left Arm", 30);
        skillList = new List<Skill>();
        skillList.Add(SkillCollection.hitokiriSlash);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
    }
}
