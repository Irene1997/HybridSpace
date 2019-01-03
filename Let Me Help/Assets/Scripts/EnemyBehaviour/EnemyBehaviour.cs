using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemyBehaviour : MonoBehaviour
{
    public enum CurrentState { Patrol, Chase, Search }

    NavMeshAgent agent;

    [SerializeField]
    Vector3 destination;
    public PatrolArea patrolArea;
    public int patrolPointer;
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
        currentState = CurrentState.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        //Decide a destination (if any) based on the current state of the enemy
        switch (currentState)
        {
            case CurrentState.Patrol: Patrol(); break;
            case CurrentState.Chase: Chase(); break;
            case CurrentState.Search: Search(); break;
            default: throw new System.ArgumentException("how the fuck did you even manage to give an enum a nonexisting value?");
        }

        //If the destination is new, set the agent destination and calculate a new path
        if (destination != agent.destination)
        {
            bool temp = agent.SetDestination(destination);
            if (!temp) { throw new System.ArgumentException("Something went wrong when setting the destination"); }
        }
    }

    /// <summary>
    /// Enemy moves around through its patrol zone
    /// </summary>
    void Patrol()
    {
        //If the player is in sight but the player is not in the Patrol zone, go Search anyway >:)
        if(InSight(transform, gameController.player.transform))
        {
            currentState = CurrentState.Search;
            Search();
            return;
        }

        //If the destination is not the same as the current patrol point, set it to that point.
        if (destination != patrolArea.patrolPoints[patrolPointer].position)
        {
            destination = patrolArea.patrolPoints[patrolPointer].position;
        }
        
        if (patrolArea == null)
        { Debug.Log("I don't have a patrol area :("); return; }

        //If close enough to patrol point, set patrol pointer to next patrol point in scene
        if (Vector3.Distance(transform.position,patrolArea.patrolPoints[patrolPointer].position) <= patrolCatchDistance)
        {
            patrolPointer++;

            //Set pointer back to the start if it's at the end of its path.
            if (patrolPointer >= patrolArea.patrolPoints.Length)
            {
                patrolPointer = 0;
            }
        }

        //if the player is in the patrol area of this enemy, go into chase state
        if(IsEntityInPatrolArea(gameController.player.transform))
        {
            Debug.Log("Chasin' Player");
            currentState = CurrentState.Chase;
        }
    }

    /// <summary>
    /// When the Player is in the Patrol Zone of an Enemy, that enemy continuously targets the player.
    /// </summary>
    void Chase()
    {
        ChasePlayer();

        //If the player is not in the Patrol Zone anymore, switch to the Search State.
        if (!IsEntityInPatrolArea(gameController.player.transform))
        {
            Debug.Log("Searching for player");
            currentState = CurrentState.Search;
        }
    }

    /// <summary>
    /// Enemy sets player as destination and plots a path.
    /// </summary>
    void ChasePlayer()
    {
        destination = gameController.player.transform.position;
    }

    /// <summary>
    /// Enemy chases player, unless the player has been out of Line Of Sight for a certain time, then it goes back to Patrol.
    /// </summary>
    void Search()
    {
        //If the player is in the patrol area, go back to the chase state
        if(IsEntityInPatrolArea(gameController.player.transform))
        {
            Debug.Log("Player is back in my patrol area");
            currentState = CurrentState.Chase;
            Chase();
            return;
        }

        //if the player is in sight, set them as the destination
        if(InSight(transform, gameController.player.transform))
        {
            searchTimer = 0;
            ChasePlayer();
        }
        else
        {
            searchTimer += Time.deltaTime;

            //If the search timer is up, go back to the patrol state
            if (searchTimer >= searchTime)
            {
                Debug.Log("Lost the player :(");

                currentState = CurrentState.Patrol;
                destination = patrolArea.patrolPoints[patrolPointer].position;

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

        float leftBound = Mathf.Min(patrolArea.topLeftCorner.position.x, patrolArea.botLeftCorner.position.x);
        float rightBound = Mathf.Max(patrolArea.topRightCorner.position.x, patrolArea.botRightCorner.position.x);
        float topBound = Mathf.Max(patrolArea.topLeftCorner.position.z, patrolArea.topRightCorner.position.z);
        float botBound = Mathf.Min(patrolArea.botLeftCorner.position.z, patrolArea.botRightCorner.position.z);

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
}
