using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 4.5f;

    private Rigidbody body;
    public Vector3 PlanarVelocity { get; private set; }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        }

        Vector3 desiredVelocity = new Vector3(input.x, 0f, input.y).normalized * movementSpeed;
        PlanarVelocity = desiredVelocity;
        body.linearVelocity = new Vector3(desiredVelocity.x, body.linearVelocity.y, desiredVelocity.z);

        if (desiredVelocity.sqrMagnitude > 0.01f)
            transform.forward = Vector3.Slerp(transform.forward, desiredVelocity.normalized, 14f * Time.fixedDeltaTime);
    }
}
