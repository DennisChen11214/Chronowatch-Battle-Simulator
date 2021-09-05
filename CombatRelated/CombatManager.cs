using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : Singleton<CombatManager>
{
    //Holds information about the player's current move
    public int currentEnemyIndex{get; private set;} = -1;
    public string currentBodyPart{get; private set;}
    public string currentSkill{get; private set;}
    public string currentAction{get; private set;}
    public string currentItem{get; private set;}
    public float attackCost{get; private set;} = 0.25f;
    public float runAwayCost{get; private set;} = 0.2f;
    public int enemyCount;
    [SerializeField] List<GameObject> enemyTypes;
    [SerializeField] List<Transform> spawnPositions;
    PlayerCombat player;
    List<EnemyCombat> enemies;
    TimeGauge timeGauge;
    //Used to determine when each enemy can move
    List<float> timeTicks;
    CombatPanel cp;
    List<string> randomParts = new List<string>{"Body","Head","Right Arm", "Left Arm", "Right Leg", "Left Leg"};
    List<int> deadEnemyIndices;
    bool namesInitialized;
    bool firstFight = true;

    protected override void Awake() {
        base.Awake();
    }
    
    private void Start() {
        deadEnemyIndices = new List<int>();
        enemies = new List<EnemyCombat>();
        timeTicks = new List<float>();
    }

    //Starts up a new fight by reinitializing everything and creating numEnemies new enemies
    public void StartNewFight(int numEnemies, string enemy1, string enemy2, string enemy3){
        Time.timeScale = 1;
        enemies.Clear();
        timeTicks.Clear();
        deadEnemyIndices.Clear();
        namesInitialized = false;
        GameManager.Instance.EnterCombat();
        cp.ResetUI();
        //Only need to get reference to time gauge and player before the first fight
        if(firstFight){
            timeGauge = cp.GetTimeGauge();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
            firstFight = false;
            player.SetBuffs(cp.PlayerBuffObject());
        }
        else{
            timeGauge.Reset();
            cp.ChangeEnemySkillText("");
        }
        //Create the new enemies based on the given types
        enemyCount = numEnemies;
        for(int i = 0; i < numEnemies; i++){
            if(i == 0){
                EnemyCombat enemy = CreateAndReturnEnemy(enemy1, 0);
                enemies.Add(enemy);
                enemy.SetBuffs(cp.BuffObjects(0));
                enemy.id = 0;
            }
            if(i == 1){
                EnemyCombat enemy = CreateAndReturnEnemy(enemy2, 1);
                enemies.Add(enemy);
                enemy.SetBuffs(cp.BuffObjects(1));
                enemy.id = 1;
            }
            if(i == 2){
                EnemyCombat enemy = CreateAndReturnEnemy(enemy3, 2);
                enemies.Add(enemy);
                enemy.SetBuffs(cp.BuffObjects(2));
                enemy.id = 2;
            }
            timeTicks.Add(100);
        }
    }

    //Changes the gamestate back to COMBAT
    public void ResumeFight(){
        GameManager.Instance.EnterCombat();
    }

    //Creates new enemies and adds them to the scene given the enemyType and what number enemy it is
    EnemyCombat CreateAndReturnEnemy(string enemyType, int index){
        int typeIndex = 0;
        switch (enemyType)
        {
            case "Berserker":
                typeIndex = 0;
                break;
            case "Gunner":
                typeIndex = 1;
                break;
            case "Mechanic":
                typeIndex = 2;
                break;
            case "Swordsman":
                typeIndex = 3;
                break;
            case "Tank":
                typeIndex = 4;
                break;
        }
        EnemyCombat newEnemy = Instantiate(enemyTypes[typeIndex], spawnPositions[index].position, Quaternion.identity).GetComponent<EnemyCombat>();
        return newEnemy;
    }

    private void Update() {
        //Don't update ticks if time is frozen
        if(GameManager.Instance.CurrentGameState != GameManager.GameState.COMBAT){
            return;
        }
        //Go to the customize menu if all the enemies are dead
        if(deadEnemyIndices.Count == enemyCount){
            GameManager.Instance.GoToMenu();
        }
        if(timeGauge.timeFrozen){
            return;
        }
        //Initialize names in the UI after enemies exist
        if(!namesInitialized && enemies[0].enemyName != null){
            cp.InitializeEnemyNames();
            UpdateEnemyParts();
            namesInitialized = true;
        }
        else if(enemies[0].enemyName == null){
            return;
        }
        for(int i = 0; i < timeTicks.Count;i++){
            //If the enemy is dead or confused
            if(deadEnemyIndices.Contains(i) || enemies[i].confused){
                continue;
            }
            //Decrease the ticks based on the enemy's speed
            timeTicks[i] -= enemies[i].pStats.spd * Time.deltaTime * 1.5f;
            //Turn on the windup marker if below 20 ticks
            if(timeTicks[i] <= 10){
                cp.ToggleWindUp(i, true);
            }
            //Uses a skill with a certain chance or attacks a random body part on the player when it's the enemy's turn
            if(timeTicks[i] <= 0){
                int bodyPartIndex = Random.Range(0,randomParts.Count);
                while(player.GetBodyPartHealth(randomParts[bodyPartIndex]) <= 0 && !player.CheckDead()){
                    bodyPartIndex = Random.Range(0,randomParts.Count);
                }
                float skillChance = Random.Range(0,100);
                if(skillChance < 33){
                    //Get the enemy's skill
                    Skill skill = SkillCollection.ReturnSkill(enemies[i].GetSkill());
                    string skillText = enemies[i].enemyName + " used " + skill.name;
                    //Have an indication show up on the screen that the enemy used the skill
                    cp.ChangeEnemySkillText(skillText);
                    //Handle the skill depending on what type of skill it is
                    if(skill.type == Skill.SkillType.BUFF){
                        HandleSkillBuffs(skill, false, enemies[i]);
                    }
                    else if(skill.type == Skill.SkillType.DEBUFF){
                        HandleSkillDebuffs(skill, false);
                    }
                    else if(skill.type == Skill.SkillType.ATTACK){
                        HandleSkillAttacks(skill, false, enemies[i]);
                    }
                    else if(skill.type == Skill.SkillType.HEALING){
                        HandleSkillHealing(skill, false, enemies[i]);
                    }
                }
                else{
                    enemies[i].Attack(player, new BodyPart(randomParts[bodyPartIndex]));
                }
                //Reset the enemy's ticks and turn off the windup marker
                timeTicks[i] = 100;
                cp.ToggleWindUp(i, false);
            }
        }
    }

    //Handles how the buff skill is applied to either the player or enemy depending on isPlayer
    void HandleSkillBuffs(Skill skill, bool isPlayer, EnemyCombat enemy = null){
        print("Used skill: " + skill.name);
        if(skill.wholeBody){
            print("Buffed whole body");
            if(isPlayer){
                player.Buff(skill, null, true);
            }
            else{
                enemy.Buff(skill, null, true);
            }
        }
        else{
            print("Buffed " + currentBodyPart);
            if(isPlayer){
                //Buff the player's selected body part
                BodyPart bodyPart = new BodyPart(currentBodyPart);
                player.Buff(skill, bodyPart);
            }
            else{
                //Buff a random body part of the enemy
                int bodyPartIndex = Random.Range(0,randomParts.Count);
                BodyPart part = new BodyPart(randomParts[bodyPartIndex]);
                enemy.Buff(skill, part);
            }
        }
    }

    //Handles how the debuff skill is applied to either the player or enemy depending on isPlayer
    void HandleSkillDebuffs(Skill skill, bool isPlayer){
        if(skill.wholeBody){
            //Debuffs the whole body of every enemy
            if(skill.multiTarget){
                print("Used skill: " + skill.name + " on all enemies");
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    EnemyCombat enemy = enemies[i];
                    foreach(BodyPart bp in enemy.GetParts()){
                        enemy.Debuff(skill, bp, true);
                    }
                }
            }
            //Debuffs the whole body of a single enemy
            else{
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    //If an enemy is using the protect skill, change the target to that enemy
                    EnemyCombat enemy = enemies[i];
                    if(enemy.protecting){
                        currentEnemyIndex = i;
                    }
                }
                print("Used skill: " + skill.name + " on " + enemies[currentEnemyIndex].enemyName);
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    if(i == currentEnemyIndex){
                        EnemyCombat enemy = enemies[i];
                        foreach(BodyPart bp in enemy.GetParts()){
                            enemy.Debuff(skill, bp, true);
                        }
                    }
                }
            }
        }
        else{
            //Debuffs a single body part of each enemy
            if(skill.multiTarget){
                print("Used skill: " + skill.name + " on all enemies' " + currentBodyPart);
                for(int i = 0; i < enemies.Count; i++){
                    EnemyCombat enemy = enemies[i];
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    foreach(BodyPart bp in enemy.GetParts()){
                        if(bp.body_Part == currentBodyPart){
                            enemy.Debuff(skill, bp);
                        }
                    }
                }
            }
            //Debuffs a single body part of a single enemy or the player
            else{
                print("Used skill: " + skill.name + " on " + enemies[currentEnemyIndex].enemyName + "'s " + currentBodyPart);
                if(isPlayer){
                    for(int i = 0; i < enemies.Count; i++){
                        if(deadEnemyIndices.Contains(i)){
                            continue;
                        }
                        EnemyCombat enemy = enemies[i];
                        if(enemy.protecting){
                            currentEnemyIndex = i;
                        }
                    }
                    for(int i = 0; i < enemies.Count; i++){
                        if(deadEnemyIndices.Contains(i)){
                            continue;
                        }
                        if(i == currentEnemyIndex){
                            EnemyCombat enemy = enemies[i];
                            foreach(BodyPart bp in enemy.GetParts()){
                                if(bp.body_Part == currentBodyPart){
                                    enemy.Debuff(skill, bp);
                                }
                            }
                        }
                    }
                }
                else{
                    int bodyPartIndex = Random.Range(0,randomParts.Count);
                    BodyPart part = new BodyPart(randomParts[bodyPartIndex]);
                    player.Debuff(skill, part);
                }
            }
        }
    }

    //Handles how the skill is used on either the player or enemy(s)
    void HandleSkillAttacks(Skill skill, bool isPlayer, EnemyCombat enemyCaster = null){
        (float, bool) damageTaken = (0, false);
        float playerAtk = player.pStats.atk;
        //Attack all body parts of all enemies
        if(skill.wholeBody){
            if(skill.multiTarget){
                print("Using skill: " + skill.name + " on all enemies");
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    EnemyCombat enemy = enemies[i];
                    foreach(BodyPart bp in enemy.GetParts()){
                        damageTaken = CalculateDamage(skill, playerAtk, enemy.pStats.def, player.pStats.critChance);
                        enemy.TakeDamage(bp, damageTaken.Item1, damageTaken.Item2);
                    }
                }
            }
            //Attack all body parts of an enemy
            else{
                print("Using skill: " + skill.name + " on " + enemies[currentEnemyIndex].enemyName);
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    EnemyCombat enemy = enemies[i];
                    if(enemy.protecting){
                        currentEnemyIndex = i;
                    }
                }
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    if(i == currentEnemyIndex){
                        EnemyCombat enemy = enemies[i];
                        foreach(BodyPart bp in enemy.GetParts()){
                            damageTaken = CalculateDamage(skill, playerAtk, enemy.pStats.def, player.pStats.critChance);
                            enemy.TakeDamage(bp, damageTaken.Item1, damageTaken.Item2);
                        }
                        break;
                    }
                }
            }
        }
        else{
            //Attack one body parts of all enemies
            if(skill.multiTarget){
                print("Using skill: " + skill.name + " on " + currentBodyPart  + " on all enemies");
                for(int i = 0; i < enemies.Count; i++){
                    if(deadEnemyIndices.Contains(i)){
                        continue;
                    }
                    EnemyCombat enemy = enemies[i];
                    foreach(BodyPart bp in enemy.GetParts()){
                        if(bp.body_Part == currentBodyPart){
                            damageTaken = CalculateDamage(skill, playerAtk, enemy.pStats.def, player.pStats.critChance);
                            enemy.TakeDamage(bp, damageTaken.Item1, damageTaken.Item2);
                        }
                    }
                }
                
            }
            //Attack one body part of player/enemy
            else{
                if(isPlayer){
                    print("Using skill: " + skill.name + " on " + currentBodyPart + " on " + enemies[currentEnemyIndex].enemyName);
                    for(int i = 0; i < enemies.Count; i++){
                        if(deadEnemyIndices.Contains(i)){
                            continue;
                        }
                        EnemyCombat enemy = enemies[i];
                        if(enemy.protecting){
                            currentEnemyIndex = i;
                        }
                    }
                    for(int i = 0; i < enemies.Count; i++){
                        if(deadEnemyIndices.Contains(i)){
                            continue;
                        }
                        if(i == currentEnemyIndex){
                            EnemyCombat enemy = enemies[i];
                            foreach(BodyPart bp in enemy.GetParts()){
                                if(bp.body_Part == currentBodyPart){
                                    damageTaken = CalculateDamage(skill, playerAtk, enemy.pStats.def, player.pStats.critChance);
                                    bool hit = enemy.TakeDamage(bp, damageTaken.Item1, damageTaken.Item2);
                                    if(hit) 
                                        enemy.Debuff(skill, bp);
                                }
                            }
                            break;
                        }
                    }
                }
                else{
                    int bodyPartIndex = Random.Range(0,randomParts.Count);
                    BodyPart part = new BodyPart(randomParts[bodyPartIndex]);
                    float enemyAtk = enemyCaster.pStats.atk;
                    damageTaken = CalculateDamage(skill, enemyAtk, player.pStats.def, enemyCaster.pStats.critChance);
                    bool hit = player.TakeDamage(part, damageTaken.Item1, damageTaken.Item2);
                    if(hit){
                        print("Player hit by " + skill.name);
                        player.Debuff(skill, part);
                    }
                }
            }
        }
        UpdateEnemyParts();
        if(isPlayer && timeTicks[currentEnemyIndex] <= 10){
            cp.ToggleWindUp(currentEnemyIndex, false);
            timeTicks[currentEnemyIndex] = 35;
        }
    }

    //Calculate how much damage a skill does based on the user's atk, target's def, and user's crit chance
    (float, bool) CalculateDamage(Skill skill, float atk, float def, float critChance){
        float damageTaken = Mathf.RoundToInt(atk * atk / (atk + def) * skill.multiplier); 
        float crit = Random.Range(0,100);
        bool isCrit = false;
        if(crit <= critChance){
            isCrit = true;
        }
        return (damageTaken, isCrit);
    }

    //Get a list of all the parts of all the enemies and update the UI accordingly
    public void UpdateEnemyParts(){
        List<List<BodyPart>> enemyParts = new List<List<BodyPart>>();
        foreach(EnemyCombat enemy in enemies){
            enemyParts.Add(enemy.GetParts(true));
        }
        cp.UpdateEnemyParts(enemyParts);
    }

    //Handles how the player/enemy uses a healing skill
    void HandleSkillHealing(Skill skill, bool isPlayer, EnemyCombat enemy = null){
        print("Used skill: " + skill.name);
        if(isPlayer){
            if(skill.wholeBody){
                float healAmnt = player.pStats.health / 20 *  skill.multiplier;
                player.Heal(healAmnt, null, true);
            }
            else{
                BodyPart bp = new BodyPart(currentBodyPart);
                float healAmnt = player.pStats.health / 20 *  skill.multiplier;
                player.Heal(healAmnt, bp);
            }   
        }
        else{
            if(skill.multiTarget){
                if(skill.wholeBody){
                    print("Party heal");
                    for(int i = 0; i < enemies.Count; i++){
                        if(deadEnemyIndices.Contains(i)){
                            continue;
                        }
                        float healAmount = enemies[i].pStats.health / 20 * skill.multiplier;
                        enemies[i].Heal(healAmount , null, true);
                    }
                }
            }
        }
    }

    //Adjusts the enemy index given by the combat panel based on dead enemies
    public void UpdateEnemyIndex(int index){
        for(int i = 0; i <= index; i++){
            if(deadEnemyIndices.Contains(i)){
                index++;
            }
        }
        currentEnemyIndex = index;
        TakeAction();
    }

    //Returns the adjusted enemy index after accounting for dead enemies
    public int GetEnemyIndex(int index){
        for(int i = 0; i <= index; i++){
            if(deadEnemyIndices.Contains(i)){
                index++;
            }
        }
        return index;
    }

    //Updates the current body part target
    public void UpdateBody(string bName){
        currentBodyPart = bName;
        TakeAction();
    }
    
    //Updates the current action
    public void UpdateAction(string aName){
        currentAction = aName;
        TakeAction();
    }

    //Updates the current item
    public void UpdateItem(string iName){
        currentItem = iName;
        TakeAction();
    }

    //Updates the current skill
    public void UpdateSkill(string sName){
        currentSkill = sName;
        TakeAction();
    }

    //Does nothing if the player's move is not valid yet, otherwise acts based on the current action, target, and body part
    public void TakeAction(){
        switch (currentAction)
        {
            case "Attack":
                Attack();
                break;
            case "Items": 
                UseItem();
                break;
            case "Skills":
                UseSkill();
                break;
            case "Run Away":
                RunAway();
                break;
        }
    }

    void Attack(){
        //Don't attack if no body part selected
        if(currentBodyPart == "")
            return;
        //If an enemy is using the protect skill, change the target to that enemy
        for(int i = 0; i < enemies.Count; i++){
            if(deadEnemyIndices.Contains(i)){
                continue;
            }
            EnemyCombat enemy = enemies[i];
            if(enemy.protecting){
                currentEnemyIndex = i;
            }
        }
        print("Player attacked " + enemies[currentEnemyIndex].enemyName + " at " + currentBodyPart);
        player.Attack(enemies[currentEnemyIndex], new BodyPart(currentBodyPart));
        UpdateEnemyParts();
        //Stagger the enemy and delay its turn
        if(timeTicks[currentEnemyIndex] <= 10){
            cp.ToggleWindUp(currentEnemyIndex, false);
            timeTicks[currentEnemyIndex] = 35;
        }
        timeGauge.DecreaseTime(attackCost);
        cp.SwitchPanel("Initial");
    }

    //Gets the selected item from the inventory and uses it on either the player or enemies
    void UseItem(){
        if(currentItem == "")
            return;
        Item itemUsed = Inventory.GetItem(currentItem);
        if(!itemUsed.wholeBody && currentBodyPart == "")
            return;
        print("Player used item: " + currentItem);
        //Use item on player
        timeGauge.DecreaseTime(itemUsed.cost);
        if(itemUsed.targetPlayer){
            itemUsed.Use(player);
        }
        //Use item on enemies
        else{
            for(int i = 0; i < enemies.Count; i++){
                if(deadEnemyIndices.Contains(i)){
                    continue;
                }
                itemUsed.Use(enemies[i]);
                //Stagger the enemies and make them move later
                if(timeTicks[i] <= 10){
                    cp.ToggleWindUp(i, false);
                    timeTicks[i] = 35;
                }
            }
            UpdateEnemyParts();
        }
        cp.SwitchPanel("Initial");
    }

    //Uses the current skill selected on either the player or enemy
    void UseSkill(){
        //Don't use the skill if the conditions aren't valid
        if(currentSkill == "")
            return;
        Skill skill = SkillCollection.ReturnSkill(currentSkill);
        //No body part targeted
        if(!skill.wholeBody && currentBodyPart == "")
            return;
        //Enemy not selected
        if(!skill.multiTarget && skill.type != Skill.SkillType.BUFF && 
                skill.type != Skill.SkillType.HEALING && currentEnemyIndex == -1)
            return;
        float skillCost = SkillCollection.ReturnSkill(currentSkill).timeCost;
        timeGauge.DecreaseTime(skillCost);
        //Handle each type of skill
        if(skill.type == Skill.SkillType.BUFF){
            HandleSkillBuffs(skill, true);
        }
        else if(skill.type == Skill.SkillType.DEBUFF){
            HandleSkillDebuffs(skill, true);
        }
        else if(skill.type == Skill.SkillType.ATTACK){
            HandleSkillAttacks(skill, true);
        }
        else if(skill.type == Skill.SkillType.HEALING){
            HandleSkillHealing(skill, true);
        }
        cp.SwitchPanel("Initial");
    }

    //Uses all of the player's time gauge to run away, higher chance of running if time gauge is more filled
    void RunAway(){
        float chance = 0.2f + timeGauge.GetValue() * 0.8f / 0.6f;
        timeGauge.DecreaseTime(timeGauge.GetComponent<TimeGauge>().GetValue());
        if(Random.Range(0.0f,1.0f) < chance){
            print("Ran Away Successfully");
            GameManager.Instance.GoToMenu();
        }
        else{
            print("Failed to run away");
        }
    }

    //Tells the combat panel if an enemy was hit or not
    public void SetHit(int enemyId, string hitMissCrit){
        cp.SetHit(enemyId, hitMissCrit);
    }

    //Resets the player's health back to full
    public void ResetPlayerHealth(){
        if(player)
            player.ResetHealth();
    }

    //Get a list of all the enemy names
    public List<string> GetEnemyNames(){
        List<string> enemyNames = new List<string>();
        for(int i = 0; i < enemies.Count; i++){
            if(deadEnemyIndices.Contains(i)){
                continue;
            }
            EnemyCombat enemy = enemies[i];
            enemyNames.Add(enemy.enemyName);
        }
        return enemyNames;
    }

    //Get a list of all the body parts of a given enemy
    public List<string> GetBodyParts(string enemyName){
        List<string> bodyParts = new List<string>();
        for(int i = 0; i < enemies.Count; i++){
            if(deadEnemyIndices.Contains(i)){
                continue;
            }
            EnemyCombat enemy = enemies[i];
            if(enemyName.Equals(enemy.enemyName)){
                foreach(BodyPart bp in enemy.GetParts()){
                    bodyParts.Add(bp.body_Part);
                }
                return bodyParts;
            }
        }
        if(bodyParts.Count == 0){
            foreach(BodyPart bp in enemies[0].GetParts()){
                bodyParts.Add(bp.body_Part);
            }
        }
        return bodyParts;
    }

    //If an enemy died, add its index to a list and have the combat panel remove it from the UI
    public void EnemyDied(EnemyCombat enemy){
        deadEnemyIndices.Add(enemies.IndexOf(enemy));
        cp.RemoveEnemy(enemies.IndexOf(enemy));
    }

    //Allows the player to tell the time gauge to increase how fast it recharges and decrease how fast it depletes
    public void ToggleMindBuff(bool buffed){
        timeGauge.ToggleMindBuff(buffed);
    }

    //Allows the player to tell the time gauge to decrease how fast it recharges and increase how fast it depletes
    public void ToggleMindDebuff(bool debuffed){
        timeGauge.ToggleMindDebuff(debuffed);
    }

    public void IncreaseTimeGauge(float amountIncreased){
        timeGauge.IncreaseTime(amountIncreased);
    }

    public List<Transform> GetEnemyPositions(){
        return spawnPositions;
    }

    public void SetCombatPanel(CombatPanel combatPanel){
        cp = combatPanel;
    }

    public List<string> GetPlayerBodyParts(){
        return player.GetBodyParts();
    }

    public List<Skill> GetPlayerSkills(){
        return player.GetSkills();
    }
    

}
