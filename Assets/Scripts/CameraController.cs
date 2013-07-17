using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float rotationSpeed = 4;
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

    public void FreezeYDown(bool value)
    {
        frozenYDown = value;
    }

    public void FreezeYUp(bool value)
    {
        frozenYUp = value;
    }
}
