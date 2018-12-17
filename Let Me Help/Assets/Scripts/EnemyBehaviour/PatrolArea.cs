using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Designates a patrol area and an array of patrol points for a monster
/// </summary>
public class PatrolArea
{
    public Transform topLeftCorner;
    public Transform botLeftCorner;
    public Transform topRightCorner;
    public Transform botRightCorner;

    public Transform[] patrolPoints;
}
