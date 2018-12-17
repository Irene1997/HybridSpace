using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    
    NavMeshAgent agent;

    public Transform[] patrolPoints;
    int patrolPointer;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(GameController.Instance.player.transform.position);
	}

    void Patrol()
    {

    }

    void Chase()
    {

    }

    void Search()
    {

    }
}
