using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotionReader : MonoBehaviour
{
    private CharacterController controller;

    public Vector3 PlanarVelocity
    {
        get
        {
            Vector3 velocity = controller.velocity;
            return new Vector3(velocity.x, 0f, velocity.z);
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
}
