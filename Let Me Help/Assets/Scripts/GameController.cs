using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    // The instance of this class
    static GameController instance;

    [SerializeField]
    Canvas canvasOfDeath;
    [SerializeField]
    Canvas canvasOfWinner;

    [SerializeField] [Range(0, 50)]
    private float winDistance;
    //public enum PlayerState { Alive, Dead };

    // Instances that can be accesed from every class via the Instance of this class
    public GameObject player;
    public PlayerController playerScript;
    //public PlayerState playerState;

    public GameObject enemies;
    public EnemyBehaviour[] enemyControllers;

    public GameObject doors;
    public DoorScript[] doorScripts;


    public ArduinoHandler arduinoHandler;

    [SerializeField]
    private Transform endPoint;

    // Use this for initialization
    void Start() {

        // Sets all instances
        instance = this;
        if (player == null) {
            player = GameObject.Find("Player");
            //playerState = PlayerState.Alive;
            if (player == null) { Debug.LogWarning("No player could be found."); }
        }
        if (playerScript == null) {
            playerScript = player.GetComponent<PlayerController>();
        }
        playerScript = player.GetComponent<PlayerController>();
        if (enemies == null) {
            enemies = GameObject.Find("Enemies");
            if (enemies == null) { Debug.LogWarning("No enemies could be found."); }
        }
        if (enemyControllers == null || enemyControllers.Length == 0) {
        enemyControllers = enemies.GetComponentsInChildren<EnemyBehaviour>();
        }
        if (doors == null) {
            doors = GameObject.Find("Doors");
            if (doors == null) { Debug.LogWarning("No doors could be found."); }
        }
        if (doorScripts == null || doorScripts.Length == 0) {
            doorScripts = doors.GetComponentsInChildren<DoorScript>();
        }
        arduinoHandler = GetComponent<ArduinoHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        DidPlayerWin();
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
        canvasOfDeath.gameObject.SetActive(true);
    }

    /// <summary>
    /// Checks if the player is a winner, and if so, activates the winning screen ^^
    /// </summary>
    public void DidPlayerWin()
    {
        if(Vector3.Distance(endPoint.position,player.transform.position) < winDistance)
        {
            canvasOfWinner.gameObject.SetActive(true);
        }
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
