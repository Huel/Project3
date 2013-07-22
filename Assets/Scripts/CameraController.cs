using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float _zeldaDistanceAway = 10.0f;
    [SerializeField]
    private float _zeldaDistanceUp = 5.0f;
    [SerializeField]
    private float _cameraMovementSpeed = 2.0f;
    [SerializeField]
    private float _cameraRotationSpeed = 100.0f;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _cameraDestination;


    private GameObject limitRotation;
    private bool moveTowardsZelda = false;


    void Start()
    {
        limitRotation = new GameObject();
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
            if (Mathf.Abs((_player.position - transform.position).magnitude) > _zeldaDistanceAway)
                transform.position = Vector3.Lerp(transform.position, _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway, Time.deltaTime * _cameraMovementSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _cameraDestination.position, Time.deltaTime * _cameraMovementSpeed);
        }

        limitRotation.transform.position = transform.position;
        limitRotation.transform.rotation = transform.rotation;
        limitRotation.transform.LookAt(_player);

        transform.rotation = Quaternion.Slerp(transform.rotation, limitRotation.transform.rotation, Time.deltaTime * _cameraRotationSpeed);
    }
}