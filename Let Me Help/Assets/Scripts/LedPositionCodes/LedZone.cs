using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a zone in the maze linked to a led. Multiple zones can link to the same led.
/// </summary>
public class LedZone : MonoBehaviour, IComparable
{
    //public Transform leftBorder;
    //public Transform rightBorder;
    //public Transform topBorder;
    //public Transform bottomBorder;

    public LedPosition led;
    public int priority;

    //private float leftBound, rightBound, topBound, bottomBound;

    BoxCollider collider;

    void Start()
    {
        collider = GetComponent<BoxCollider>();

        //float leftBound = leftBorder.position.x;
        //float rightBound = rightBorder.position.x;
        //float topBound = topBorder.position.y;
        //float bottomBound = bottomBorder.position.y;

        ////If the bounds of the zone are placed incorrectly, throw an exception.
        //if(leftBound >= rightBound)
        //{
        //    throw new System.ArgumentException("Left bound of Led Zone cannot be to the right of Right bound of Led Zone");
        //}
        //if(bottomBound >= topBound)
        //{
        //    throw new System.ArgumentException("Bottom bound of Led Zone cannot be higher than of Top bound of Led Zone");
        //}

    }

    /// <summary>
    /// Checks whether an entity is in this zone. If it is, returns the Led Position, returns Null otherwise
    /// </summary>
    /// <param name="objectPos">The position of the entity</param>
    /// <returns></returns>
    public LedPosition IsEntityInZone(Vector3 objectPos)
    {
        if (!collider.bounds.Contains(objectPos)) { return null; }
        return led;
    }

    /// <summary>
    /// Checks whether an entity is in this zone. If it is, returns the Led Position, returns Null otherwise
    /// </summary>
    /// <param name="entity">The entity</param>
    /// <returns></returns>
    public LedPosition IsEntityInZone(Transform entity)
    {
        return IsEntityInZone(entity.position);
    }

    public int CompareTo(object obj)
    {
        return priority.CompareTo(obj);
    }

    //public static bool operator >(LedZone a, LedZone b)
    //{
    //    return (a.priority > b.priority);
    //}

    //public static bool operator <(LedZone a, LedZone b)
    //{
    //    return (a.priority < b.priority);
    //}

    //public static bool operator ==(LedZone a, LedZone b)
    //{
    //    return (a.priority == b.priority);
    //}

    //public static bool operator !=(LedZone a, LedZone b)
    //{
    //    return (a.priority != b.priority);
    //}
}
