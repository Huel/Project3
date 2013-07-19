using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float _directionDampTime = 0.25f;
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
        targetDirection.y = 0;
        transform.LookAt(targetDirection + transform.position);

        //Vector3 forward = transform.FindChild("Player").TransformDirection(Vector3.forward);

        //forward.y = 0;
        //forward.Normalize();

        //float v = Input.GetAxisRaw("leftanalogY");
        //float h = Input.GetAxisRaw("leftanalogX");


        //forward = forward.normalized * _movementSpeed;

        //forward.y = -1;

        //if (v != 0f || h != 0f)
        //{
        //    transform.GetComponent<CharacterController>().Move(forward);
        //}
    }

    private void RotatePlayer()
    {

        //if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") == 0f && lookDirection != "forward") //forward
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 0f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 180f, 0f, Space.Self);
        //    lookDirection = "forward";
        //}

        //if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") == 0f && lookDirection != "backward") //backwards
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 180f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 0f, 0f, Space.Self);
        //    lookDirection = "backward";
        //}

        //if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") > 0f && lookDirection != "right") //right
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 90f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 270f, 0f, Space.Self);
        //    lookDirection = "right";
        //}

        //if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") < 0f && lookDirection != "left") //left
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 270f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 90f, 0f, Space.Self);
        //    lookDirection = "left";
        //}

        //if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") > 0f && lookDirection != "forwardright")//forwardright
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 45f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 225f, 0f, Space.Self);
        //    lookDirection = "forwardright";
        //}

        //if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") < 0f && lookDirection != "forwardleft")//forwardleft
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 315f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 135f, 0f, Space.Self);
        //    lookDirection = "forwardleft";
        //}

        //if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") > 0f && lookDirection != "backwardright")//backwardsright
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 135f, transform.FindChild("Player").localEulerAngles.z);
        //    //transform.FindChild("Player").Rotate(0f, 315f, 0f, Space.Self);
        //    lookDirection = "backwardright";
        //}

        //if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") < 0f && lookDirection != "backwardleft")//backwardsleft
        //{
        //    transform.FindChild("Player").localEulerAngles = new Vector3(transform.FindChild("Player").localEulerAngles.x, 225f, transform.FindChild("Player").localEulerAngles.z); //_camera.localEulerAngles.y + 
        //    //transform.FindChild("Player").Rotate(0f, 45f, 0f, Space.Self);
        //    lookDirection = "backwardleft";
        //}
    }
}
