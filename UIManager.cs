using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject combatPanel;
    [SerializeField] GameObject customizePanel;
    [SerializeField] GameObject cancelButton;

    PlayerControls controls;
    //Used to make sure when returning to battle, scale is 0 if time is stopped
    float prevTimeScale = 1;

    protected override void Awake()
    {
        controls = new PlayerControls();
        base.Awake();
        controls.Combat.Menu.performed += ctx => Pause();
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        GameManager.onGameStateChanged += InitiateCombat;
        GameManager.onGameStateChanged += CustomizeMenu;
    }

    //Turn on the menu and the cancel button
    void Pause(){
        if(GameManager.Instance.CurrentGameState == GameManager.GameState.MENU){
            cancelButton.SetActive(false);
            GameManager.Instance.EnterCombat();
        }
        else if(GameManager.Instance.CurrentGameState == GameManager.GameState.COMBAT){
            GameManager.Instance.GoToMenu();
            cancelButton.SetActive(true);
        }
    }

    //When the gamestate is changed to COMBAT, set the combat panel to active
    void InitiateCombat(GameManager.GameState previousState, GameManager.GameState currentState){
        if(currentState == GameManager.GameState.COMBAT){
            combatPanel.SetActive(true);
        }
    }

    //When the gamestate is changed to MENU, set the customize panel to active and freeze time, cancel button is not on
    void CustomizeMenu(GameManager.GameState previousState, GameManager.GameState currentState){
        if(currentState == GameManager.GameState.MENU){
            cancelButton.SetActive(false);
            customizePanel.SetActive(true);
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else{
            customizePanel.SetActive(false);
            cancelButton.SetActive(false);
            Time.timeScale = prevTimeScale;
        }
    }

    //Used for time gauge to tell combat panel to go back to the stop time panel
    public void ResetCombat(){
        combatPanel.GetComponent<CombatPanel>().ResetRealTime();
    }

    private void OnEnable() {
        controls.Combat.Enable();
    }

    private void OnDisable() {
        controls.Combat.Disable();
    }

}
