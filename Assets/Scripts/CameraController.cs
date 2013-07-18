using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1;
    public float rotationSpeed = 4;
    private Quaternion xCorrection;
    private float lastRotationCheck;
    private float rotationCheck = 0.0f;
    public int obereKameraGrenze = 50;
    public int untereKameraGrenze = -50;
    private bool _frozenYDown = false;
    private bool _frozenYUp = false;
    public int selectedCameraBehaviour = 0;
    public float zeldaSpeed = 1;
    private GameObject player;
    private float zeldaCameraDistance;
    private bool moveTowardsZelda = false;
    private Vector3 towardsZelda;

    void Start()
    {
        towardsZelda = new Vector3(0f, 0f, 0f);
        player = GameObject.Find("Player");
        Vector3 abc = player.transform.position - player.transform.FindChild("zeldaCameraHere").position;
        abc.y = 0f;
        zeldaCameraDistance = MakePositive(abc.magnitude);
        xCorrection = Quaternion.identity;
    }

    void Update()
    {
        switch (selectedCameraBehaviour)
        {
            case 0:
                transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = false;
                DefaultCameraLogic();
                break;
            case 1:
                transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = true;
                ZeldaCameraLogic();
                break;
            default:
                transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = false;
                DefaultCameraLogic();
                break;
        }
    }

    public bool FreezeYDown
    {
        set
        {
            _frozenYDown = value;
        }

        get
        {
            return _frozenYDown;
        }
    }

    public bool FreezeYUp
    {
        set
        {
            _frozenYUp = value;
        }

        get
        {
            return _frozenYUp;
        }
    }

    private void RotatePlayer()
    {

        if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") == 0f) //forward
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 180, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") == 0f) //backwards
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") > 0f) //right
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 270, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") < 0f) //left
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 90, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") > 0f)//forwardright
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 225, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") < 0f)//forwardleft
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 135, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") > 0f)//backwardsright
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 315, player.transform.localEulerAngles.z);
        }

        if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") < 0f)//backwardsleft
        {
            player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 45, player.transform.localEulerAngles.z);
        }
    }

    private void ZeldaCameraLogic()
    {
        player = GameObject.Find("Player");

        RotatePlayer();

        Vector3 forward = transform.FindChild("cameraCorrection").TransformDirection(Vector3.forward);

        forward.y = 0;
        forward.Normalize();
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("leftanalogY");
        float h = Input.GetAxisRaw("leftanalogX");

        Vector3 targetDirection = h * right + v * forward;
        targetDirection = targetDirection.normalized * speed;


        targetDirection.y = -1;
        player.GetComponent<CharacterController>().Move(targetDirection);

        Vector3 distanceToPlayer = player.transform.position - transform.FindChild("cameraCorrection").position;
        distanceToPlayer.y = 0f;
        if (MakePositive(distanceToPlayer.magnitude) > zeldaCameraDistance)
        {
            transform.FindChild("cameraCorrection").position = player.transform.FindChild("zeldaCameraHere").position;
            moveTowardsZelda = false;
        }

        if (Input.GetButtonDown("AButton"))
        {
            moveTowardsZelda = true;
            towardsZelda = (player.transform.FindChild("zeldaCameraHere").position -
                                    transform.FindChild("cameraCorrection").position) * 0.01f;
        }

        if (moveTowardsZelda)
        {
            transform.FindChild("cameraCorrection").GetComponent<CharacterController>().Move(towardsZelda);
        }

        var relativePos = player.transform.position - transform.FindChild("cameraCorrection").position;
        var rotation = Quaternion.LookRotation(relativePos);
        transform.FindChild("cameraCorrection").rotation = rotation;

    }

    private void DefaultCameraLogic()
    {
        //camera direction
        Vector3 forward = transform.FindChild("cameraCorrection").FindChild("Camera").TransformDirection(Vector3.forward);
        //doesn't need y
        forward.y = 0;
        forward.Normalize();
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("leftanalogY");

        Vector3 targetDirection = v * forward;
        targetDirection = targetDirection.normalized * speed;
        targetDirection.y = -1;
        GetComponent<CharacterController>().Move(targetDirection);

        transform.parent.Rotate(0, Input.GetAxis("rightanalogX") * rotationSpeed, 0);
        transform.parent.Rotate(0, Input.GetAxis("leftanalogX") * rotationSpeed, 0);
        if ((Input.GetAxis("rightanalogY") <= 0f && !_frozenYDown) || (Input.GetAxis("rightanalogY") >= 0f && !_frozenYUp) || transform.localEulerAngles.x < untereKameraGrenze || transform.localEulerAngles.x > obereKameraGrenze)
        {
            rotationCheck += Input.GetAxis("rightanalogY") * rotationSpeed;
            if (rotationCheck > untereKameraGrenze && rotationCheck < obereKameraGrenze)
            {
                transform.parent.Rotate(Input.GetAxis("rightanalogY") * rotationSpeed, 0, 0);
            }
            else
            {
                rotationCheck -= Input.GetAxis("rightanalogY") * rotationSpeed;
            }

            if (rotationCheck == 0 || (lastRotationCheck < 0 && rotationCheck > 0) || (lastRotationCheck > 0 && rotationCheck < 0))
            {
                transform.parent.localRotation = xCorrection;
                rotationCheck = 0;
            }
            lastRotationCheck = rotationCheck;
        }
        transform.FindChild("cameraCorrection").GetComponent<CameraClippingCorrection>().CorrectCameraClipping();
    }

    float MakePositive(float Input)
    {
        if (Input < 0)
            return -Input;
        return Input;
    }
}
