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
    [SerializeField] float cameraMinFOV = 50f;
    [SerializeField] float cameraMaxFOV = 70f;
    [SerializeField] float cameraMinY = 5f;
    [SerializeField] float cameraMaxZ = -10f;

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
            self.DOFieldOfView(cameraMinFOV, cameraZoomSpeed);

            Vector3 targetPosition = FocusingObject.position + Vector3.up * cameraMinY - Vector3.back * cameraMaxZ;
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);

            Quaternion lookAt = Quaternion.LookRotation(new Vector3(transform.position.x, FocusingObject.transform.position.y, FocusingObject.transform.position.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, cameraRotationSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public IEnumerator ResetCamera() {
        FocusingObject = null;
        while (FocusingObject == null) {
            transform.position = Vector3.Lerp(transform.position, startCameraPosition, cameraSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, startCameraRotation, cameraRotationSpeed * Time.deltaTime);
            self.DOFieldOfView(cameraMaxFOV, cameraZoomSpeed);
            yield return null;
        }
    }
}
