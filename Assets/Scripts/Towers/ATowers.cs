using System;
using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ATowers : MonoBehaviour, ITowers {
    [Header("References")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected GameObject towerNamePanel;
    [SerializeField] protected TextMeshPro towerNameText;
    [SerializeField] protected AnimatorRenderer render;
    [SerializeField] protected HealthComponent health;



    [Header("Tower Attributes")]
    public string TowerName { get; set; }
    public Byte Level { get; set; }
    public bool StartCanSeePhantom { get; set; }
    public bool CanSeePhantom { get; set; }



    [Header("Money Attributes")]
    public int BuildCost { get; set; }



    [Header("Debug")]
    [SerializeField] internal Enum state;
    public Enum_TowerTypes TowerType { get; protected set; }
    [SerializeField] protected GameObject occupiedGround;
    public GameObject OccupiedGround { get => occupiedGround; set => occupiedGround = value; }
    public bool IsSelected { get; set; } = false;



    public void SetTowerName(string towerNameInput) {
        TowerName = towerNameInput;
    }

    public IEnumerator DisplayTowerNameOrAssignedWord() {
        yield return new WaitForEndOfFrame();
        DisplayTowerName();
        // Check if tower has IActivatables interface
        if (gameObject.GetComponent<IActivatables>() != null && InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode) {
            IActivatables activatableTower = GetComponent<IActivatables>();
            if ((Enum_CampfireState)state == Enum_CampfireState.Active || (Enum_AttackerTowerState)state == Enum_AttackerTowerState.Active || (Enum_RangedTowerState)state == Enum_RangedTowerState.Active || (Enum_MageTowerState)state == Enum_MageTowerState.Active) {
                // Display assigned word if it has one
                towerNameText.text = activatableTower.AssignedWord;
                towerNameText.fontStyle = FontStyles.Bold;
                if (activatableTower.AssignedWord == "" || activatableTower.AssignedWord == null) {
                    towerNamePanel.SetActive(false);
                }
                else {
                    towerNamePanel.SetActive(true);
                }
            }
        }

        Debug.Log($"{towerNameText.text} displayed");

        void DisplayTowerName() {
            towerNameText.text = TowerName;
            towerNameText.fontStyle = FontStyles.UpperCase;
            towerNamePanel.SetActive(true);
        }
    }

    public virtual void DestroyTower() {
        MoneyManager.instance.AddMoney(BuildCost * MoneyManager.instance.percentRefund * GlobalAttributeMultipliers.PercentRefundMultiplier);
        OccupiedGround.GetComponent<GroundScript>().tower = null;
        OccupiedGround.GetComponent<GroundScript>().hasTower = false;
        TowerNameManager.instance.usedTowerNames.Remove(TowerName);
        BuildManager.instance.builtTowerList.Remove(gameObject);
    }

    public virtual void ChangeTowerState(Enum newState) {
        state = newState;
    }

    protected virtual void Dead() {
        // Unsuscribe from events
        PlayerTowerSelectionHandler.instance.OnTowerSelected.RemoveListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.RemoveListener(this.OnDeselected);

        Destroy(gameObject);
    }

    public void OnSelected() {
        if (IsSelected) {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else {
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 0.25f));
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnDeselected() {
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
        transform.GetChild(1).gameObject.SetActive(true);
    }
}
