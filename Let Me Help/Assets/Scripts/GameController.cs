using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    static GameController instance;

    public GameObject player;
    public GameObject enemies;

    [SerializeField]
    EnemyBehaviour[] enemyControllers;

    public DoorScript[] doors;

    // Use this for initialization
    void Start() {
        instance = this;
        if (player == null) {
            player = GameObject.Find("PLayer");
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
}
