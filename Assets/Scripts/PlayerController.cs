using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 9.0f; //speed of player, will be multiplied by deltatime
    [SerializeField]
    private Transform _camera;

    private Vector3 forward;
    private bool runningBack = false;
    private Vector3 targetDirection;

    void Start()
    {
        forward = new Vector3(0f, 0f, 0f);
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {

        if (Input.GetAxisRaw("leftanalogY") > 0.0f) //bugifx, if you are running back and push the stick forwards, the player will rotate 180° on each frame while under camera otherwise
        {
            runningBack = false;
        }

        if (_camera.localEulerAngles.x == 90) //if camera looking down
        {
            runningBack = true;
        }

        if (!runningBack)
        {
            forward = _camera.forward;
        }

        if (runningBack && _camera.localEulerAngles.x != 90)
        {
            forward = -_camera.forward;
        }

        if (runningBack && Input.GetAxisRaw("leftanalogY") == 0.0f)
        {
            runningBack = false;
        }

        forward.y = 0;
        forward.Normalize();

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("leftanalogY");
        float h = Input.GetAxisRaw("leftanalogX");

        if (runningBack)
        {
            h *= -1;
        }

        targetDirection = h * right + v * forward;
        targetDirection = targetDirection.normalized * _movementSpeed;
        targetDirection.y = -1;
        transform.GetComponent<CharacterController>().Move(targetDirection * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        targetDirection.y = 0;
        transform.LookAt(targetDirection + transform.position);
    }
}