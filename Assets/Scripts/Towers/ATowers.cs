using DG.Tweening;
using System;
using System.Collections;
using TMPro;
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
                towerNameText.SetText("<b>" + activatableTower.AssignedWord + "</b>");
                if (activatableTower.AssignedWord == "" || activatableTower.AssignedWord == null) {
                    towerNamePanel.SetActive(false);
                }
                else {
                    switch (TowerType) {
                        case Enum_TowerTypes.Campfire: // Send unit
                            towerNamePanel.GetComponent<SpriteRenderer>().color = new Color32(249, 255, 90, 255);
                            break;
                        case Enum_TowerTypes.Attacker: // Send unit
                            towerNamePanel.GetComponent<SpriteRenderer>().color = new Color32(249, 255, 90, 255);
                            break;
                        case Enum_TowerTypes.Ranged: // Send object
                            towerNamePanel.GetComponent<SpriteRenderer>().color = new Color32(157, 245, 243, 255);
                            break;
                        case Enum_TowerTypes.Mage: // Buff
                            towerNamePanel.GetComponent<SpriteRenderer>().color = new Color32(151, 121, 255, 255);
                            break;
                    }
                    towerNamePanel.SetActive(true);
                }
            }
        }

        Debug.Log($"{towerNameText.text} displayed");

        void DisplayTowerName() {
            towerNameText.SetText("<b>" + TowerName + "</b>");
            towerNameText.fontStyle = FontStyles.UpperCase;
            towerNamePanel.SetActive(true);
            towerNamePanel.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    public virtual void DestroyTower() {
        MoneyManager.instance.AddMoney(BuildCost * MoneyManager.instance.percentRefund * GlobalAttributeMultipliers.PercentRefundMultiplier);
        OccupiedGround.GetComponent<GroundScript>().tower = null;
        OccupiedGround.GetComponent<GroundScript>().hasTower = false;
        TowerNameManager.instance.usedTowerNames.Remove(TowerName);
        BuildManager.instance.builtTowerList.Remove(gameObject);

        // Play dead animation
        render.PlayAnimation(render.HURT);
    }

    public virtual void ChangeTowerState(Enum newState) {
        state = newState;
    }

    public virtual void Dead() {
        // Unsuscribe from events
        if (IsSelected) {
            PlayerTowerSelectionHandler.instance.OnTowerDeselected.Invoke();
        }

        PlayerTowerSelectionHandler.instance.OnTowerSelected.RemoveListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.RemoveListener(this.OnDeselected);

        Destroy(gameObject);
    }

    public void OnSelected() {
        if (IsSelected) {
            DOVirtual.Color(transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetColor("_Tint"), new Color(1, 1, 1, 1), 0.15f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", x));
            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else {
            DOVirtual.Color(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.25f), 0.15f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", x));
            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 0.25f));
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnDeselected() {
        DOVirtual.Color(transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetColor("_Tint"), new Color(1, 1, 1, 1), 0.15f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", x));
        //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
        transform.GetChild(1).gameObject.SetActive(true);
    }
}
