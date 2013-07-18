using UnityEngine;

public class CameraClippingCorrection : MonoBehaviour
{   //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------don't ignore this
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------
    //---------------->CAMERA COLLIDES ONLY WITH LAYER 8 RIGHT NOW!!!!! <--------------------if you want to change it, search for all instances of gameObject.layer here and in IsBackZoomAllowed.

    public float ZoomStep = 0.03f;
    private float preserveZoomStep;
    private float ZoomStepForward = 0.3f;
    private bool isColliding = false;
    private bool _leavingZoomAllowed = true;
    private Vector3 newPos;
    private float RotationCheck;
    private float DestinationY;
    private float DestinationZ;
    private float DestinationRotationX;
    private int collisionObjectsCounter = 0;
    private bool collisionLastFrame = false;

    public bool LeavingZoomAllowed
    {
        get
        {
            return _leavingZoomAllowed;
        }

        set
        {
            _leavingZoomAllowed = value;
        }
    }

    void Start()
    {
        preserveZoomStep = ZoomStep;
        DestinationY = transform.localPosition.y;
        DestinationZ = transform.localPosition.z;
        DestinationRotationX = transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void correctCameraClipping()
    {
        if (transform.parent.localEulerAngles.x < 180)
        {
            transform.parent.parent.GetComponent<CameraController>().FreezeYDown = false;
        }
        if (transform.parent.localEulerAngles.x > 180)
        {
            transform.parent.parent.GetComponent<CameraController>().FreezeYUp = false;
        }
        if (transform.localPosition.y - ZoomStep >= 0f && (isColliding || transform.parent.parent.GetComponent<CameraController>().FreezeYDown || transform.parent.parent.GetComponent<CameraController>().FreezeYUp))
        {
            transform.Translate(0f, -ZoomStep, 0f, Space.Self);//falls du kollidierst, zoom an kamera pivot heran
        }

        if (transform.localPosition.z + ZoomStep * (MakePositive(DestinationZ) / DestinationY) <= 0f && (isColliding || transform.parent.parent.GetComponent<CameraController>().FreezeYDown || transform.parent.parent.GetComponent<CameraController>().FreezeYUp))
        {
            transform.Translate(0f, 0f, ZoomStep * (MakePositive(DestinationZ) / DestinationY), Space.Self);//falls du kollidierst, zoom an kamera pivot heran
        }

        if (!isColliding
            && LeavingZoomAllowed
            && transform.localPosition.y + ZoomStep <= DestinationY)
        {
            transform.Translate(0f, ZoomStep, 0f, Space.Self);//falls du nicht kollidierst, zoom von kamera pivot weg
        }

        if (!isColliding
            && LeavingZoomAllowed
            && transform.localPosition.z - ZoomStep * (MakePositive(DestinationZ) / DestinationY) >= DestinationZ)
        {
            transform.Translate(0f, 0f, -ZoomStep * (MakePositive(DestinationZ) / DestinationY), Space.Self);//falls du nicht kollidierst, zoom von kamera pivot weg
        }

        gameObject.GetComponent<BoxCollider>().size = new Vector3(gameObject.GetComponent<BoxCollider>().size.x,
                                                                  gameObject.GetComponent<BoxCollider>().size.y,
                                                                  Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f))) / 2);
        gameObject.GetComponent<BoxCollider>().center = new Vector3(0, 0, gameObject.GetComponent<BoxCollider>().size.z * 0.66f - 0.5f);
        transform.Rotate(-transform.localEulerAngles.x, 0, 0, Space.Self);
        transform.Rotate(DestinationRotationX + Mathf.Acos(MakePositive(transform.localPosition.z) / Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f)))), 0, 0, Space.Self);

        //transform.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0.17f);
        collisionLastFrame = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            collisionObjectsCounter++;
            collisionLastFrame = true;
        }
        if (!isColliding)
        {
            isColliding = other.gameObject.layer == 8;
            if (transform.parent.eulerAngles.x < 180 && isColliding)
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYUp = isColliding;
            }
            else
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYDown = isColliding;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            collisionObjectsCounter--;
        }
        if (isColliding)
        {
            isColliding = !(other.gameObject.layer == 8 && collisionObjectsCounter == 0);
            if (transform.parent.eulerAngles.x < 180 && !isColliding)
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYUp = isColliding;
            }
            else
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYDown = isColliding;
            }
        }
    }

    float MakePositive(float Input)
    {
        if (Input < 0)
            return -Input;
        return Input;
    }
}
