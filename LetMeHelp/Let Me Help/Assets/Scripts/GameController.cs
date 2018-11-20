using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public GameObject playerObject;
    public Vector3 playerPosition;
    public GameObject enemies;

    IList<EnemyController> enemyList;

    // Use this for initialization
    void Start() {
        instance = this;
        playerPosition = playerObject.transform.position;
        enemyList = new List<EnemyController>();
        foreach (EnemyController enemy in enemies.GetComponentsInChildren<EnemyController>()) {
            enemyList.Add(enemy);
            enemy.Initialize();
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
