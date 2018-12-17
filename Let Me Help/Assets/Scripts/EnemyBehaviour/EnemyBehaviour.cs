using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    
    NavMeshAgent agent;

    public PatrolArea patrolArea;
    int patrolPointer;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(GameController.Instance.player.transform.position);
	}

    /// <summary>
    /// Enemy moves around through it's patrol zone
    /// </summary>
    void Patrol()
    {
        
    }

    /// <summary>
    /// Enemy chases player
    /// </summary>
    void Chase()
    {

    }

    /// <summary>
    /// Enemy chases player, unless the player has been out of Line Of Sight for a certain time, then it goes back to patrol
    /// </summary>
    void Search()
    {

    }
}
