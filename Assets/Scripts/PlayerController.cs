using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 1;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //camera direction
        Vector3 forward = transform.FindChild("cameraPivot").FindChild("cameraCorrection").FindChild("Camera").TransformDirection(Vector3.forward);
        //doesn't need y
        forward.y = 0;
        forward.Normalize();
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("leftanalogY");

        Vector3 targetDirection = v * forward;
        targetDirection = targetDirection.normalized * speed;
        targetDirection.y = -1;
        controller.Move(targetDirection);

    }

}
