using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GodsOfferingManager : MonoBehaviour {
    public static GodsOfferingManager instance;

    [Header("References")]
    [SerializeField] PlayerGodOfferingHandler playerGodOfferingHandler; // Reference to the player god offering handler
    [SerializeField] GameObject Panel_GodOffering; // Reference to the god offering panel
    [SerializeField] Button Button_BuyableGodOffering1; // Reference to the buyable offering 1 button
    [SerializeField] Image Image_BuyableOffering1; // Reference to the buyable offering 1 button
    [SerializeField] TextMeshProUGUI Text_BuyableOffering1; // Reference to the buyable offering 1 text
    [SerializeField] TextMeshProUGUI Text_CostOffering1; // Reference to the cost offering 1 text
    [SerializeField] Button Button_BuyableGodOffering2; // Reference to the buyable offering 2 button`
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
            Text_BuyableOffering1.text = buyableOfferings[0].offeringName;
            Text_CostOffering1.text = buyableOfferings[0].offeringCost.ToString();

            // Add listener to the buyable offering 1 button
            Button_BuyableGodOffering1.onClick.RemoveAllListeners();
            Button_BuyableGodOffering1.onClick.AddListener(() => OnBuyableGodOfferingButtonClick(buyableOfferings[0]));

            // Set the buyable offering 1 to active
            Image_BuyableOffering1.gameObject.SetActive(true);
        }
        else {
            Image_BuyableOffering1.gameObject.SetActive(false);
        }

        if (buyableOfferings.Count > 1) {
            // Update the buyable offering 2 UI
            Image_BuyableOffering2.sprite = buyableOfferings[1].offeringSprite;
            Text_BuyableOffering2.text = buyableOfferings[1].offeringName;
            Text_CostOffering2.text = buyableOfferings[1].offeringCost.ToString();

            // Add listener to the buyable offering 2 button
            Button_BuyableGodOffering2.onClick.RemoveAllListeners();
            Button_BuyableGodOffering2.onClick.AddListener(() => OnBuyableGodOfferingButtonClick(buyableOfferings[1]));

            // Set the buyable offering 2 to active
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

    void OnBuyableGodOfferingButtonClick(SO_GodOffering godOffering) {
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
