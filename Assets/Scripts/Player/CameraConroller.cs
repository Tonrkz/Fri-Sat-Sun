using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraConroller : MonoBehaviour {
    public static CameraConroller instance;

    [Header("References")]
    Vector3 startCameraPosition;
    Quaternion startCameraRotation;
    Camera self;
    public Transform FocusingObject { get; set; }

    [Header("Camera Attributes")]
    [SerializeField] float cameraSpeed = 5f;
    [SerializeField] float cameraZoomSpeed = 5f;
    [SerializeField] float cameraRotationSpeed = 5f;
    [SerializeField] float cameraMinFOV = 30f;
    [SerializeField] float cameraMaxFOV = 70f;
    [SerializeField] float cameraMinY = 5f;
    [SerializeField] float cameraMaxX = -5f;

    [Header("Damping Attributes")]
    [SerializeField] float positionDamping = 0.2f;
    [SerializeField] float rotationDamping = 0.2f;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        startCameraPosition = transform.position;
        startCameraRotation = transform.rotation;
        self = GetComponent<Camera>();
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(() => StartCoroutine(SmoothDampFocusObject()));
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(() => StartCoroutine(ResetCamera()));
    }

    public IEnumerator SmoothDampFocusObject() {
        FocusingObject = PlayerTowerSelectionHandler.instance.SelectedTower.transform;
        while (FocusingObject != null) {
            Quaternion targetRotation = Quaternion.LookRotation(FocusingObject.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraRotationSpeed * Time.deltaTime);

            self.DOFieldOfView(cameraMinFOV, cameraZoomSpeed);
            yield return null;
        }
    }

    public IEnumerator ResetCamera() {
        FocusingObject = null;
        while (FocusingObject == null) {
            transform.rotation = Quaternion.Slerp(transform.rotation, startCameraRotation, cameraRotationSpeed * Time.deltaTime);
            self.DOFieldOfView(cameraMaxFOV, cameraZoomSpeed);
            yield return null;
        }
    }
}
