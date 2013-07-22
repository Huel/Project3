using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool moveTowardsZelda = false;
    [SerializeField]
    private float _zeldaDistanceAway = 10.0f;
    [SerializeField]
    private float _zeldaDistanceUp = 5.0f;
    [SerializeField]
    private float _cameraLerpSpeed = 2.0f;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _cameraDestination;

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
                transform.position = Vector3.Lerp(transform.position, _player.position + Vector3.up * _zeldaDistanceUp - _player.FindChild("Player").forward * _zeldaDistanceAway, Time.deltaTime * _cameraLerpSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _cameraDestination.position, Time.deltaTime * _cameraLerpSpeed);
        }

        transform.LookAt(_player);
    }


    float Value(float Input)
    {
        if (Input < 0)
            return -Input;
        return Input;
    }
}