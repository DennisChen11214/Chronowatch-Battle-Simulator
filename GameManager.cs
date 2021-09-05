using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    public enum GameState{
        MENU,
        COMBAT,
    }

    #region Public Vars
    public delegate void GameStateChanged(GameState current, GameState changeTo);
    public static event GameStateChanged onGameStateChanged;
    public GameObject[] systemManagers;
    public GameState CurrentGameState{
        get{ return currentGameState; }
    }
    #endregion
    
    #region Private Vars
    List<GameObject> instantiatedSystemManagers;
    string currentSceneName;
    GameState currentGameState = GameState.MENU;
    #endregion

    private void Start() {
        DontDestroyOnLoad(gameObject);
        instantiatedSystemManagers = new List<GameObject>();
        InstantiateSystemManagers();
        LoadScene("Combat Test");
        GoToMenu();
        Inventory.InitializeInventory();
    }

    //Load a scene asynchronously
    public void LoadScene(string sceneName){
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if(ao == null)
        {
            Debug.LogError("Unable to Load Level: " + sceneName);
            return;
        }
        currentSceneName = sceneName;
        ao.completed += OnLoadComplete;
    }

    public void UnloadScene(string sceneName){
        AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneName);
    }

    void OnLoadComplete(AsyncOperation ao){
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
    }

    /*
    Updates the gamestate and acts according to the new state
    */
    void UpdateState(GameState state){
        GameState previous = currentGameState;
        currentGameState = state;
        switch(currentGameState){
            case GameState.MENU:
                break;
            case GameState.COMBAT:
                break;
        }
        if(onGameStateChanged != null){
            onGameStateChanged(previous, currentGameState);
        }
    }

    /*
    Instantiates all the system managers into the scene
    */
    void InstantiateSystemManagers(){
        foreach(GameObject manager in systemManagers){
            instantiatedSystemManagers.Add(Instantiate(manager));
        }
    }
    
    //Enter the COMBAT game state
    public void EnterCombat(){
        UpdateState(GameState.COMBAT);
    }

    //Enter the MENU game state
    public void GoToMenu(){
        UpdateState(GameState.MENU);
    }

    /*
    Destroys the system managers along with the game manager
    */
    protected override void OnDestroy()
    {
        foreach(GameObject manager in instantiatedSystemManagers){
            Destroy(manager);
        }
        instantiatedSystemManagers.Clear();
        base.OnDestroy();
    }
}
