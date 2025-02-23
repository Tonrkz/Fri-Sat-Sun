using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GodsOfferingManager : MonoBehaviour {
    public static GodsOfferingManager instance;

    [Header("References")]
    [SerializeField] PlayerGodOfferingHandler playerGodOfferingHandler; // Reference to the player god offering handler
    [SerializeField] GameObject Panel_godOffering; // Reference to the god offering panel
    [SerializeField] Image Image_BuyableOffering1; // Reference to the buyable offering 1 button
    [SerializeField] TextMeshProUGUI Text_BuyableOffering1; // Reference to the buyable offering 1 text
    [SerializeField] TextMeshProUGUI Text_CostOffering1; // Reference to the cost offering 1 text
    [SerializeField] Image Image_BuyableOffering2; // Reference to the buyable offering 2 button
    [SerializeField] TextMeshProUGUI Text_BuyableOffering2; // Reference to the buyable offering 2 text
    [SerializeField] TextMeshProUGUI Text_CostOffering2; // Reference to the cost offering 2 text
    [SerializeField] Image Image_OwnedOffering1; // Reference to the owned offering 1 image
    [SerializeField] Image Image_OwnedOffering2; // Reference to the owned offering 2 image
    [SerializeField] TextMeshProUGUI Text_Money; // Reference to the money text

    [Header("God Offerings")]
    public List<SO_GodOffering> godOfferings = new List<SO_GodOffering>(); // List of all god offerings
    List<SO_GodOffering> buyableOfferings = new List<SO_GodOffering>(); // List of all buyable offerings

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        RandomBuyableOfferings();
        UpdateOfferingsUI();
    }

    /// <summary>
    /// This method will be called when the player wants to buy an offering
    /// </summary>
    void RandomBuyableOfferings() {
        buyableOfferings.Clear();
        while (buyableOfferings.Count < 2) {
            SO_GodOffering randomGodOffering = godOfferings[Random.Range(0, godOfferings.Count)];
            if (!randomGodOffering.isActivated) {
                buyableOfferings.Add(randomGodOffering);
            }
        }
    }

    /// <summary>
    /// This method will be called when the player wants to buy an offering
    /// </summary>
    void UpdateOfferingsUI() {
        Text_Money.text = MoneyManager.instance.money.ToString(); // Update the money text

        if (buyableOfferings.Count > 0) {
            Image_BuyableOffering1.sprite = buyableOfferings[0].offeringSprite;
            Text_BuyableOffering1.text = buyableOfferings[0].offeringName;
            Text_CostOffering1.text = buyableOfferings[0].offeringCost.ToString();
            Image_BuyableOffering1.gameObject.SetActive(true);
        }
        else {
            Image_BuyableOffering1.gameObject.SetActive(false);
        }

        if (buyableOfferings.Count > 1) {
            Image_BuyableOffering2.sprite = buyableOfferings[1].offeringSprite;
            Text_BuyableOffering2.text = buyableOfferings[1].offeringName;
            Text_CostOffering2.text = buyableOfferings[1].offeringCost.ToString();
            Image_BuyableOffering2.gameObject.SetActive(true);
        }
        else {
            Image_BuyableOffering2.gameObject.SetActive(false);
        }

        if (playerGodOfferingHandler.godOffering_1 != null) {
            Image_OwnedOffering1.gameObject.SetActive(true);
            Image_OwnedOffering1.sprite = playerGodOfferingHandler.godOffering_1.offeringSprite;
        }
        else {
            Image_OwnedOffering1.gameObject.SetActive(false);
        }

        if (playerGodOfferingHandler.godOffering_2 != null) {
            Image_OwnedOffering2.gameObject.SetActive(true);
            Image_OwnedOffering2.sprite = playerGodOfferingHandler.godOffering_2.offeringSprite;
        }
        else {
            Image_OwnedOffering2.gameObject.SetActive(false);
        }
    }
}
