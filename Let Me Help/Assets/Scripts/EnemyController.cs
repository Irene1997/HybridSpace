using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Transform[]> patrolPaths;
    EnemyBehaviour[] enemies;
    float patrolTimer;
    [SerializeField]
    private int patrolTimerMin;
    [SerializeField]
    private int patrolTimerMax;
    
    // Use this for initialization
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        patrolTimer -= Time.deltaTime;

        if(patrolTimer<0) { RedistributePatrolPaths(); patrolTimer = Random.Range(patrolTimerMin, patrolTimerMax); }
    }

    void RedistributePatrolPaths()
    {
        if (patrolPaths.Count < enemies.Length)
        {
            throw new System.ArgumentException("There can never be less patrol paths than enemies");
        }

        ListShuffle(patrolPaths);

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].patrolPoints = patrolPaths[i];
        }
    }

    /// <summary>
    /// Something something shuffles lists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toShuffle">List that has to be shuffled</param>
    void ListShuffle<T>(List<T> toShuffle)
    {
        int n = toShuffle.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = toShuffle[k];
            toShuffle[k] = toShuffle[n];
            toShuffle[n] = value;
        }
    }
}