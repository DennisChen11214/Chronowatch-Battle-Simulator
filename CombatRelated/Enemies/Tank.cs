using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tanks have more health and defense, less speed, and the ability to protect its teammates
public class Tank : EnemyCombat
{
    void Start()
    {
        pStats = new Stats(250, 12, 25, 7, 5, 5);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Tank";
        BodyPart head = new BodyPart("Head", 50);
        BodyPart body = new BodyPart("Body", 60);
        BodyPart rLeg = new BodyPart("Right Leg", 30);
        BodyPart lLeg = new BodyPart("Left Leg", 30);
        BodyPart rArm = new BodyPart("Right Arm", 40);
        BodyPart lArm = new BodyPart("Left Arm", 40);
        skillList = new List<Skill>();
        skillList.Add(SkillCollection.protect);
        bodyPartsHP.Add(head);
        bodyPartsHP.Add(body);
        bodyPartsHP.Add(rArm);
        bodyPartsHP.Add(rLeg);
        bodyPartsHP.Add(lArm);
        bodyPartsHP.Add(lLeg);
    }
}
