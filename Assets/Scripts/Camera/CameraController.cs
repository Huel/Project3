using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _zeldaDistanceAway = 10.0f; //Distance camera to player on z axis
    [SerializeField]
    private float _zeldaDistanceUp = 5.0f; //Distance camera to player on y axis
    [SerializeField]
<<<<<<< HEAD:Assets/Scripts/Camera/CameraController.cs
    private float _cameraMovementSpeed = 2.0f;
    [SerializeField]
    private float _cameraRotationSpeed = 100.0f;
=======
    private float _cameraMovementSpeed = 2.0f; //speed of camera, will be multiplied by deltatime
    [SerializeField]
    private float _cameraRotationSpeed = 100.0f; //rotation speed of camera, will be multiplied by deltatime
>>>>>>> refs/heads/feature/camera:Assets/Scripts/CameraController.cs
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _initialCameraPosition; //an empty gameobject which is a child of player


    private GameObject limitRotation; //transform proxy for interpolation of position and rotation of player
    private bool _towardsInitialPosition = false;



    private GameObject limitRotation;
    private bool moveTowardsZelda = false;


    void Start()
    {
        limitRotation = new GameObject();
<<<<<<< HEAD:Assets/Scripts/Camera/CameraController.cs
        _cameraDestination.position = _player.position + Vector3.up * _zeldaDistanceUp - _player.forward * _zeldaDistanceAway;
=======
        _initialCameraPosition.position = _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway;
>>>>>>> refs/heads/feature/camera:Assets/Scripts/CameraController.cs
    }

    private void FixedUpdate()
    {

        if (Input.GetAxis("leftanalogY") != 0.0f || Input.GetAxis("leftanalogX") != 0.0f)
        {
            _towardsInitialPosition = false;
        }
        if (Input.GetButtonDown("AButton"))
        {
            _towardsInitialPosition = true;
        }

        if (!_towardsInitialPosition)
        {
            if (Mathf.Abs((_player.position - transform.position).magnitude) > _zeldaDistanceAway)
<<<<<<< HEAD:Assets/Scripts/Camera/CameraController.cs
                transform.position = Vector3.Lerp(transform.position, _player.position + Vector3.up * _zeldaDistanceUp - _player.forward * _zeldaDistanceAway, Time.deltaTime * _cameraMovementSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _cameraDestination.position, Time.deltaTime * _cameraMovementSpeed);
=======
                transform.GetComponent<CharacterController>().Move(Vector3.Lerp(transform.position, _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway, Time.deltaTime * _cameraMovementSpeed) - transform.position);
        }
        else
        {
            transform.GetComponent<CharacterController>().Move(Vector3.Lerp(transform.position, _initialCameraPosition.position, Time.deltaTime * _cameraMovementSpeed) - transform.position);
>>>>>>> refs/heads/feature/camera:Assets/Scripts/CameraController.cs
        }

        limitRotation.transform.position = transform.position;
        limitRotation.transform.rotation = transform.rotation;
        limitRotation.transform.LookAt(_player);

        transform.rotation = Quaternion.Slerp(transform.rotation, limitRotation.transform.rotation, Time.deltaTime * _cameraRotationSpeed);
    }
}