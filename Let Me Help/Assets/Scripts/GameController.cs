using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    // The instance of this class
    static GameController instance;

    [SerializeField]
    Canvas canvasOfDeath;

    //public enum PlayerState { Alive, Dead };

    // Instances that can be accesed from every class via the Instance of this class
    public GameObject player;
    //public PlayerState playerState;

    public GameObject enemies;
    public EnemyBehaviour[] enemyControllers;

    public DoorScript[] doors;
    public PlayerController playerScript;

    public ArduinoHandler arduinoHandler;

    // Use this for initialization
    void Start() {

        // Sets all instances
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

        arduinoHandler = GetComponent<ArduinoHandler>();
    }

    // Update is called once per frame
    void Update() {

    }

    // Tries to return the instance of this class
    public static GameController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameController>();
                if (instance == null) {
                         SceneManager.LoadScene(0);
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
        Debug.Log("DEATH AND DESTRUCTION");
        //canvasOfDeath.enabled = true;
        canvasOfDeath.gameObject.SetActive(true);
    }
}
/*                  *\
     |\_  /\  _/|     
     |{}\/{}\/{}|     
     |__________|     
     /##########\     
    /#(.)#/\#(.)#\    
   /#####/__\#####\   
  |#######\/#######|  
 /|################|\ 
/ |####/      \####| \
| |###|        |###| |
| |###|        |###| |
\_\###|________|###/_/
   /_\          /_\   
Jaap de Keizerspinguin
\*                  */
