using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Designates a patrol area and an array of patrol points for a monster
/// </summary>
[System.Serializable]
public class PatrolArea
{
    public Vector3 topLeftCorner; //z max x min
    public Vector3 botLeftCorner; //z min x min
    public Vector3 topRightCorner; //z max x max
    public Vector3 botRightCorner; //z min x max

    public Vector3[] patrolPoints;
}
