using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedPositionsHandler : MonoBehaviour {
    // Stores all known LED positions
    LedPosition[] ledPositions;
    // Stores the corresponding LED position of the player and enemies
    LedPosition playerLed;
    LedPosition[] enemyLeds;

    // Initialization
    void Start() {
        ledPositions = GetComponentsInChildren<LedPosition>();
        enemyLeds = new LedPosition[GameController.Instance.enemyControllers.Length];
    }

    // Update
    void Update() {
        // Finds the closest LED to the player
        LedPosition closestLed = ClosestToPosition(GameController.Instance.player.transform.position);
        if (closestLed != playerLed) {
            playerLed = closestLed;
            GameController.Instance.arduinoHandler.WritePlayerPosition(closestLed.col, closestLed.row);
        }
        // Finds the closest LED for each enemy
        for (int i = 0; i < enemyLeds.Length; ++i) {
            closestLed = ClosestToPosition(GameController.Instance.enemies.GetComponentsInChildren<Transform>()[i].position);
            if (closestLed != enemyLeds[i]) {
                enemyLeds[i] = closestLed;
                GameController.Instance.arduinoHandler.WriteMonsterPosition(i, closestLed.col, closestLed.row);
            }
        }

    }

    // Finds the closest LED to a position
    LedPosition ClosestToPosition(Vector3 pos) {
        LedPosition closestLed = null;
        float closetDistanceSquared = float.PositiveInfinity;
        foreach (LedPosition ledPosition in ledPositions) {
            if ((ledPosition.transform.position - pos).sqrMagnitude < closetDistanceSquared) {
                closestLed = ledPosition;
                closetDistanceSquared = (ledPosition.transform.position - pos).sqrMagnitude;
            }
        }
        return closestLed;
    }
}
