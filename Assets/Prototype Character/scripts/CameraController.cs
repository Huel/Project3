using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float rotationSpeed = 10;
    private Quaternion xCorrection;
    private float lastRotationCheck;
    public float rotationCheck = 0.0f;
    public int obereKameraGrenze = 50;
    public int untereKameraGrenze = -50;
    public bool frozenYDown = false;
    public bool frozenYUp = false;
    // Use this for initialization
    void Start()
    {
        xCorrection = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);
        if ((Input.GetAxis("Mouse Y") <= 0f && !frozenYDown) || (Input.GetAxis("Mouse Y") >= 0f && !frozenYUp) || transform.FindChild("cameraPivot").localEulerAngles.x < untereKameraGrenze || transform.FindChild("cameraPivot").localEulerAngles.x > obereKameraGrenze)
        {
            rotationCheck += Input.GetAxis("Mouse Y") * rotationSpeed;
            if (rotationCheck > untereKameraGrenze && rotationCheck < obereKameraGrenze)
            {
                transform.FindChild("cameraPivot").transform.Rotate(Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0);
            }
            else
            {
                rotationCheck -= Input.GetAxis("Mouse Y") * rotationSpeed;
            }

            if (rotationCheck == 0 || (lastRotationCheck < 0 && rotationCheck > 0) || (lastRotationCheck > 0 && rotationCheck < 0))
            {
                transform.FindChild("cameraPivot").transform.localRotation = xCorrection;
                rotationCheck = 0;
            }
            lastRotationCheck = rotationCheck;
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
}
