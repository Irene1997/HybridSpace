using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    static GameController instance;

    //public enum PlayerState { Alive, Dead };


    public GameObject player;
    //public PlayerState playerState;

    public GameObject enemies;

    [SerializeField]
    EnemyBehaviour[] enemyControllers;

    public DoorScript[] doors;

    // Use this for initialization
    void Start() {

        //load player and enemies
        instance = this;
        if (player == null) {
            player = GameObject.Find("PLayer");
            //playerState = PlayerState.Alive;
            if (player == null) { Debug.LogWarning("No player could be found."); }
        }
        if (enemies == null) {
            enemies = GameObject.Find("Enemies");
            if (enemies == null) { Debug.LogWarning("No enemies could be found."); }
        }
        enemyControllers = enemies.GetComponentsInChildren<EnemyBehaviour>();
    }

    // Update is called once per frame
    void Update() {

    }

    public static GameController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameController>();
                if (instance == null) {
                    Debug.LogWarning("No attached GameController could be found.");
                } else {
                    instance.Start();
                }
            }
            return instance;
        }
    }

    public void PlayerDied()
    {
        SceneManager.LoadScene(0);
    }

    //Get Set

    //PlayerState ChangePlayerState {
    //    get {
    //        return playerState;
    //    }
    //    set {
    //        playerState = value;
    //    }
    //}
}
