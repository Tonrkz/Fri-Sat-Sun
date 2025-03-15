using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerGodOfferingHandler : MonoBehaviour {
    [Header("God Offerings")]
    public SO_GodOffering godOffering_1;
    public SO_GodOffering godOffering_2;

    public void AssignGodOffering(SO_GodOffering godOffering) {
        if (godOffering_1 == null) {
            godOffering_1 = godOffering;
            godOffering_1.OnAssigned();
        }
        else if (godOffering_2 == null) {
            godOffering_2 = godOffering;
            godOffering_2.OnAssigned();
        }
    }

    public void UnassignGodOffering(SO_GodOffering godOffering) {
        if (godOffering_1 == godOffering) {
            godOffering_1.OnUnassigned();
            godOffering_1 = null;
        }
        else if (godOffering_2 == godOffering) {
            godOffering_2.OnUnassigned();
            godOffering_2 = null;
        }
    }
}
