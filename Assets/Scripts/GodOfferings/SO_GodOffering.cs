using System;
using UnityEngine;

[Serializable]
public abstract class SO_GodOffering : ScriptableObject {
    [Header("God Offering Information")]
    public string godOfferingID; // This will be used to identify the offering
    public Sprite offeringSprite; // This will be used to display the offering sprite
    public string offeringName; // This will be used to display the offering name
    public string offeringDescription; // This will be used to display the offering description
    public int offeringCost; // This will be used to display the offering cost

    /// <summary>
    /// This method will be called when the offering is assigned to the player
    /// </summary>
    public abstract void OnAssigned();

    /// <summary>
    /// This method will be called when the offering is unassigned from the player
    /// </summary>
    public abstract void OnUnassigned();
}
