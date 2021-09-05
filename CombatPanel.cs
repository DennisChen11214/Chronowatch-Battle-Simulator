using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CombatPanel : MonoBehaviour
{
    //All the panels and UI elements relating to combat
    [SerializeField] GameObject stopTimePanel;
    [SerializeField] GameObject initialPanel;
    [SerializeField] GameObject targetPanel;
    [SerializeField] GameObject bodyPartPanel;
    [SerializeField] GameObject itemPanel;
    [SerializeField] GameObject skillsPanel;
    [SerializeField] GameObject timeGauge;
    [SerializeField] GameObject enemyMarker;
    [SerializeField] GameObject enemySkillText;
    [SerializeField] List<GameObject> windupMarker;
    [SerializeField] GameObject playerParts;
    [SerializeField] List<GameObject> enemyParts;
    [SerializeField] Sprite increaseMarker;
    [SerializeField] Sprite decreaseMarker;
    //Indicates whether the player/enemies hit or dodged an attack
    [SerializeField] GameObject playerHitText;
    [SerializeField] GameObject enemyHitText1;
    [SerializeField] GameObject enemyHitText2;
    [SerializeField] GameObject enemyHitText3;
    GameObject currentPanel;
    PlayerControls controls;
    //Used to stop the coroutines when they're repeated
    Coroutine coroutine;
    Coroutine hitText1Coroutine;
    Coroutine hitText2Coroutine;
    Coroutine hitText3Coroutine;
    Coroutine pHitTextCoroutine;

    //Enables keybind that changes to the previous panel
    private void Awake() {
        controls = new PlayerControls();
        controls.Combat.PreviousPanel.performed += ctx => PrevPanel();
    }

    private void Update() {
        //In the panel we're currently in, check to see what we have enough time gauge for
        if(initialPanel.activeSelf){
            CheckCostInitial();
        }
        else if(skillsPanel.activeSelf){
            CheckCostSkills();
        }
        else if(itemPanel.activeSelf){
            CheckCostItems();
        }
        //If we chose the action attack, but no longer have enough time gauge to attack, go back to the action panel
        if(CombatManager.Instance.currentAction == "Attack" && timeGauge.GetComponent<TimeGauge>().GetValue() < CombatManager.Instance.attackCost){
            SwitchPanel(initialPanel);
        }
        //If we chose a skill, but no longer have enough time gauge to use it, go back to the action panel
        if(CombatManager.Instance.currentAction == "Skill" && timeGauge.GetComponent<TimeGauge>().GetValue() < SkillCollection.ReturnSkill(CombatManager.Instance.currentSkill).timeCost){
            SwitchPanel(initialPanel);
        }
        //If we're currently selecting an enemy or an enemy is already selected, move the enemy marker over them
        if(currentPanel == targetPanel || CombatManager.Instance.currentEnemyIndex != -1){
            enemyMarker.SetActive(true);
            int enemyIndex;
            if(CombatManager.Instance.currentEnemyIndex != -1){
                enemyIndex = CombatManager.Instance.currentEnemyIndex;
            }
            else{
                enemyIndex = CombatManager.Instance.GetEnemyIndex(EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
            }
            //Calculate the position of the enemy marker based on the selected enemy
            RectTransform canvasRect = transform.parent.GetComponent<RectTransform>();
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Vector2 viewPortPos = cam.WorldToViewportPoint(CombatManager.Instance.GetEnemyPositions()[enemyIndex].position + new Vector3(0,1,0));
            Vector2 markerScreenPos = new Vector2(viewPortPos.x * canvasRect.sizeDelta.x , 
                                                  viewPortPos.y * canvasRect.sizeDelta.y);
            enemyMarker.GetComponent<RectTransform>().anchoredPosition = markerScreenPos;
        }
        else{
            enemyMarker.SetActive(false);
        }
    }

    /*
        Check if we have enough time gauge to attack or run away.
        If not, grey out the buttons and make them uninteractable.
        If we do, return them to normal
        If we're currently selected on a button that's greyed out, move the selection to the next button
    */
    void CheckCostInitial(){
        if(timeGauge.GetComponent<TimeGauge>().GetValue() < CombatManager.Instance.attackCost && initialPanel.transform.GetChild(0).GetComponent<Button>().GetComponent<ColorSelector>().initialColor == Color.black){
            initialPanel.transform.GetChild(0).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.gray;
            initialPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
            initialPanel.GetComponent<ButtonSelector>().SelectNext();
        }
        else if(timeGauge.GetComponent<TimeGauge>().GetValue() >= CombatManager.Instance.attackCost){
            initialPanel.transform.GetChild(0).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.black;
            initialPanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
        if(timeGauge.GetComponent<TimeGauge>().GetValue() < CombatManager.Instance.runAwayCost && initialPanel.transform.GetChild(3).GetComponent<Button>().GetComponent<ColorSelector>().initialColor == Color.black){
            initialPanel.transform.GetChild(3).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.gray;
            initialPanel.transform.GetChild(3).GetComponent<Button>().interactable = false;
            initialPanel.GetComponent<ButtonSelector>().SelectNext();
        }
        else if(timeGauge.GetComponent<TimeGauge>().GetValue() >= CombatManager.Instance.runAwayCost){
            initialPanel.transform.GetChild(3).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.black;
            initialPanel.transform.GetChild(3).GetComponent<Button>().interactable = true;
        }
    }
    
    /*
        For each skill, check if we have enough time gauge to use it.
        If not, grey out the button and make it uninteractable.
        If we do, return it to normal
        If we're currently selected on a button that's greyed out, move the selection to the next button
    */
    void CheckCostSkills(){
        for(int i = 0; i < skillsPanel.transform.childCount; i++){
            string skillName = skillsPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text;
            float cost = SkillCollection.ReturnSkill(skillName).timeCost;
            if(timeGauge.GetComponent<TimeGauge>().GetValue() < cost){
                skillsPanel.transform.GetChild(i).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.gray;
                skillsPanel.transform.GetChild(i).GetComponent<Button>().interactable = false;
                skillsPanel.GetComponent<ButtonSelector>().SelectNext();
            }
            else{
                skillsPanel.transform.GetChild(i).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.black;
                skillsPanel.transform.GetChild(i).GetComponent<Button>().interactable = true;
            }
        }
    }

    /*
        For each item, check if we have enough time gauge to use it.
        If not, grey out the button and make it uninteractable.
        If we do, return it to normal
        If we're currently selected on a button that's greyed out, move the selection to the next button
    */
    void CheckCostItems(){
        for(int i = 0; i < itemPanel.transform.GetChild(0).transform.childCount; i++){
            string itemName = itemPanel.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text;
            float cost = Inventory.GetItem(itemName).cost;
            if(timeGauge.GetComponent<TimeGauge>().GetValue() < cost){
                itemPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.gray;
                itemPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                itemPanel.transform.GetChild(0).GetComponent<ButtonSelector>().SelectNext();
            }
            else{
                itemPanel.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().GetComponent<ColorSelector>().initialColor = Color.black;
                itemPanel.transform.GetChild(0).transform.GetChild(i).GetComponent<Button>().interactable = true;
            }
        }
    }
    
    /*
        Turns the current panel inactive and switches to the given panel.
        Sets up the panel and highlights the first button that we have the time gauge to press
    */
    private void SwitchPanel(GameObject panel){
        if(panel == targetPanel){
            SetUpTargetPanel();
        }
        else if(panel == bodyPartPanel){
            SetUpBodyPanel(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);
        }
        else if(panel == stopTimePanel){
            timeGauge.GetComponent<TimeGauge>().InRealTime();
        }
        //Resets all the information that the combat manager has about what the player is going to do
        else if(panel == initialPanel){
            CombatManager.Instance.UpdateAction("");
            CombatManager.Instance.UpdateBody("");
            CombatManager.Instance.UpdateEnemyIndex(-1);
            CombatManager.Instance.UpdateSkill("");
            CombatManager.Instance.UpdateItem("");
            CheckCostInitial();
            initialPanel.GetComponent<ButtonSelector>().SelectFirst();
        }
        else if(panel == skillsPanel){
            SetUpSkillPanel();
            CheckCostSkills();
            skillsPanel.GetComponent<ButtonSelector>().SelectFirst();
        }
        else if(panel == itemPanel){
            SetUpItemPanel();
            CheckCostItems();
            itemPanel.transform.GetChild(0).gameObject.GetComponent<ButtonSelector>().SelectFirst();
        }
        currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);
    }

    //Allows the combat manager to switch panels
    public void SwitchPanel(string panel){
        switch (panel)
        {
            case "Initial":
                SwitchPanel(initialPanel);
                break;
            case "Target":
                SwitchPanel(targetPanel);
                break;
            case "Body Part":
                SwitchPanel(bodyPartPanel);
                break;
            case "Skills":
                SwitchPanel(skillsPanel);
                break;
            case "Items":
                SwitchPanel(itemPanel);
                break;
            case "Stop":
                SwitchPanel(stopTimePanel);
                break;
        }
    }

    //Gets a list of all the enemy names and sets up the buttons accordingly
    void SetUpTargetPanel(){
        List<string> enemyNames = CombatManager.Instance.GetEnemyNames();
        for(int i = 0; i < enemyNames.Count; i++){
            targetPanel.transform.GetChild(i).gameObject.SetActive(true);
            targetPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = enemyNames[i];
        }
        for(int i = enemyNames.Count; i < 4; i++){
            targetPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //Gets a list of the player or enemy bodyparts and sets up the buttons accordingly
    void SetUpBodyPanel(string enemyName){
        Skill skill = SkillCollection.ReturnSkill(CombatManager.Instance.currentSkill);
        List<string> bodyParts = CombatManager.Instance.GetBodyParts(enemyName);
        //Set up with the player body parts if the player is using a buff/healing skill
        if(skill != null && (skill.type == Skill.SkillType.HEALING || skill.type == Skill.SkillType.BUFF)){
            bodyParts = CombatManager.Instance.GetPlayerBodyParts();
        }
        for(int i = 0; i < bodyParts.Count; i++){
            bodyPartPanel.transform.GetChild(i).gameObject.SetActive(true);
            bodyPartPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = bodyParts[i];
        }
        for(int i = bodyParts.Count; i < 6; i++){
            bodyPartPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //Gets a list of the player's skills and sets the buttons accordingly
    void SetUpSkillPanel(){
        List<Skill> skills = CombatManager.Instance.GetPlayerSkills();
        for(int i = 0; i < skills.Count; i++){
            skillsPanel.transform.GetChild(i).gameObject.SetActive(true);
            skillsPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = skills[i].name;
        }
        for(int i = skills.Count; i < 4; i++){
            skillsPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    //Gets a list of the player's items and sets the buttons accordingly
    void SetUpItemPanel(){
        int index = 0;
        for(int i = 0 ; i < Inventory.inventory.Count; i++){
            if(Inventory.inventory[i].amount > 0){
                itemPanel.transform.GetChild(0).GetChild(index).gameObject.SetActive(true);
                itemPanel.transform.GetChild(0).GetChild(index).GetChild(0).GetComponent<Text>().text = Inventory.inventory[i].itemName;
                index++;
            }
        }
        for(int i = index; i < 6; i++){
            itemPanel.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
        RectTransform itemScroll = itemPanel.transform.GetChild(0).GetComponent<RectTransform>();
        itemScroll.sizeDelta = new Vector2(itemScroll.sizeDelta.x, 60 * index);
        itemScroll.anchoredPosition = new Vector3(0,0,0);
    }
    //Sends to the combat manager what action the player clicked and switches to that panel
    public void SendAction(){
        string action = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        CombatManager.Instance.UpdateAction(action);
        switch (action)
        {
            case "Attack":
                SwitchPanel(targetPanel);
                break;
            case "Skills":
                SwitchPanel(skillsPanel);
                break;
            case "Items":
                SwitchPanel(itemPanel);
                break;
            case "Run Away":
                SwitchPanel(stopTimePanel);
                break;            
        }
    }
    
    //Sends to the combat manager what the body part the player clicked
    public void SendBodyPart(){
        CombatManager.Instance.UpdateBody(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);
    }
    //Sends to the combat manager what the index of the enemy button clicked is and switches to the body part panel
    public void SendEnemyIndex(){
        CombatManager.Instance.UpdateEnemyIndex(EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex());
        if(CombatManager.Instance.currentAction == "Attack"){
            SwitchPanel(bodyPartPanel);
        }
        else if(CombatManager.Instance.currentAction == "Skills"){
            Skill curSkill = SkillCollection.ReturnSkill(CombatManager.Instance.currentSkill);
            if(!curSkill.wholeBody){
                SwitchPanel(bodyPartPanel);    
            }
        }
    }
    //Sends to the combat manager what skill the player clicked
    public void SendSkill(){
        string skillName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        CombatManager.Instance.UpdateSkill(skillName);
        Skill curSkill = SkillCollection.ReturnSkill(skillName);
        if(curSkill.multiTarget || curSkill.type == Skill.SkillType.BUFF || curSkill.type == Skill.SkillType.HEALING){
            if(!curSkill.wholeBody)
                SwitchPanel(bodyPartPanel);
        }
        else{
            SwitchPanel(targetPanel);
        }
    }
    //Sends to the combat manager what item the player clicked
    public void SendItem(){
        string itemName = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        CombatManager.Instance.UpdateItem(itemName);
        Item item = Inventory.GetItem(itemName);
        if(!item.wholeBody){
            SwitchPanel(bodyPartPanel);
        }
    }

    //Return a reference to the time gauge for the combat manager to use
    public TimeGauge GetTimeGauge(){
        return timeGauge.GetComponent<TimeGauge>();
    }
    
    //Switches to the previous panel and resets the information in the combat manager accordingly
    void PrevPanel(){
        if(currentPanel == targetPanel){
            CombatManager.Instance.UpdateAction("");
            CombatManager.Instance.UpdateSkill("");
            CombatManager.Instance.UpdateItem("");
            SwitchPanel(initialPanel);
        }
        else if(currentPanel == bodyPartPanel){
            print(CombatManager.Instance.currentAction);
            //Check if we're targeting the player
            if(CombatManager.Instance.currentEnemyIndex == -1){
                CombatManager.Instance.UpdateEnemyIndex(-1);
                //If we're using a buff skill
                if(CombatManager.Instance.currentAction == "Skills"){
                    SwitchPanel(skillsPanel);
                }
                //If we're using a consumable item
                else if(CombatManager.Instance.currentAction == "Items"){
                    SwitchPanel(itemPanel);
                }
            }
            else{
                CombatManager.Instance.UpdateEnemyIndex(-1);
                SwitchPanel(targetPanel);
            }
        }
        else if(currentPanel == itemPanel){
            CombatManager.Instance.UpdateAction("");
            SwitchPanel(initialPanel);
        }
        else if(currentPanel == skillsPanel){
            CombatManager.Instance.UpdateAction("");
            SwitchPanel(initialPanel);
        }
        else if(currentPanel == initialPanel){
            SwitchPanel(stopTimePanel);
        }
    }

    //Depending on the health of each of the player's body parts, changes the color of that body part on the player diagram
    void UpdatePlayerParts(List<BodyPart> parts){
        foreach(BodyPart bp in parts){
            int bodyPartIndex = 0;
            switch (bp.body_Part)
            {
                case "Head":
                    bodyPartIndex = 0;
                    break;
                case "Body":
                    bodyPartIndex = 1;
                    break;
                case "Right Arm":
                    bodyPartIndex = 2;
                    break;
                case "Left Arm":
                    bodyPartIndex = 3;
                    break;
                case "Right Leg":
                    bodyPartIndex = 4;
                    break;
                case "Left Leg":
                    bodyPartIndex = 5;
                    break;                
            }
            if(bp.bPartHealth > 0.7f * bp.maxHealth){
                playerParts.transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.green;
            }
            else if(bp.bPartHealth > 0.35f * bp.maxHealth){
                playerParts.transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.yellow;
            }
            else if(bp.bPartHealth > 0){
                playerParts.transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.red;
            }
            else{
                playerParts.transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.black;
            }
        }
    }

    //Sets the enemy diagram and name to inactive, called when the enemy dies
    public void RemoveEnemy(int index){
        enemyParts[index].SetActive(false);
    }

    /*
        Given a list of each enemy's body parts, it updates their body diagram according to their health, much like the player one
    */
    public void UpdateEnemyParts(List<List<BodyPart>> parts){
        for(int i = 0; i < parts.Count; i++){
            foreach(BodyPart bp in parts[i]){
                int bodyPartIndex = 0;
                switch (bp.body_Part)
                {
                    case "Head":
                        bodyPartIndex = 0;
                        break;
                    case "Body":
                        bodyPartIndex = 1;
                        break;
                    case "Right Arm":
                        bodyPartIndex = 2;
                        break;
                    case "Left Arm":
                        bodyPartIndex = 3;
                        break;
                    case "Right Leg":
                        bodyPartIndex = 4;
                        break;
                    case "Left Leg":
                        bodyPartIndex = 5;
                        break;                
                }
                if(bp.bPartHealth > 0.7f * bp.maxHealth){
                    enemyParts[i].transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.green;
                }
                else if(bp.bPartHealth > 0.35f * bp.maxHealth){
                    enemyParts[i].transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.yellow;
                }
                else if(bp.bPartHealth > 0){
                    enemyParts[i].transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.red;
                }
                else{
                    enemyParts[i].transform.GetChild(0).GetChild(bodyPartIndex).GetComponent<Image>().color = Color.black;
                }
            }
        }
    }

    //Sets the names of each of the enemies on one of the panels
    public void InitializeEnemyNames(){
        List<string> enemyNames = CombatManager.Instance.GetEnemyNames();
        for(int i = 0; i < CombatManager.Instance.enemyCount; i++){
            enemyParts[i].GetComponent<Text>().text = enemyNames[i];            
            enemyParts[i].SetActive(true);
        }
        for(int i = CombatManager.Instance.enemyCount; i < 3; i++ ){
            enemyParts[i].SetActive(false);
        }
    }

    //Turn on/off the marker that shows that the enemy is about to attack, index is which enemy is going to attack
    public void ToggleWindUp(int index, bool isOn){
        windupMarker[index].SetActive(isOn);
    }

    //Displays what skill the enemy used onto the screen
    public void ChangeEnemySkillText(string skillText){
        if(coroutine != null){
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SkillTextDuration(skillText));
    }

    //The skill text is displayed for 3 seconds
    IEnumerator SkillTextDuration(string skillText){
        enemySkillText.GetComponent<Text>().text = skillText;
        yield return new WaitForSecondsRealtime(3);
        enemySkillText.GetComponent<Text>().text = "";
        coroutine = null;
    }

    //Switch back to the stop time panel when time is no longer stopped
    public void ResetRealTime(){
        SwitchPanel(stopTimePanel);
    }

    //Gets a list of the buffs for an enemy with the UI objects
    public List<BuffInfo> BuffObjects(int index){
        List<BuffInfo> buffObjects = new List<BuffInfo>();
        Transform buffParent = enemyParts[index].transform.GetChild(1);
        for(int i = 0; i < buffParent.childCount; i++){
            BuffInfo bInfo = new BuffInfo(buffParent.GetChild(i).gameObject);
            buffObjects.Add(bInfo);
        }
        return buffObjects;
    }

    //Gets a list of the buffs for the player with the UI objects
    public List<BuffInfo> PlayerBuffObject(){
        List<BuffInfo> buffObjects = new List<BuffInfo>();
        Transform buffParent = playerParts.transform.GetChild(1);
        for(int i = 0; i < buffParent.childCount; i++){
            BuffInfo bInfo = new BuffInfo(buffParent.GetChild(i).gameObject);
            buffObjects.Add(bInfo);
        }
        return buffObjects;
    }

    //Set the corresponding coroutine for whoever was hit/crit/missed
    public void SetHit(int enemyId, string hitMissCrit){
        if(enemyId == -1){
            pHitTextCoroutine = StartCoroutine(HitText(enemyId, hitMissCrit));
        }
        else if(enemyId == 0){
            hitText1Coroutine = StartCoroutine(HitText(enemyId, hitMissCrit));
        }
        else if(enemyId == 1){
            hitText2Coroutine = StartCoroutine(HitText(enemyId, hitMissCrit));
        }
        else{
            hitText3Coroutine = StartCoroutine(HitText(enemyId, hitMissCrit));
        }
    }

    //Shows whether a certain enemy was hit, crit, or dodged an attack for 1 second
    IEnumerator HitText(int enemyId, string hitMissCrit){
        if(enemyId == -1){
            //If something is already showing, stop it and only show this one
            if(pHitTextCoroutine != null)
                StopCoroutine(pHitTextCoroutine);
            playerHitText.GetComponent<Text>().text = hitMissCrit;
            playerHitText.SetActive(true);
        }
        else if(enemyId == 0){
            if(hitText1Coroutine != null)
                StopCoroutine(hitText1Coroutine);
            enemyHitText1.GetComponent<Text>().text = hitMissCrit;
            enemyHitText1.SetActive(true);
        }
        else if(enemyId == 1){
            if(hitText2Coroutine != null)
                StopCoroutine(hitText2Coroutine);
            enemyHitText2.GetComponent<Text>().text = hitMissCrit;
            enemyHitText2.SetActive(true);
        }
        else{
            if(hitText3Coroutine != null)
                StopCoroutine(hitText3Coroutine);
            enemyHitText3.GetComponent<Text>().text = hitMissCrit;
            enemyHitText3.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(1);
        if(enemyId == -1){
            playerHitText.GetComponent<Text>().text = "";
            playerHitText.SetActive(false);
        }
        else if(enemyId == 0){
            enemyHitText1.GetComponent<Text>().text = "";
            enemyHitText1.SetActive(false);
        }
        else if(enemyId == 1){
            enemyHitText2.GetComponent<Text>().text = "";
            enemyHitText2.SetActive(false);
        }
        else{
            enemyHitText3.GetComponent<Text>().text = "";
            enemyHitText3.SetActive(false);
        }
    }

    //Resets the buff, markers, and hit text UI
    public void ResetUI(){
        Transform buffParent1 = enemyParts[0].transform.GetChild(1);
        for(int i = 0; i < buffParent1.childCount; i++){
            buffParent1.GetChild(i).gameObject.SetActive(false);
        }
        Transform buffParent2 = enemyParts[1].transform.GetChild(1);
        for(int i = 0; i < buffParent2.childCount; i++){
            buffParent2.GetChild(i).gameObject.SetActive(false);
        }
        Transform buffParent3 = enemyParts[2].transform.GetChild(1);
        for(int i = 0; i < buffParent3.childCount; i++){
            buffParent3.GetChild(i).gameObject.SetActive(false);
        }
        Transform buffParentPlayer = playerParts.transform.GetChild(1);
        for(int i = 0; i < buffParentPlayer.childCount; i++){
            buffParentPlayer.GetChild(i).gameObject.SetActive(false);
        }
        enemyMarker.SetActive(false);
        windupMarker[0].SetActive(false);
        windupMarker[1].SetActive(false);
        windupMarker[2].SetActive(false);
        enemyHitText1.SetActive(false);
        enemyHitText2.SetActive(false);
        enemyHitText3.SetActive(false);
        playerHitText.SetActive(false);
    }

    void OnEnable(){
        CombatManager.Instance.SetCombatPanel(this);
        currentPanel = stopTimePanel;
        controls.Combat.Enable();
        PlayerCombat.damageTaken += UpdatePlayerParts;
    }

    void OnDisable(){
        controls.Combat.Disable();
        PlayerCombat.damageTaken -= UpdatePlayerParts;
    }
}
