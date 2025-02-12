using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class TutorialManager : MonoBehaviour {
    public static TutorialManager instance;

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
        $"Welcome to Kingdom Kome! My new king.",
        $"You are the chosen one to be the king, in the hard time like this, when that Demon King trying to conquer our kingdom.",
        $"Because you are a new king to this kingdom, I am going to teach you the basics of managing the army."
    };
    List<string> uiIntroductionTutorialMessages = new List<string> {
        $"I'll introduce what are you seeing now.",
        $"The big land you are seeing now is our battlefield.",
        $"The square frame on a land is a space which you are selecting.",
        $"On top of the screen you will see a bar, which indicates time left in a day or night.",
        $"Left side of it is a day count. The scouts told me that Demon King's army would last around 5 days.",
        $"So, we have to survive for 5 days to win this war.",
        $"On the right side, there is a paper which you will write the command, in order to withstand the demon horde.",
        $"Above that, it's the money you are holding. Of course, every war have to use money.",
        $"That's all you need to know for now. Let's get start!"
    };
    List<string> playerMovementTutorialMessages = new List<string> {
        $"First, you need to move your king to the desired location.",
        $"Use the arrow keys to move your king.",
        $"Now, move to the highlighted location.",
        $"Good job! You have successfully moved into the desired location."
    };
    List<string> buildCommandTutorialMessages = new List<string> {
        $"Let's plan our defense.",
        $"In Kingdom Kome, before you can build any tower, you must build a campfire first.",
        $"Campfire's cost is {MoneyManager.campfireBuildCost}. Make sure you have enough money to built it.",
        $"To build a campfire, type 'build' and press enter.",
        $"Now, try building a campfire by typing 'build' and press enter.",
        $"Good job! You have successfully built a campfire. Each tower will have their own name displaying above it."
    };
    List<string> modeSwitchingTutorialMessages = new List<string> {
        $"Next step is how to defense our kingdom.",
        $"There are few ways to withstand against the demon horde.",
        $"One of them is to send out some soldiers to fight.",
        $"In order to do that, press 'Space Bar'."
    };
    List<string> towerActivationTutorialMessages = new List<string> {
        $"As you can see, your campfire's name has been changed into another word.",
        $"This is because you are now in 'Activation Mode'.",
        $"In this mode, you can activate your tower by typing the word above it.",
        $"Each tower has their own ability when activated.",
        $"Try activate your campfire by typing word above it.",
        $"Great job, you have sent a soldier to defend our kingdom! Remember that one soldier can occupy one tile."
    };
    List<string> destroyCommandTutorialMessages = new List<string> {
        $"But I think that campfire is a little bit too far from where the horde will come.",
        $"It'd be better if you build towers close to the frontline.",
        $"Let's build another campfire.",
        $"You would have ask that you have not enough money to build another one, wouldn't you?",
        $"That's why you can destroy it! To get some money back!",
        $"To destroy a tower, type 'destroy' and the tower's name.",
        $"Now you can build another campfire, because you have enough money!"
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
                if (!"build".StartsWith(CommandTyperScript.instance.inputString) && CommandTyperScript.instance.inputString != "" && CommandTyperScript.instance.inputString != null) {
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, "Looks like you have write a wrong command, you can delete it by pressing 'Backspace'");
                    if (Input.GetKeyDown(KeyCode.Backspace)) {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, buildCommandTutorialMessages[buildCommandTutorialMessages.Count - 2]);
                    }
                }
                if (BuildManager.instance.builtTowerList.Count > 0) {
                    if (!IsConditionMeet) {
                        builtTower = BuildManager.instance.builtTowerList[0];
                        OnTutorialConditionMeet();
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
                        InputStateManager.instance.GameInputState = Enum_GameInputState.CommandMode;
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
                        InputStateManager.instance.GameInputState = Enum_GameInputState.ActivateMode;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
                break;

            case Enum_TutorialState.DestroyCommmand:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (tutorialTextIndex < destroyCommandTutorialMessages.Count - 2) {
                        tutorialTextIndex++;
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, destroyCommandTutorialMessages[tutorialTextIndex]);
                    }
                    if (tutorialTextIndex == destroyCommandTutorialMessages.Count - 2) {
                        tutorialTextIndex++;
                        pressEnterToContinueText.SetActive(false);
                        InputStateManager.instance.GameInputState = Enum_GameInputState.ActivateMode;
                    }
                    if (IsConditionMeet) {
                        UpdateStep();
                        return;
                    }
                }
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
                    UserInterfaceManager.instance.ChangeTextMessage(tutorialText, $"For example, try '{"destroy"} {BuildManager.instance.builtTowerList[0].GetComponent<ITowers>().TowerName}'!");
                    if (!$"{"destroy"} {BuildManager.instance.builtTowerList[0].GetComponent<ITowers>().TowerName}".StartsWith(CommandTyperScript.instance.inputString) && CommandTyperScript.instance.inputString != "" && CommandTyperScript.instance.inputString != null) {
                        UserInterfaceManager.instance.ChangeTextMessage(tutorialText, "Looks like you have write a wrong command, you can delete it by pressing 'Backspace'");
                    }
                }
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
                UpdateStep();
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
