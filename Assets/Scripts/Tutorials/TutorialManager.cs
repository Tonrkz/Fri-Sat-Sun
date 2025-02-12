using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TutorialManager : MonoBehaviour {
    public static TutorialManager instance;

    [Header("References")]
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] GameObject pressEnterToContinueText;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerMovementTutorialTargetLocation;


    [Header("Attributes")]
    [SerializeField] Enum_TutorialState state = Enum_TutorialState.Start;
    List<string> startTutorialMessages = new List<string> {
        $"Welcome to Kingdom Kome!",
        $"Because you are a new king to this kingdom, I am going to teach you the basics of managing your army.",
        $"Let's get started!"
    };
    List<string> playerMovementTutorialMessages = new List<string> {
        $"First, you need to move your king to the desired location.",
        $"Use the arrow keys to move your king.",
        $"Now, move to the highlighted location.",
        $"Good job! You have successfully moved into the desired location."
    };
    List<string> buildCommandTutorialMessages = new List<string> {
        $"Let's plan our defense.",
        $"In Kingdom Kome, you have to build a campfire first.",
        $"To build a campfire, type 'build' and press enter.",
        $"Now, try building a campfire by typing 'build' and press enter.",
        $"Good job! You have successfully built a campfire."
    };

    [Header("Debug")]
    public bool IsConditionMeet { get; private set; }
    Byte tutorialTextIndex = 0;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        tutorialPanel.SetActive(true);
        InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
        OnInitiateTutorial();
    }

    void Update() {
        OnTutorialConditionCheck();
    }

    /// TODO: Add the tutorial text
    /// <summary>
    /// Initiate tutorial based on the current state
    /// </summary>
    void OnInitiateTutorial() {
        pressEnterToContinueText.SetActive(true);
        IsConditionMeet = false;
        InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
        switch (state) {
            case Enum_TutorialState.Start:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, startTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.PlayerMovement:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.BuildCommand:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.ModeSwitching:
                break;
            case Enum_TutorialState.TowerActivation:
                break;
            case Enum_TutorialState.DestroyCommmand:
                break;
            case Enum_TutorialState.PlayerTest1:
                break;
            case Enum_TutorialState.DifferentiateCommand:
                break;
            case Enum_TutorialState.UpgradeCommand:
                break;
            case Enum_TutorialState.PlayerTest2:
                break;
            case Enum_TutorialState.TaxCommand:
                break;
            case Enum_TutorialState.GodOffering:
                break;
            case Enum_TutorialState.PlayerTest3:
                break;
            case Enum_TutorialState.End:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Check the condition of the tutorial
    /// </summary>
    void OnTutorialConditionCheck() {
        switch (state) {
            case Enum_TutorialState.Start:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < startTutorialMessages.Count - 1) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, startTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        if (!IsConditionMeet) {
                            OnTutorialConditionMeet();
                        }
                    }
                }
                break;

            case Enum_TutorialState.PlayerMovement:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < playerMovementTutorialMessages.Count - 3) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[playerMovementTutorialMessages.Count - 2]);
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = Enum_GameInputState.CommandMode;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                if (Vector3.Distance(player.transform.position, playerMovementTutorialTargetLocation.transform.position) < 1.5f) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                break;

            case Enum_TutorialState.BuildCommand:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < buildCommandTutorialMessages.Count - 3) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[buildCommandTutorialMessages.Count - 2]);
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = Enum_GameInputState.CommandMode;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                if (BuildManager.instance.builtTowerList.Count > 0) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                break;

            case Enum_TutorialState.ModeSwitching:
                break;
            case Enum_TutorialState.TowerActivation:
                break;
            case Enum_TutorialState.DestroyCommmand:
                break;
            case Enum_TutorialState.PlayerTest1:
                break;
            case Enum_TutorialState.DifferentiateCommand:
                break;
            case Enum_TutorialState.UpgradeCommand:
                break;
            case Enum_TutorialState.PlayerTest2:
                break;
            case Enum_TutorialState.TaxCommand:
                break;
            case Enum_TutorialState.GodOffering:
                break;
            case Enum_TutorialState.PlayerTest3:
                break;
            case Enum_TutorialState.End:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Active when meet the condition of the tutorial
    /// and update the tutorial state
    /// </summary>
    void OnTutorialConditionMeet() {
        IsConditionMeet = true;
        switch (state) {
            case Enum_TutorialState.Start:
                UpdateStep();
                break;

            case Enum_TutorialState.PlayerMovement:
                InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[playerMovementTutorialMessages.Count - 1]);
                pressEnterToContinueText.SetActive(true);
                break;

            case Enum_TutorialState.BuildCommand:
                InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[buildCommandTutorialMessages.Count - 1]);
                pressEnterToContinueText.SetActive(true);
                break;

            case Enum_TutorialState.ModeSwitching:
                break;
            case Enum_TutorialState.TowerActivation:
                break;
            case Enum_TutorialState.DestroyCommmand:
                break;
            case Enum_TutorialState.PlayerTest1:
                break;
            case Enum_TutorialState.DifferentiateCommand:
                break;
            case Enum_TutorialState.UpgradeCommand:
                break;
            case Enum_TutorialState.PlayerTest2:
                break;
            case Enum_TutorialState.TaxCommand:
                break;
            case Enum_TutorialState.GodOffering:
                break;
            case Enum_TutorialState.PlayerTest3:
                break;
            case Enum_TutorialState.End:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Update the tutorial state
    /// </summary>
    void UpdateStep() {
        state++;
        tutorialTextIndex = 0;
        OnInitiateTutorial();
    }
}
