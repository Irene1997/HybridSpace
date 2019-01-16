using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LedPositionsHandler : MonoBehaviour
{
    // Stores all known LED positions
    //LedPosition[] ledPositions;
    // Stores the corresponding LED position of the player and enemies
    [SerializeField]
    LedPosition playerLed;
    [SerializeField]
    LedPosition[] enemyLeds;

    LedZone[] ledZones;

    // Initialization
    void Start()
    {
        //ledPositions = GetComponentsInChildren<LedPosition>();
        ledZones = GetComponentsInChildren<LedZone>();
        enemyLeds = new LedPosition[GameController.Instance.enemyControllers.Length];

        ledZones.OrderBy(l => l.priority);
        //Array.Sort(ledZones);
    }

    // Update
    void Update()
    {
        // Finds the closest LED to the player
        LedPosition closestLed;
        try
        {
            closestLed = FindLed(GameController.Instance.player.transform.position);
            if (closestLed != null)
            {
                if (closestLed != playerLed)
                {
                    playerLed = closestLed;
                    GameController.Instance.arduinoHandler.WritePlayerPosition(closestLed.col, closestLed.row);
                }
            }
        }
        catch (Exception e) { }

        // Finds the closest LED for each enemy
        for (int i = 0; i < enemyLeds.Length; i++)
        {
            try
            {
                closestLed = FindLed(GameController.Instance.enemyControllers[i].transform.position);
                if (closestLed != null)
                {
                    if (closestLed != enemyLeds[i])
                    {
                        enemyLeds[i] = closestLed;
                        GameController.Instance.arduinoHandler.WriteMonsterPosition(i, closestLed.col, closestLed.row);
                    }
                }
            }
            catch (Exception e) { }
        }

    }

    //// Finds the closest LED to a position
    //LedPosition ClosestToPosition(Vector3 pos) {
    //    LedPosition closestLed = null;
    //    float closetDistanceSquared = float.PositiveInfinity;
    //    foreach (LedPosition ledPosition in ledPositions) {
    //        if ((ledPosition.transform.position - pos).sqrMagnitude < closetDistanceSquared) {
    //            closestLed = ledPosition;
    //            closetDistanceSquared = (ledPosition.transform.position - pos).sqrMagnitude;
    //        }
    //    }
    //    return closestLed;
    //}

    /// <summary>
    /// Get the position of the led corresponding to a position of an entity
    /// </summary>
    /// <param name="pos">Position of the entity</param>
    /// <returns>Led corresponding to position of entity</returns>
    LedPosition FindLed(Vector3 pos)
    {
        LedPosition led = null;
        for (int i = 0; i < ledZones.Length; i++)
        {
            led = ledZones[i].IsEntityInZone(pos);

            if (led != null) { i = ledZones.Length; }
        }

        return led;
        //throw new System.ArgumentOutOfRangeException("Entity not in any ledzone. Entity position:" + pos);
    }

    public void SendAllCurrentStates()
    {
        GameController.Instance.arduinoHandler.WritePlayerPosition(playerLed.col, playerLed.row);
        for (int i = 0; i < enemyLeds.Length; ++i)
        {
            GameController.Instance.arduinoHandler.WriteMonsterPosition(i, enemyLeds[i].col, enemyLeds[i].row);
        }
    }
}
