﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{

    NavMeshAgent agent;

    public PatrolArea patrolArea;
    int patrolPointer;
    float searchTimer;
    RaycastHit hit;
    [SerializeField]
    int searchTime;
    [SerializeField]
    float patrolCatchDistance;
    [SerializeField]
    float sightAngle;
    GameController gameController;
    CurrentState currentState;

    // Use this for initialization
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        patrolPointer = 0;
        gameController = GameController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(GameController.Instance.player.transform.position);
        switch (currentState)
        {
            case CurrentState.Patrol: Patrol(); break;
            case CurrentState.Chase: Chase(); break;
            case CurrentState.Search: Search(); break;
            default: throw new System.ArgumentException("how the fuck did you even manage to give an enum a nonexisting value?");
        }
    }

    /// <summary>
    /// Enemy moves around through its patrol zone
    /// </summary>
    void Patrol()
    {
        if (patrolArea == null)
        { Debug.Log("I don't have a patrol area :("); return; }

        //If close enough to patrol point, set destination to next patrol point in scene
        if (agent.remainingDistance <= patrolCatchDistance)
        {
            patrolPointer++;

            if (patrolPointer >= patrolArea.patrolPoints.Length)
            {
                patrolPointer = 0;
            }

            agent.destination = patrolArea.patrolPoints[patrolPointer];
        }

        //if the player is in the patrol area of this enemy, go into chase state
        if(IsEntityInPatrolArea(gameController.player.transform))
        {
            currentState = CurrentState.Chase;
        }
    }

    /// <summary>
    /// Enemy Chase State behaviour
    /// </summary>
    void Chase()
    {
        ChasePlayer();

        if (!IsEntityInPatrolArea(transform))
        {
            currentState = CurrentState.Search;
        }
    }

    /// <summary>
    /// Enemy sets player as destination
    /// </summary>
    void ChasePlayer()
    {
        agent.destination = gameController.player.transform.position;
    }

    /// <summary>
    /// Enemy chases player, unless the player has been out of Line Of Sight for a certain time, then it goes back to patrol
    /// </summary>
    void Search()
    {
        //If the player is in the patrol area, go back to the chase state
        if(IsEntityInPatrolArea(gameController.player.transform))
        {
            currentState = CurrentState.Chase;
            Chase();
            return;
        }

        //if the player is in sight, set them as the
        if(InSight(transform, gameController.player.transform))
        {
            searchTimer = 0;
            ChasePlayer();
        }
        else
        {
            searchTimer += Time.deltaTime;

            if (searchTimer >= searchTime)
            {
                currentState = CurrentState.Patrol;
                Patrol();
                return;
            }
        }
    }

    /// <summary>
    /// Checks if an entity is in the patrol area of this enemy
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool IsEntityInPatrolArea(Transform entity)
    {
        Vector3 pos = entity.position;

        float leftBound = Mathf.Min(patrolArea.topLeftCorner.x, patrolArea.botLeftCorner.x);
        float rightBound = Mathf.Max(patrolArea.topRightCorner.x, patrolArea.botRightCorner.x);
        float topBound = Mathf.Max(patrolArea.topLeftCorner.z, patrolArea.topRightCorner.z);
        float botBound = Mathf.Min(patrolArea.botLeftCorner.z, patrolArea.botRightCorner.z);

        return (entity.position.x >= leftBound && entity.position.x <= rightBound && entity.position.z >= botBound && entity.position.z <= topBound) ;
    }

    /// <summary>
    /// Checks if an entity is in sight
    /// </summary>
    /// <param name="me">The entity looking</param>
    /// <param name="target">The entity we're looking for</param>
    /// <returns></returns>
    public bool InSight(Transform me, Transform target)
    {
        return InSight(me.position, me.rotation.y, target.position);
    }

    /// <summary>
    /// Checks if an entity is in sight
    /// </summary>
    /// <param name="position">Position of this entity</param>
    /// <param name="rotation">Rotation of this entity</param>
    /// <param name="target">Target Position</param>
    /// <returns>true if there's line of sight</returns>
    public bool InSight(Vector3 position, float rotation, Vector3 target)
    {
        float angle = Vector3.Angle(target - position, transform.forward);
        if (angle * 2 < sightAngle)
        {
            if (Physics.Raycast(position, Vector3.Normalize(target - position), out hit, Vector3.Distance(target, position)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public enum CurrentState { Patrol, Chase, Search}
}