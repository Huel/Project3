using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public float defaultSpeed = 1;
    //public float defaultRotationSpeed = 4;
    //private Quaternion xCorrection;
    //private float lastRotationCheck;
    //private float rotationCheck = 0.0f;
    //public int defaultObereKameraGrenze = 50;
    //public int defaultUntereKameraGrenze = -50;
    //private bool _frozenYDown = false;
    //private bool _frozenYUp = false;
    //public int selectedCameraBehaviour = 0;
    //public float zeldaSpeed = 1;
    private GameObject player;
    //private float zeldaCameraDistance;
    private bool moveTowardsZelda = false;
    //private Vector3 towardsZelda;
    [SerializeField]
    private float _zeldaDistanceAway;
    [SerializeField]
    private float _zeldaDistanceUp;
    [SerializeField]
    private float _zeldaSmooth;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _zeldaCameraSpeed;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private Transform _cameraDestination;

    private float speed = 0.0f;
    private float direction = 0.0f;
    private bool abuttonKlicked = false;

    void Start()
    {
        _cameraDestination.position = _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway;
    }

    private void LateUpdate()
    {
        if (Input.GetAxis("leftanalogY") != 0.0f || Input.GetAxis("leftanalogX") != 0.0f)
        {
            moveTowardsZelda = false;
        }
        if (Input.GetButtonDown("AButton"))
        {
            moveTowardsZelda = true;
        }

        if (!moveTowardsZelda)
        {
            if (Value((_player.position - transform.position).magnitude) > _zeldaDistanceAway)
                transform.position = Vector3.Lerp(transform.position, _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway, Time.deltaTime * _zeldaSmooth);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _cameraDestination.position, Time.deltaTime * _zeldaSmooth);
        }

        transform.LookAt(_player);
    }


    float Value(float Input)
    {
        if (Input < 0)
            return -Input;
        return Input;
    }

    //void LateUpdate()
    //{
    //    //switch (selectedCameraBehaviour)
    //    //{
    //    //    case 0:
    //    //        //transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = false;
    //    //        DefaultCameraLogic();
    //    //        break;
    //    //    case 1:
    //    //        //transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = true;
    //    //        ZeldaCameraLogic();
    //    //        break;
    //    //    default:
    //    //        //transform.FindChild("cameraCorrection").GetComponent<CharacterController>().enabled = false;
    //    //        DefaultCameraLogic();
    //    //        break;
    //    //}
    //}

    //public bool FreezeYDown
    //{
    //    set
    //    {
    //        _frozenYDown = value;
    //    }

    //    get
    //    {
    //        return _frozenYDown;
    //    }
    //}

    //public bool FreezeYUp
    //{
    //    set
    //    {
    //        _frozenYUp = value;
    //    }

    //    get
    //    {
    //        return _frozenYUp;
    //    }
    //}

    //private void RotatePlayer()
    //{

    //    if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") == 0f) //forward
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 180, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") == 0f) //backwards
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") > 0f) //right
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 270, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") == 0f && Input.GetAxisRaw("leftanalogX") < 0f) //left
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 90, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") > 0f)//forwardright
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 225, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") > 0f && Input.GetAxisRaw("leftanalogX") < 0f)//forwardleft
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 135, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") > 0f)//backwardsright
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 315, player.transform.localEulerAngles.z);
    //    }

    //    if (Input.GetAxisRaw("leftanalogY") < 0f && Input.GetAxisRaw("leftanalogX") < 0f)//backwardsleft
    //    {
    //        player.transform.localEulerAngles = new Vector3(player.transform.localEulerAngles.x, transform.FindChild("cameraCorrection").localEulerAngles.y + 45, player.transform.localEulerAngles.z);
    //    }
    //}

    //private void ZeldaCameraLogic()
    //{
    //    StickToWorldSpace(player.transform.parent, transform.FindChild("cameraCorrection"), ref direction, ref speed);

    //    RotatePlayer();

    //    Vector3 forward = player.transform.parent.TransformDirection(Vector3.forward);

    //    forward.y = 0;
    //    forward.Normalize();
    //    Vector3 right = new Vector3(forward.z, 0, -forward.x);

    //    float v = Input.GetAxisRaw("leftanalogY");
    //    float h = Input.GetAxisRaw("leftanalogX");

    //    Vector3 targetDirection = h * right + v * forward;
    //    targetDirection = targetDirection.normalized * _zeldaCameraSpeed;

    //    targetDirection.y = -1;
    //    player.transform.parent.GetComponent<CharacterController>().Move(targetDirection);


    //    targetPosition = _player.position + _player.up * _zeldaDistanceUp - _player.forward * _zeldaDistanceAway;
    //    transform.FindChild("cameraCorrection").position = Vector3.Lerp(transform.FindChild("cameraCorrection").position, targetPosition, Time.deltaTime * _zeldaSmooth);
    //    transform.FindChild("cameraCorrection").LookAt(_player);
    //}

    //public void StickToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut)
    //{
    //    Vector3 rootDirection = root.forward;
    //    Vector3 stickDirection = new Vector3(Input.GetAxisRaw("leftanalogX"), 0, Input.GetAxisRaw("leftanalogY"));
    //    speedOut = stickDirection.sqrMagnitude;

    //    Vector3 CameraDirection = camera.forward;
    //    CameraDirection.y = 0.0f;
    //    Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

    //    Vector3 moveDirection = referentialShift * stickDirection;
    //    Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

    //    float angleRootMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
    //    angleRootMove /= 180f;
    //    directionOut = angleRootMove * directionSpeed;
    //}

    //private void DefaultCameraLogic()
    //{   //0,1.88,-6 default distance between camerapivot and camera
    //    //camera direction
    //    Vector3 forward = transform.FindChild("cameraCorrection").FindChild("Camera").TransformDirection(Vector3.forward);
    //    //doesn't need y
    //    forward.y = 0;
    //    forward.Normalize();
    //    Vector3 right = new Vector3(forward.z, 0, -forward.x);

    //    float v = Input.GetAxisRaw("leftanalogY");

    //    Vector3 targetDirection = v * forward;
    //    targetDirection = targetDirection.normalized * defaultSpeed;
    //    targetDirection.y = -1;
    //    GetComponent<CharacterController>().Move(targetDirection);

    //    transform.parent.Rotate(0, Input.GetAxis("rightanalogX") * defaultRotationSpeed, 0);
    //    transform.parent.Rotate(0, Input.GetAxis("leftanalogX") * defaultRotationSpeed, 0);
    //    if ((Input.GetAxis("rightanalogY") <= 0f && !_frozenYDown) || (Input.GetAxis("rightanalogY") >= 0f && !_frozenYUp) || transform.localEulerAngles.x < defaultUntereKameraGrenze || transform.localEulerAngles.x > defaultObereKameraGrenze)
    //    {
    //        rotationCheck += Input.GetAxis("rightanalogY") * defaultRotationSpeed;
    //        if (rotationCheck > defaultUntereKameraGrenze && rotationCheck < defaultObereKameraGrenze)
    //        {
    //            transform.parent.Rotate(Input.GetAxis("rightanalogY") * defaultRotationSpeed, 0, 0);
    //        }
    //        else
    //        {
    //            rotationCheck -= Input.GetAxis("rightanalogY") * defaultRotationSpeed;
    //        }

    //        if (rotationCheck == 0 || (lastRotationCheck < 0 && rotationCheck > 0) || (lastRotationCheck > 0 && rotationCheck < 0))
    //        {
    //            transform.parent.localRotation = xCorrection;
    //            rotationCheck = 0;
    //        }
    //        lastRotationCheck = rotationCheck;
    //    }
    //    transform.FindChild("cameraCorrection").GetComponent<CameraClippingCorrection>().CorrectCameraClipping();
    //}

    //
}