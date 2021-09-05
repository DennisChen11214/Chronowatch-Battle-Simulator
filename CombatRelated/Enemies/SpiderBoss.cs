using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBoss : EnemyCombat
{
    // Start is called before the first frame update
    void Start()
    {
        pStats = new Stats(200, 40, 30, 20, 5, 5);
        bodyPartsHP = new List<BodyPart>();
        enemyName = "Spider";
        BodyPart leg1 = new BodyPart("Leg 1", 20);
        BodyPart leg2 = new BodyPart("Leg 2", 20);
        BodyPart leg3 = new BodyPart("Leg 3", 20);
        BodyPart leg4 = new BodyPart("Leg 4", 20);
        BodyPart leg5 = new BodyPart("Leg 5", 20);
        BodyPart leg6 = new BodyPart("Leg 6", 20);
        BodyPart leg7 = new BodyPart("Leg 7", 20);
        BodyPart leg8 = new BodyPart("Leg 8", 20);
        bodyPartsHP.Add(leg1);
        bodyPartsHP.Add(leg2);
        bodyPartsHP.Add(leg3);
        bodyPartsHP.Add(leg4);
        bodyPartsHP.Add(leg5);
        bodyPartsHP.Add(leg6);
        bodyPartsHP.Add(leg7);
        bodyPartsHP.Add(leg8);
    }
}
