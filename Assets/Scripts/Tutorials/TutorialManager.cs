using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TutorialManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] GameObject pressEnterToContinueText;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerMovementTutorialTargetLocation;
    GameObject builtTower;


    [Header("Attributes")]
    [SerializeField] Enum_TutorialState state = Enum_TutorialState.Start;
    List<string> startTutorialMessages = new List<string> {
        $"Welcome to Kingdom Kome, my liege!",
        $"You are our chosen king, come to us in this dire hour as the Demon King seeks to conquer our realm.",
        $"As you are new to the throne, allow me to guide you through the basics of commanding our armies."
    };

    List<string> uiIntroductionTutorialMessages = new List<string> {
        $"Let me acquaint you with your surroundings, Your Majesty.",
        $"This vast expanse before you is our battlefield.",
        $"Each square upon the land denotes a space you may select.",
        $"Above, you will see a bar indicating the passage of day and night.",
        $"To its left, the count of days. Our scouts foretell the Demon King's onslaught will endure for five days.",
        $"Thus, we must withstand the onslaught for five days to claim victory in this war.",
        $"To the right, you see the parchment where you shall issue commands to repel the demonic horde.",
        $"And above, your treasury, for even war demands coin.",
        $"That is all you require for now. Let us begin!"
    };

    List<string> playerMovementTutorialMessages = new List<string> {
        $"Firstly, you must command your king to a strategic location.", // 0
        $"Utilize the arrow keys to direct your royal personage.", // 1
        $"Now, move to the location marked for you.", // 2
        // Player move to the target location
        $"Excellent! You have successfully positioned yourself at the designated location." // 3
    };

    List<string> buildCommandTutorialMessages = new List<string> {
        $"Let us devise our defenses.", // 0
        $"In Kingdom Kome, ere any tower may rise, a campfire must first be established.", // 1
        $"A campfire's cost is {MoneyManager.campfireBuildCost}. Ensure you possess sufficient funds to construct it.", // 2
        $"To raise a campfire, type 'build' and press enter.", // 3
        // Player build a campfire
        $"Well done! A campfire has been erected. Each tower will bear its name above it for ease of command." // 4
    };

    List<string> modeSwitchingTutorialMessages = new List<string> {
        $"Now we shall turn our attention to defending our kingdom.",
        $"There are several strategies to resist the demonic horde.",
        $"One such tactic is to dispatch soldiers into the fray.",
        $"To achieve this, press the 'Space Bar'."
        // Player switch to activation mode
    };

    List<string> towerActivationTutorialMessages = new List<string> {
        $"Observe! The campfire's designation has shifted.", // 0
        $"This signifies your entry into 'Activation Mode'.", // 1
        $"In this mode, you may activate your towers by inputting the word displayed above them.", // 2
        $"Each tower boasts a unique ability upon activation.", // 3
        $"Attempt to activate your campfire by typing the word now displayed above it.", // 4 
        // Player activate the campfire
        $"Magnificent! You have dispatched a soldier to defend our kingdom! Remember, each soldier occupies a single tile." // 5
    };

    List<string> destroyCommandTutorialMessages = new List<string> {
        $"However, I believe this campfire is somewhat distant from the impending assault.", // 0
        $"It would serve us better to construct towers closer to the front lines.", // 1
        $"Let us erect another campfire.", // 2
        $"You might inquire if your coffers are sufficient for another, would you not?", // 3
        $"Hence, you possess the power to dismantle it! To reclaim a portion of your investment!", // 4
        $"To dismantle a tower, type 'destroy' followed by the tower's name.", // 5
        // Player destroy the campfire
        $"Now, you possess the funds to build another campfire in a more advantageous position!" // 6
        // Player build another campfire
    };

    List<string> playerTest1Messages = new List<string> {
        $"Hark! Look! A demon approaches!", // 0
        $"You must deploy your soldier to engage it in combat!" // 1
        // Player kills the demon
    };

    List<string> evolveCommandTutorialMessages = new List<string> {
        $"You have proven yourself a capable commander, Your Majesty.", // 0
        $"Now, let us delve into the intricacies of tower evolution.", // 1
        $"Each tower may be upgraded to enhance its abilities.", // 2
        $"To evolve a tower, type '[Tower name] evolve [Tower Type]' ", // 3
        $"The cost of evolution is dependent upon the tower's type.", // 4
        $"Let's evolve the campfire into an attacker tower.", // 5
        // Player evolve the campfire into an attacker tower
        $"A new tower has been born! It shall serve us well in the battles to come.", // 6
        $"Here is the cost to evolve each tower type and it's power: {BuildManager.attackerStringRef} = {MoneyManager.attackerTowerBuildCost} (Sends a stronger soldier)", // 
        $"{BuildManager.rangedStringRef} = {MoneyManager.rangedTowerBuildCost} (Shoots enemies from afar)", // 7
        $"{BuildManager.mageStringRef} = {MoneyManager.mageTowerBuildCost} (Slows down demons)" // 9
    };

    List<string> upgradeCommandTutorialMessages = new List<string> {
        $"You have proven yourself a capable commander, Your Majesty.", // 0
        $"Now, let us delve into the intricacies of tower evolution.", // 1
        $"Each tower may be upgraded to enhance its abilities.", // 2
        $"To evolve a tower, type '[Tower name] evolve [Tower Type]' ", // 3
        $"The cost of evolution is dependent upon the tower's type.", // 4
        $"Let's evolve the campfire" // 5
    };

    [Header("Debug")]
    public bool IsConditionMeet { get; private set; }
    Byte tutorialTextIndex = 0;
    Enum_GameInputState previousInputState;

    void Start() {
        tutorialPanel.SetActive(true);
        InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
        StartCoroutine(OnInitiateTutorial());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F12)) {
            state++;
            StartCoroutine(OnInitiateTutorial());
        }
        OnTutorialConditionCheck();
    }

    /// TODO: Add the tutorial text
    /// <summary>
    /// Initiate tutorial based on the current state
    /// </summary>
    IEnumerator OnInitiateTutorial() {
        yield return new WaitForEndOfFrame();
        tutorialPanel.SetActive(true);
        pressEnterToContinueText.SetActive(true);
        IsConditionMeet = false;
        previousInputState = InputStateManager.instance.GameInputState;
        InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
        switch (state) {
            case Enum_TutorialState.Start:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, startTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.UIIntroduction:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, uiIntroductionTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.PlayerMovement:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.BuildCommand:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.ModeSwitching:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, modeSwitchingTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.TowerActivation:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, towerActivationTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.DestroyCommmand:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, destroyCommandTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.PlayerTest1:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerTest1Messages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.EvolveCommand:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, evolveCommandTutorialMessages[tutorialTextIndex]);
                break;
            case Enum_TutorialState.UpgradeCommand:
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, upgradeCommandTutorialMessages[tutorialTextIndex]);
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
        yield return null;
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

            case Enum_TutorialState.UIIntroduction:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < uiIntroductionTutorialMessages.Count - 1) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, uiIntroductionTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        if (!IsConditionMeet) {
                            OnTutorialConditionMeet();
                        }
                    }
                }
                break;

            case Enum_TutorialState.PlayerMovement:
                if (Vector3.Distance(player.transform.position, playerMovementTutorialTargetLocation.transform.position) < 1.5f) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < playerMovementTutorialMessages.Count - 3) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerMovementTutorialMessages[playerMovementTutorialMessages.Count - 2]);
                        pressEnterToContinueText.SetActive(false);
                        playerMovementTutorialTargetLocation.GetComponent<MeshRenderer>().material.color = Color.red;
                        //playerMovementTutorialTargetLocation.GetComponent<MeshRenderer>().material.SetColor("_Tint", Color.yellow);
                        InputStateManager.instance.GameInputState = Enum_GameInputState.CommandMode;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        playerMovementTutorialTargetLocation.GetComponent<MeshRenderer>().material.color = Color.white;
                        //playerMovementTutorialTargetLocation.GetComponent<MeshRenderer>().material.SetColor("_Tint", new Color32(0, 115, 6, 255));
                        return;
                    }
                }
                break;

            case Enum_TutorialState.BuildCommand:
                if (BuildManager.instance.builtTowerList.Count > 0) {
                    if (!IsConditionMeet) {
                        builtTower = BuildManager.instance.builtTowerList[0];
                        if ((Enum_CampfireState)builtTower.GetComponent<CampfireScript>().state != Enum_CampfireState.Building) {
                            OnTutorialConditionMeet();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Backspace)) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[buildCommandTutorialMessages.Count - 2]);
                }
                if (!"build".StartsWith(CommandTyperScript.instance.inputString) && CommandTyperScript.instance.inputString != "" && CommandTyperScript.instance.inputString != null) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, "Looks like you have write a wrong command, you can delete it by pressing 'Backspace'");

                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < buildCommandTutorialMessages.Count - 3) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[buildCommandTutorialMessages.Count - 2]);
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = previousInputState;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                break;

            case Enum_TutorialState.ModeSwitching:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < modeSwitchingTutorialMessages.Count - 2) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, modeSwitchingTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, modeSwitchingTutorialMessages[modeSwitchingTutorialMessages.Count - 1]);
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = previousInputState;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                break;

            case Enum_TutorialState.TowerActivation:
                if (GameObject.FindGameObjectWithTag("Soldier")) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < towerActivationTutorialMessages.Count - 3) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, towerActivationTutorialMessages[tutorialTextIndex]);
                    }
                    else {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, towerActivationTutorialMessages[towerActivationTutorialMessages.Count - 2]);
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = previousInputState;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                break;

            case Enum_TutorialState.DestroyCommmand:
                if (builtTower.IsDestroyed()) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, destroyCommandTutorialMessages[tutorialTextIndex]);
                    if (BuildManager.instance.builtTowerList.Count > 0) {
                        OnTutorialConditionMeet();
                    }
                    return;
                }
                if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, $"Don't forget to return back to command mode, in order to write a command! (Pressing 'Space Bar')");

                }
                else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, $"For example, try '{"destroy"} {BuildManager.instance.builtTowerList[0].GetComponent<ATowers>().TowerName}'!");
                    if (!$"{"destroy"} {BuildManager.instance.builtTowerList[0].GetComponent<ATowers>().TowerName}".StartsWith(CommandTyperScript.instance.inputString) && CommandTyperScript.instance.inputString != "" && CommandTyperScript.instance.inputString != null) {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, "Looks like you have write a wrong command, you can delete it by pressing 'Backspace'");
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < destroyCommandTutorialMessages.Count - 2) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, destroyCommandTutorialMessages[tutorialTextIndex]);
                    }
                    if (tutorialTextIndex == destroyCommandTutorialMessages.Count - 2) {
                        tutorialTextIndex++;
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = previousInputState;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                break;

            case Enum_TutorialState.PlayerTest1:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < playerTest1Messages.Count - 1) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, playerTest1Messages[tutorialTextIndex]);
                        InputStateManager.instance.GameInputState = previousInputState;
                        pressEnterToContinueText.SetActive(false);
                        StartCoroutine(DemonsSpawnerManager.instance.TutorialPlayerTest1());
                    }
                }
                if (DemonsSpawnerManager.instance.DemonAlive == 0 && DemonsSpawnerManager.instance.DemonCount == 0) {
                    if (!IsConditionMeet) {
                        OnTutorialConditionMeet();
                    }
                }
                break;
            case Enum_TutorialState.EvolveCommand:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < 5) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, evolveCommandTutorialMessages[tutorialTextIndex]);
                    }
                    else if (tutorialTextIndex == 5) {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, $"For example, type {builtTower.GetComponent<ATowers>().TowerName} evolve attacker");
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = previousInputState;
                    }
                    else if (tutorialTextIndex <= evolveCommandTutorialMessages.Count - 1) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, evolveCommandTutorialMessages[tutorialTextIndex]);
                        pressEnterToContinueText.SetActive(true);
                        if (tutorialTextIndex == evolveCommandTutorialMessages.Count - 1) {
                            OnTutorialConditionMeet();
                        }
                    }
                }
                builtTower = BuildManager.instance.builtTowerList[0];
                if (builtTower.GetComponent<ATowers>().TowerType != Enum_TowerTypes.Campfire) {
                    if (!IsConditionMeet) {
                        IsConditionMeet = true;
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, evolveCommandTutorialMessages[tutorialTextIndex]);
                        previousInputState = InputStateManager.instance.GameInputState;
                        InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
                        pressEnterToContinueText.SetActive(true);
                    }
                }
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

            case Enum_TutorialState.UIIntroduction:
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
                UpdateStep();
                break;

            case Enum_TutorialState.TowerActivation:
                InputStateManager.instance.GameInputState = Enum_GameInputState.Tutorial;
                UserInterfaceManager.instance.ChangeTextMessage(tutorialText, towerActivationTutorialMessages[towerActivationTutorialMessages.Count - 1]);
                pressEnterToContinueText.SetActive(true);
                break;
            case Enum_TutorialState.DestroyCommmand:
                builtTower = BuildManager.instance.builtTowerList[0];
                UpdateStep();
                break;
            case Enum_TutorialState.PlayerTest1:
                UpdateStep();
                break;
            case Enum_TutorialState.EvolveCommand:
                UpdateStep();
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

    IEnumerator HideTalkingBubble(Single shownDuration) {
        yield return new WaitForSeconds(shownDuration);
        tutorialPanel.SetActive(false);
    }

    /// <summary>
    /// Update the tutorial state
    /// </summary>
    void UpdateStep() {
        state++;
        tutorialTextIndex = 0;
        StartCoroutine(OnInitiateTutorial());
    }
}
