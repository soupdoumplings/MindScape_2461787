using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorFreeCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float fastMoveMultiplier = 2.5f;
    [SerializeField] private float lookSensitivity = 0.12f;

    private bool freeCameraActive;
    private Transform playerCameraParent;
    private Vector3 savedLocalPosition;
    private Quaternion savedLocalRotation;
    private Behaviour firstPersonController;
    private PlayerInput playerInput;
    private float pitch;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
            ToggleFreeCamera();

        if (!freeCameraActive)
            return;

        UpdateLook();
        UpdateMovement();
    }

    private void ToggleFreeCamera()
    {
        freeCameraActive = !freeCameraActive;

        if (freeCameraActive)
        {
            PlayerMotionReader player = FindFirstObjectByType<PlayerMotionReader>();
            if (player != null)
            {
                firstPersonController = player.GetComponent("FirstPersonController") as Behaviour;
                playerInput = player.GetComponent<PlayerInput>();
                if (firstPersonController != null) firstPersonController.enabled = false;
                if (playerInput != null) playerInput.enabled = false;
            }

            playerCameraParent = transform.parent;
            savedLocalPosition = transform.localPosition;
            savedLocalRotation = transform.localRotation;
            transform.SetParent(null, true);
            Vector3 euler = transform.eulerAngles;
            pitch = euler.x > 180f ? euler.x - 360f : euler.x;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Free camera ON — WASD move, mouse look, Q/E down/up, Shift boost, F1 return.");
        }
        else
        {
            if (playerCameraParent != null)
            {
                transform.SetParent(playerCameraParent, false);
                transform.localPosition = savedLocalPosition;
                transform.localRotation = savedLocalRotation;
            }

            if (firstPersonController != null) firstPersonController.enabled = true;
            if (playerInput != null) playerInput.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Free camera OFF — first-person camera restored.");
        }
    }

    private void UpdateLook()
    {
        if (Mouse.current == null)
            return;

        Vector2 look = Mouse.current.delta.ReadValue() * lookSensitivity;
        pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
        transform.rotation = Quaternion.Euler(pitch, transform.eulerAngles.y + look.x, 0f);
    }

    private void UpdateMovement()
    {
        if (Keyboard.current == null)
            return;

        Vector3 input = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) input.z += 1f;
        if (Keyboard.current.sKey.isPressed) input.z -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.eKey.isPressed) input.y += 1f;
        if (Keyboard.current.qKey.isPressed) input.y -= 1f;

        float speed = moveSpeed * (Keyboard.current.leftShiftKey.isPressed ? fastMoveMultiplier : 1f);
        transform.position += transform.TransformDirection(input.normalized) * speed * Time.unscaledDeltaTime;
    }
}
