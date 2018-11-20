using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;

    public GameObject playerObject;
    public Vector3 playerPosition;
    public GameObject[] enemies;

    // Use this for initialization
    void Start() {
        instance = this;
        playerPosition = playerObject.transform.position;
        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<EnemyController>().Initialize();
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
