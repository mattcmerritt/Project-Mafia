using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPeek : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera mainCamera;
    [Range(2, 100)][SerializeField] private float cameraTargetDivider;

    private void Update()
    {
        Vector3 mousePosition = playerTransform.GetComponent<PlayerMovement>().UseCursorPositionAsTarget();
        var cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * playerTransform.position) / cameraTargetDivider;
        transform.position = cameraTargetPosition;
    }
}