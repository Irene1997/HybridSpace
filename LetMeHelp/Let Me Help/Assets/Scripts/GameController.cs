using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    static GameController instance;

    public GameObject player;
    public GameObject enemies;

    [SerializeField]
    IList<EnemyController> enemyList;

    // Use this for initialization
    void Start() {
        instance = this;
        enemyList = new List<EnemyController>();
        foreach (EnemyController enemy in enemies.GetComponentsInChildren<EnemyController>()) {
            enemyList.Add(enemy);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public static GameController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameController>();
                if (instance == null) { Debug.LogWarning("No attached GameController could be found."); }
            }
            return instance;
        }
    }
}
