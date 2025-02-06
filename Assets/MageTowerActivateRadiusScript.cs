using System.Collections.Generic;
using Unity;
using UnityEngine;

public class MageTowerActivateRadiusScript : MonoBehaviour {
    public Enum_MageTowerSelectedPower power;
    [Range(0, 1)] public float slowDownPercentage = 0.25f;
    [Range(0, 1)] public float ATKDownPercentage = 0.25f;
    [Range(0, 1)] public float ATKSpeedUpPercentage = 0.25f;

    public List<Collider> collidedDemons = new List<Collider>();

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Demon")) {
            collidedDemons.Add(other);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Demon")) {
            switch (power) {
                case Enum_MageTowerSelectedPower.Slow:
                    other.GetComponent<DemonsMovement>().SlowWalkSpeed(slowDownPercentage);
                    break;
                case Enum_MageTowerSelectedPower.ATKDown:
                    break;
                case Enum_MageTowerSelectedPower.ATKSpeedUp:
                    break;
                case Enum_MageTowerSelectedPower.Reveal:
                    break;
                default:
                    break;
            }
        }
    }

    public void ResetCollidedDemon() {
        foreach (var demon in collidedDemons) {
            switch (power) {
                case Enum_MageTowerSelectedPower.Slow:
                    demon.GetComponent<DemonsMovement>().ResetWalkSpeed();
                    break;
                case Enum_MageTowerSelectedPower.ATKDown:
                    break;
                case Enum_MageTowerSelectedPower.ATKSpeedUp:
                    break;
                case Enum_MageTowerSelectedPower.Reveal:
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Demon")) {
            collidedDemons.Remove(other);
            Debug.Log("MageTowerActivateRadiusScript: OnTriggerExit: " + other.name);
            switch (power) {
                case Enum_MageTowerSelectedPower.Slow:
                    other.GetComponent<DemonsMovement>().ResetWalkSpeed();
                    break;
                case Enum_MageTowerSelectedPower.ATKDown:
                    break;
                case Enum_MageTowerSelectedPower.ATKSpeedUp:
                    break;
                case Enum_MageTowerSelectedPower.Reveal:
                    break;
                default:
                    break;
            }
        }
    }
}
