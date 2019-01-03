using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedPositionsHandler : MonoBehaviour {

    LedPosition[] ledPositions;
    LedPosition playerLed;
    LedPosition[] enemyLeds;

	// Use this for initialization
	void Start () {
        ledPositions = GetComponentsInChildren<LedPosition>();
        enemyLeds = new LedPosition[GameController.Instance.enemyControllers.Length];
	}
	
	// Update is called once per frame
	void Update () {
        if (GameController.Instance.arduinoHandler.namedArduinos.ContainsKey("MazeArduino")) {
            LedPosition closestLed = ClosestToPosition(GameController.Instance.player.transform.position);
            if (closestLed != playerLed) {
                playerLed = closestLed;
                GameController.Instance.arduinoHandler.namedArduinos["MazeArduino"].Write("P " + closestLed.col + " " + closestLed.row);
            }
            for (int i = 0; i < enemyLeds.Length; ++i) {
                closestLed = ClosestToPosition(GameController.Instance.enemies.GetComponentsInChildren<Transform>()[i].position);
                if (closestLed != enemyLeds[i]) {
                    enemyLeds[i] = closestLed;
                    GameController.Instance.arduinoHandler.namedArduinos["MazeArduino"].Write("M " + i + " " + closestLed.col + " " + closestLed.row);
                }
            }
        }
	}

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
