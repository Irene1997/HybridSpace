using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    Vector3 playerPosition;
    NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        playerPosition = GameController.Instance.player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(playerPosition);
	}
}
