using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Designates a patrol area and an array of patrol points for a monster
/// </summary>
[System.Serializable]
public class PatrolArea : MonoBehaviour
{
    public Transform topLeftCorner; //z max x min
    public Transform botLeftCorner; //z min x min
    public Transform topRightCorner; //z max x max
    public Transform botRightCorner; //z min x max

    public Transform[] patrolPoints;
}
