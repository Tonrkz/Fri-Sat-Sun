using System.Collections.Generic;
using Unity;
using UnityEngine;

public class MageTowerActivateRadiusScript : MonoBehaviour {
    public Enum_MageTowerSelectedPower power;
    [Range(0, 1)] public float slowDownPercentage;
    [Range(0, 1)] public float ATKDownPercentage;
    [Range(0, 1)] public float ATKSpeedUpPercentage;

    public List<Collider> collided = new List<Collider>();

    private void OnTriggerEnter(Collider other) {
        collided.Add(other);
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Demon")) {
            switch (power) {
                case Enum_MageTowerSelectedPower.Slow:
                    StartCoroutine(other.GetComponent<DemonsMovement>().SlowWalkSpeed(slowDownPercentage));
                    break;
                case Enum_MageTowerSelectedPower.ATKDown:
                    StartCoroutine(other.GetComponent<IAttackables>().AttackDown(ATKDownPercentage));
                    break;
            }
        }
        else if (other.CompareTag("Tower")) {
            switch (power) {
                case Enum_MageTowerSelectedPower.ATKSpeedUp:
                    StartCoroutine(other.GetComponent<IActivatables>().FireRateUp(ATKSpeedUpPercentage));
                    break;
                case Enum_MageTowerSelectedPower.Reveal:
                    StartCoroutine(other.GetComponent<IActivatables>().SetCanSeePhantom(true));
                    break;
            }
        }
        else if (other.CompareTag("Soldier")) {
            switch (power) {
                case Enum_MageTowerSelectedPower.Reveal:
                    StartCoroutine(other.GetComponent<ISoldiers>().SetCanSeePhantom(true));
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Demon")) {
            collided.Remove(other);
            Debug.Log("MageTowerActivateRadiusScript: OnTriggerExit: " + other.name);
            switch (power) {
                case Enum_MageTowerSelectedPower.Slow:
                    StartCoroutine(other.GetComponent<DemonsMovement>().ResetWalkSpeed());
                    break;
                case Enum_MageTowerSelectedPower.ATKDown:
                    StartCoroutine(other.GetComponent<IAttackables>().ResetAttack());
                    break;
            }
        }
        else if (other.CompareTag("Soldier")) {
            collided.Remove(other);
            switch (power) {
                case Enum_MageTowerSelectedPower.Reveal:
                    StartCoroutine(other.GetComponent<ISoldiers>().ResetCanSeePhantom());
                    break;
            }
        }
    }

    public void ResetCollided() {
        foreach (var item in collided) {
            try {
                switch (power) {
                    case Enum_MageTowerSelectedPower.Slow:
                        StartCoroutine(item.GetComponent<DemonsMovement>().ResetWalkSpeed());
                        break;
                    case Enum_MageTowerSelectedPower.ATKDown:
                        StartCoroutine(item.GetComponent<IAttackables>().ResetAttack());
                        break;
                    case Enum_MageTowerSelectedPower.ATKSpeedUp:
                        StartCoroutine(item.GetComponent<IActivatables>().ResetFireRate());
                        break;
                    case Enum_MageTowerSelectedPower.Reveal:
                        if (item.CompareTag("Tower")) {
                            StartCoroutine(item.GetComponent<IActivatables>().ResetCanSeePhantom());
                        }
                        else if (item.CompareTag("Soldier")) {
                            StartCoroutine(item.GetComponent<ISoldiers>().ResetCanSeePhantom());
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception) {
                continue;
            }
        }
        collided.Clear();
    }
}
