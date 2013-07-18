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
    public float zeldaCameraMaxDistance = 10;
    private GameObject Player;

    void Start()
    {
        xCorrection = Quaternion.identity;
    }

    void Update()
    {
        switch (selectedCameraBehaviour)
        {
            case 0:
                DefaultCameraLogic();
                break;
            case 1:
                ZeldaCameraLogic();
                break;
            default:
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

    private void ZeldaCameraLogic()
    {
        Player = GameObject.Find("Player");
        if (Input.GetAxis("leftanalogX") > 0)
        {
            Vector3 right = transform.FindChild("cameraPivot").FindChild("cameraCorrection").FindChild("Camera").TransformDirection(Vector3.right);
            right.y = 0;
            right.Normalize();
            right = new Vector3(right.z, 0, -right.x);
            float v = Input.GetAxis("leftanalogY");
            Vector3 targetDirection = v * right;
            targetDirection = targetDirection.normalized * speed;
            Player.GetComponent<CharacterController>().Move(targetDirection);


            var relativePos = Player.transform.position - transform.position;
            var rotation = Quaternion.LookRotation(relativePos);
            rotation.y = transform.localEulerAngles.y;
            transform.rotation = rotation;
        }
    }

    private void DefaultCameraLogic()
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
        GetComponent<CharacterController>().Move(targetDirection);

        transform.Rotate(0, Input.GetAxis("rightanalogX") * rotationSpeed, 0);
        transform.Rotate(0, Input.GetAxis("leftanalogX") * rotationSpeed, 0);
        if ((Input.GetAxis("rightanalogY") <= 0f && !_frozenYDown) || (Input.GetAxis("rightanalogY") >= 0f && !_frozenYUp) || transform.FindChild("cameraPivot").localEulerAngles.x < untereKameraGrenze || transform.FindChild("cameraPivot").localEulerAngles.x > obereKameraGrenze)
        {
            rotationCheck += Input.GetAxis("rightanalogY") * rotationSpeed;
            if (rotationCheck > untereKameraGrenze && rotationCheck < obereKameraGrenze)
            {
                transform.FindChild("cameraPivot").transform.Rotate(Input.GetAxis("rightanalogY") * rotationSpeed, 0, 0);
            }
            else
            {
                rotationCheck -= Input.GetAxis("rightanalogY") * rotationSpeed;
            }

            if (rotationCheck == 0 || (lastRotationCheck < 0 && rotationCheck > 0) || (lastRotationCheck > 0 && rotationCheck < 0))
            {
                transform.FindChild("cameraPivot").transform.localRotation = xCorrection;
                rotationCheck = 0;
            }
            lastRotationCheck = rotationCheck;
        }
        transform.FindChild("cameraPivot").FindChild("cameraCorrection").GetComponent<CameraClippingCorrection>().correctCameraClipping();
    }
}
