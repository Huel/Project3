using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 1;
    public float zeldaSpeed = 1;
    public float rotationSpeed = 4;
    private Quaternion xCorrection;
    private float lastRotationCheck;
    private float rotationCheck = 0.0f;
    public int obereKameraGrenze = 50;
    public int untereKameraGrenze = -50;
    private bool frozenYDown = false;
    private bool frozenYUp = false;
    public int selectedCameraBehaviour = 0;
    // Use this for initialization
    void Start()
    {
        xCorrection = Quaternion.identity;
    }

    // Update is called once per frame
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
                break;
        }
    }

    public void FreezeYDown(bool value)
    {
        frozenYDown = value;
    }

    public void FreezeYUp(bool value)
    {
        frozenYUp = value;
    }

    private void ZeldaCameraLogic()
    {

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
        if ((Input.GetAxis("rightanalogY") <= 0f && !frozenYDown) || (Input.GetAxis("rightanalogY") >= 0f && !frozenYUp) || transform.FindChild("cameraPivot").localEulerAngles.x < untereKameraGrenze || transform.FindChild("cameraPivot").localEulerAngles.x > obereKameraGrenze)
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
    }
}
