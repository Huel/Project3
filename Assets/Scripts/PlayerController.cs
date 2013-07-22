using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 0.15f;
    [SerializeField]
    private Transform _camera;

    private float speed = 0.0f;
    private float horizontal = 0.0f;
    private float vertical = 0.0f;
    private string lookDirection = "";
    private Vector3 forward;
    private bool runningBack = false;
    private Vector3 targetDirection;

    // Use this for initialization
    void Start()
    {
        forward = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayer();
        MovePlayer();
    }

    void LateUpdate()
    {
        //MovePlayer();
    }

    private void MovePlayer()
    {

        if (Input.GetAxisRaw("leftanalogY") > 0.0f)
        {
            runningBack = false;
        }

        if (_camera.localEulerAngles.x == 90)
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
        //else Debug.Log("RUNNINGBACK TRUE");
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
        transform.GetComponent<CharacterController>().Move(targetDirection);
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        targetDirection.y = 0;
        transform.LookAt(targetDirection + transform.position);
    }
}
