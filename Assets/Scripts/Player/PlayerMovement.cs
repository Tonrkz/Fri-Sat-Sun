using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement instance;

    [Header("Attributes")]
    internal Single moveCooldown = 0.15f; // Time between each movement
    Single holdCooldown = 0.1f; // Threadhold for holding the key
    Byte movePerInput = 2;
    Single calcMoveCooldown = 0f;
    Single calcHoldCooldown = 0f;

    void Awake() {
        instance = this;
    }

    void Start() {
        calcMoveCooldown = moveCooldown;
        calcHoldCooldown = holdCooldown;
    }

    void Update() {
        if (Time.timeScale == 0) {
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            calcHoldCooldown -= Time.deltaTime;
            if (calcHoldCooldown <= 0) {
                calcMoveCooldown -= Time.deltaTime;
                if (calcMoveCooldown <= 0) {
                    transform.position += new Vector3(0, 0, movePerInput);
                    calcMoveCooldown = moveCooldown;
                }
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                transform.position += new Vector3(0, 0, movePerInput);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            calcHoldCooldown -= Time.deltaTime;
            if (calcHoldCooldown <= 0) {
                calcMoveCooldown -= Time.deltaTime;
                if (calcMoveCooldown <= 0) {
                    transform.position += new Vector3(-movePerInput, 0, 0);
                    calcMoveCooldown = moveCooldown;
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                transform.position += new Vector3(-movePerInput, 0, 0);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            calcHoldCooldown -= Time.deltaTime;
            if (calcHoldCooldown <= 0) {
                calcMoveCooldown -= Time.deltaTime;
                if (calcMoveCooldown <= 0) {
                    transform.position += new Vector3(0, 0, -movePerInput);
                    calcMoveCooldown = moveCooldown;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                transform.position += new Vector3(0, 0, -movePerInput);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            calcHoldCooldown -= Time.deltaTime;
            if (calcHoldCooldown <= 0) {
                calcMoveCooldown -= Time.deltaTime;
                if (calcMoveCooldown <= 0) {
                    transform.position += new Vector3(movePerInput, 0, 0);
                    calcMoveCooldown = moveCooldown;
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                transform.position += new Vector3(movePerInput, 0, 0);
            }
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow)) {
            calcMoveCooldown = moveCooldown;
            calcHoldCooldown = holdCooldown;
        }
    }

    /// <summary>
    /// Get the current position of the player
    /// </summary>
    /// <returns>Player's world position</returns>
    public Vector3 GetCurrentPosition() {
        Debug.Log($"Current Position: {transform.position}");
        return transform.position;
    }
}