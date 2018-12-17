using UnityEngine;
using System.Collections;

public class NavAgentScript : MonoBehaviour
{

    public enum Behaviour { Patrol, Chase, Wait };

    public int sightAngle, hardSightAngle, insaneSightAngle, looseOfSightAngle, hardLooseOfSightAngle, insaneLooseOfSightAngle, patrolPointer, patrolPointCatchDistance;
    public GameObject target;
    public float sightLoseTime, hardSightLoseTime, insaneSightLoseTime, chaseSpeed, patrolSpeed, eatTimePart, maxEatTime, patrolPointSkipChance;
    public Transform FirstPatrolPoint;
    public Transform SecondPatrolPoint;
    public Transform ThirdPatrolPoint;
    public Transform FourthPatrolPoint;
    public float spookyTimer = 0;

    public AudioClip spookyNoSeeYou;
    public AudioClip spookyLostYou;
    /*public AudioClip PatrolClip;
    public AudioClip ChaseClip;
    */

    Behaviour behaviour;
    UnityEngine.AI.NavMeshAgent agent;
    Transform itself, targetPosition;
    RaycastHit hit;
    Vector3 destination, resetPosition;
    Quaternion resetRotation;
    float timer, lastSeen;
    float eatTime;
    bool waiting;

    /*bool nowPlaying = false;
    AudioSource a = new AudioSource();
    AudioSource b = new AudioSource();
    */

    // Use this for initialization
    void Start()
    {
        patrolPointer = Random.Range(1, 4);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        itself = GetComponent<Transform>();
        targetPosition = target.GetComponent<Transform>();
        destination = targetPosition.position;
        resetPosition = transform.position;
        resetRotation = transform.rotation;
        eatTime = 0;
        switch (GameController.Difficulty)
        {
            default:
            case 0:
                //normal
                break;
            case 1:
                //hard
                sightAngle = hardSightAngle;
                looseOfSightAngle = hardLooseOfSightAngle;
                sightLoseTime = hardSightLoseTime;
                break;
            case 2:
                //insane
                sightAngle = insaneSightAngle;
                looseOfSightAngle = insaneLooseOfSightAngle;
                sightLoseTime = insaneSightLoseTime;
                break;
        }
        /*
                a.clip = PatrolClip;
                b.clip = ChaseClip;
                a.Play();
                */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameController.player)
        {
            GameController.player.GetComponent<livesScript>().Damage();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spookyTimer > 50)
        {
            // AudioSource.PlayClipAtPoint(spookyNoSeeYou, targetPosition.position, 0.6f);
            if (!GameController.player.GetComponent<AudioSource>().isPlaying)
                GameController.player.GetComponent<AudioSource>().Play();
            spookyTimer = 0;
        }
        if (eatTime > 0)
        {
            eatTime -= Time.deltaTime;
        }

        if (Time.time >= timer && waiting)
        {
            behaviour = Behaviour.Patrol;
            waiting = false;
        }

        if (behaviour == Behaviour.Patrol)
        {
            spookyTimer += Time.deltaTime;
            if (InSight(itself.position, itself.rotation.y, targetPosition.position, sightAngle))
            {
                behaviour = Behaviour.Chase;
                //  PlayChasingSong(true);
            }
            else
            {
                agent.speed = patrolSpeed;
                switch (patrolPointer)
                {
                    default:
                    case 1:
                        destination = FirstPatrolPoint.position;
                        if (Vector3.Distance(destination, itself.position) <= patrolPointCatchDistance)
                        {
                            if (Random.Range(0, patrolPointSkipChance) < 1)
                            {
                                patrolPointer = 3;
                            }
                            else
                            {
                                patrolPointer = 2;
                            }
                        }
                        break;
                    case 2:
                        destination = SecondPatrolPoint.position;
                        if (Vector3.Distance(destination, itself.position) <= patrolPointCatchDistance)
                        {
                            if (Random.Range(0, patrolPointSkipChance) < 1)
                            {
                                patrolPointer = 4;
                            }
                            else
                            {
                                patrolPointer = 3;
                            }
                        }
                        break;
                    case 3:
                        destination = ThirdPatrolPoint.position;
                        if (Vector3.Distance(destination, itself.position) <= patrolPointCatchDistance)
                        {
                            if (Random.Range(0, patrolPointSkipChance) < 1)
                            {
                                patrolPointer = 1;
                            }
                            else
                            {
                                patrolPointer = 4;
                            }
                        }
                        break;
                    case 4:
                        destination = FourthPatrolPoint.position;
                        if (Vector3.Distance(destination, itself.position) <= patrolPointCatchDistance)
                        {
                            if (Random.Range(0, patrolPointSkipChance) < 1)
                            {
                                patrolPointer = 2;
                            }
                            else
                            {
                                patrolPointer = 1;
                            }
                        }
                        break;
                }
                // PlayChasingSong(false);
            }
        }
        else if (behaviour == Behaviour.Chase)
        {  //chase
            spookyTimer = 0;
            agent.speed = chaseSpeed;
            if (InSight(itself.position, itself.rotation.y, targetPosition.position, looseOfSightAngle))
            {
                lastSeen = Time.time;
            }
            if (lastSeen + sightLoseTime < Time.time)
            {
                behaviour = Behaviour.Patrol;
                AudioSource.PlayClipAtPoint(spookyLostYou, this.transform.position, 0.6f);
            }
            else
            {
                destination = targetPosition.position;
            }
        }
        else if (behaviour == Behaviour.Wait)
        {
            agent.speed = 0;
        }
        agent.SetDestination(destination);
    }

    public void Eat()
    {
        behaviour = Behaviour.Wait;
        eatTime = (maxEatTime - eatTime) * eatTimePart + eatTime;
        timer = Time.time + eatTime;
        waiting = true;
    }

    public void Reset()
    {
        itself.position = resetPosition;
        itself.rotation = resetRotation;
        patrolPointer = Random.Range(1, 4);
    }

    public bool InSight(Vector3 position, float rotation, Vector3 target, float sightAngle)
    {
        float angle = Vector3.Angle(target - position, transform.forward);
        if (angle < sightAngle / 2 && target.magnitude > 10)
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

    /*public void PlayChasingSong(bool chasing = false, bool force = false) {
        if(chasing==nowPlaying || force == true) {
            nowPlaying = chasing;
            if (chasing) {
                b.Play();
                a.Pause();
            } else {
                a.UnPause();
                b.Stop();
            }

        }
    }
    */
}
