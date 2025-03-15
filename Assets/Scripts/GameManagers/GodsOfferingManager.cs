using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;

public class GodsOfferingManager : MonoBehaviour {
    public static GodsOfferingManager instance;

    [Header("References")]
    [SerializeField] PlayerGodOfferingHandler playerGodOfferingHandler; // Reference to the player god offering handler

    [Header("UI References")]
    // UI References
    [SerializeField] GameObject Panel_GodOffering; // Reference to the god offering panel
    [SerializeField] TextMeshProUGUI Text_Money; // Reference to the money text

    [Header("Buyable Offerings 1 UI References")]
    // Buyable Offerings 1
    [SerializeField] Button Button_BuyableGodOffering1; // Reference to the buyable offering 1 button
    [SerializeField] Image Image_BuyableOffering1; // Reference to the buyable offering 1 button
    [SerializeField] TextMeshProUGUI Text_BuyableOffering1Name; // Reference to the buyable offering 1 name
    [SerializeField] TextMeshProUGUI Text_BuyableOffering1Description; // Reference to the buyable offering 1 description
    [SerializeField] TextMeshProUGUI Text_CostOffering1; // Reference to the cost offering 1 text

    [Header("Buyable Offerings 2 UI References")]
    // Buyable Offerings 2
    [SerializeField] Button Button_BuyableGodOffering2; // Reference to the buyable offering 2 button
    [SerializeField] Image Image_BuyableOffering2; // Reference to the buyable offering 2 button
    [SerializeField] TextMeshProUGUI Text_BuyableOffering2Name; // Reference to the buyable offering 2 name
    [SerializeField] TextMeshProUGUI Text_BuyableOffering2Description; // Reference to the buyable offering 2 description
    [SerializeField] TextMeshProUGUI Text_CostOffering2; // Reference to the cost offering 2 text

    [Header("Owned Offerings 1 UI References")]
    // Owned Offerings 1
    [SerializeField] GameObject OwnedOffering1; // Reference to the owned offering 1 gameobject
    [SerializeField] Image Image_OwnedOffering1; // Reference to the owned offering 1 image
    [SerializeField] TextMeshProUGUI Text_OwnedOffering1Name; // Reference to the owned offering 1 name
    [SerializeField] TextMeshProUGUI Text_OwnedOffering1Description; // Reference to the owned offering 1 description
    [SerializeField] Button Button_UnassignOffering1; // Reference to the unassign offering 1 button

    [Header("Owned Offerings 2 UI References")]
    // Owned Offerings 2
    [SerializeField] GameObject OwnedOffering2; // Reference to the owned offering 2 gameobject
    [SerializeField] Image Image_OwnedOffering2; // Reference to the owned offering 2 image
    [SerializeField] TextMeshProUGUI Text_OwnedOffering2Name; // Reference to the owned offering 2 name
    [SerializeField] TextMeshProUGUI Text_OwnedOffering2Description; // Reference to the owned offering 2 description
    [SerializeField] Button Button_UnassignOffering2; // Reference to the unassign offering 2 button

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
    [ContextMenu("Initiate God Offerings UI")]
    /// <summary>
    /// This method will be called when end of the wave
    /// </summary>
    public void InitiateGodOfferingsUI() {
        RandomBuyableOfferings();
        UpdateOfferingsUI();
        Panel_GodOffering.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void DeinitiateGodOfferingsUI() {
        Panel_GodOffering.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    /// <summary>
    /// This method will be called when the player wants to buy an offering
    /// </summary>
    void RandomBuyableOfferings() {
        buyableOfferings.Clear();
        godOfferings.Sort((x, y) => Random.Range(-1, 1)); // Shuffle the god offerings
        for (int i = 0 ; buyableOfferings.Count < 2 ; i++) {
            if (godOfferings[i] != playerGodOfferingHandler.godOffering_1 && godOfferings[i] != playerGodOfferingHandler.godOffering_2) {
                buyableOfferings.Add(godOfferings[i]);
            }
        }
    }

    /// <summary>
    /// This method will be called InitiateGodOfferingsUI() is called
    /// </summary>
    void UpdateOfferingsUI() {
        Text_Money.text = MoneyManager.instance.money.ToString(); // Update the money text

        if (buyableOfferings.Count > 0) {
            // Update the buyable offering 1 UI
            Image_BuyableOffering1.sprite = buyableOfferings[0].offeringSprite;
            Image_BuyableOffering1.SetNativeSize();
            Text_BuyableOffering1Name.SetText(buyableOfferings[0].offeringName);
            Text_BuyableOffering1Description.SetText(buyableOfferings[0].offeringDescription);
            Text_CostOffering1.SetText(buyableOfferings[0].offeringCost.ToString());

            // Add listener to the buyable offering 1 button
            Button_BuyableGodOffering1.onClick.RemoveAllListeners();
            Button_BuyableGodOffering1.onClick.AddListener(() => OnBuyableGodOfferingButtonClick(buyableOfferings[0]));

            // Set the buyable offering 1 to active
            Button_BuyableGodOffering1.gameObject.SetActive(true);
        }
        else {
            Button_BuyableGodOffering1.gameObject.SetActive(false);
        }

        if (buyableOfferings.Count > 1) {
            // Update the buyable offering 2 UI
            Image_BuyableOffering2.sprite = buyableOfferings[1].offeringSprite;
            Image_BuyableOffering2.SetNativeSize();
            Text_BuyableOffering2Name.SetText(buyableOfferings[1].offeringName);
            Text_BuyableOffering2Description.SetText(buyableOfferings[1].offeringDescription);
            Text_CostOffering2.SetText(buyableOfferings[1].offeringCost.ToString());

            // Add listener to the buyable offering 2 button
            Button_BuyableGodOffering2.onClick.RemoveAllListeners();
            Button_BuyableGodOffering2.onClick.AddListener(() => OnBuyableGodOfferingButtonClick(buyableOfferings[1]));

            // Set the buyable offering 2 to active
            Button_BuyableGodOffering2.gameObject.SetActive(true);
        }
        else {
            Button_BuyableGodOffering2.gameObject.SetActive(false);
        }

        if (playerGodOfferingHandler.godOffering_1 != null) {
            OwnedOffering1.SetActive(true);
            Image_OwnedOffering1.sprite = playerGodOfferingHandler.godOffering_1.offeringSprite;
            Image_OwnedOffering1.SetNativeSize();
            Text_OwnedOffering1Name.SetText(playerGodOfferingHandler.godOffering_1.offeringName);
            Text_OwnedOffering1Description.SetText(playerGodOfferingHandler.godOffering_1.offeringDescription);

            Button_UnassignOffering1.onClick.RemoveAllListeners();
            Button_UnassignOffering1.onClick.AddListener(() => playerGodOfferingHandler.UnassignGodOffering(playerGodOfferingHandler.godOffering_1));
            Button_UnassignOffering1.onClick.AddListener(() => UpdateOfferingsUI());
        }
        else {
            OwnedOffering1.SetActive(false);
        }

        if (playerGodOfferingHandler.godOffering_2 != null) {
            OwnedOffering2.SetActive(true);
            Image_OwnedOffering2.sprite = playerGodOfferingHandler.godOffering_2.offeringSprite;
            Image_OwnedOffering2.SetNativeSize();
            Text_OwnedOffering2Name.SetText(playerGodOfferingHandler.godOffering_2.offeringName);
            Text_OwnedOffering2Description.SetText(playerGodOfferingHandler.godOffering_2.offeringDescription);

            Button_UnassignOffering2.onClick.RemoveAllListeners();
            Button_UnassignOffering2.onClick.AddListener(() => playerGodOfferingHandler.UnassignGodOffering(playerGodOfferingHandler.godOffering_2));
            Button_UnassignOffering2.onClick.AddListener(() => UpdateOfferingsUI());
        }
        else {
            OwnedOffering2.SetActive(false);
        }
    }

    void OnBuyableGodOfferingButtonClick(SO_GodOffering godOffering) {
        // Check if player has empty offering slot
        if (playerGodOfferingHandler.godOffering_1 != null && playerGodOfferingHandler.godOffering_2 != null) {
            Debug.Log("No empty offering slot");
            return;
        }

        if (MoneyManager.instance.CanAfford(godOffering.offeringCost)) {
            MoneyManager.instance.AddMoney(-godOffering.offeringCost);
            playerGodOfferingHandler.AssignGodOffering(godOffering);
            DeinitiateGodOfferingsUI();
        }
        else {
            Debug.Log("Not enough money");
        }
    }
}
