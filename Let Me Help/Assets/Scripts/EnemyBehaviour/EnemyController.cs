using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject patrolAreasParent;
    public List<PatrolArea> patrolAreas;
    EnemyBehaviour[] enemies;
    float patrolTimer;
    [SerializeField]
    private int patrolTimerMin;
    [SerializeField]
    private int patrolTimerMax;
    
    // Use this for initialization
    void Start()
    {
        patrolAreas = new List<PatrolArea>(patrolAreasParent.GetComponentsInChildren<PatrolArea>());
        enemies = GetComponentsInChildren<EnemyBehaviour>();
        RedistributePatrolPaths();
    }

    // Update is called once per frame
    void Update()
    {
        patrolTimer -= Time.deltaTime;

        if(patrolTimer<0) { RedistributePatrolPaths(); patrolTimer = Random.Range(patrolTimerMin, patrolTimerMax); }
    }

    /// <summary>
    /// Gives enemies new patrol paths, by randomly shuffling the patrol areas then assigning them to enemies
    /// </summary>
    void RedistributePatrolPaths()
    {
        //Make sure there's never less patrol areas than enemies. 
        //If there's more patrol areas than enemies, some patrol areas will be unused, which is fine.
        if (patrolAreas.Count < enemies.Length)
        {
            throw new System.ArgumentException("There can never be less patrol paths than enemies");
        }

        ListShuffle(patrolAreas);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].patrolArea = patrolAreas[i];
            enemies[i].patrolPointer = 0;
        }
    }

    /// <summary>
    /// Shuffles a list
    /// </summary>
    /// <typeparam name="T">Type of the list to shuffle</typeparam>
    /// <param name="toShuffle">List that has to be shuffled</param>
    void ListShuffle<T>(List<T> toShuffle)
    {
        int n = toShuffle.Count;
        while (n > 1)
        {
            //Cool algorithm based on the Fisher-Yates Shuffle. Source: https://stackoverflow.com/questions/273313/randomize-a-listt
            n--;
            int k = Random.Range(0, n + 1);
            T value = toShuffle[k];
            toShuffle[k] = toShuffle[n];
            toShuffle[n] = value;
        }
    }
}