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

    public float ZoomStep = 0.01f;
    public float ZoomStepForward = 0.3f;
    private bool isColliding = false;
    public bool LeavingZoomAllowed = true;
    private Vector3 newPos;
    private float RotationCheck;
    private float DestinationY;
    private float DestinationZ;
    private float DestinationRotationX;

    void Start()
    {
        DestinationY = transform.localPosition.y;
        DestinationZ = transform.localPosition.z;
        DestinationRotationX = transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.localEulerAngles.x < 180)
        {
            transform.parent.parent.GetComponent<CameraController>().FreezeYDown(false);
        }
        if (transform.parent.localEulerAngles.x > 180)
        {
            transform.parent.parent.GetComponent<CameraController>().FreezeYUp(false);
        }
        if (transform.localPosition.y - ZoomStep >= 0f && (isColliding || transform.parent.parent.GetComponent<CameraController>().frozenYDown || transform.parent.parent.GetComponent<CameraController>().frozenYUp))
        {
            transform.Translate(0f, -ZoomStep, 0f, Space.Self);//falls du kollidierst, zoom an kamera pivot heran
        }

        if (transform.localPosition.z + ZoomStep * (MakePositive(DestinationZ) / DestinationY) <= 0f && (isColliding || transform.parent.parent.GetComponent<CameraController>().frozenYDown || transform.parent.parent.GetComponent<CameraController>().frozenYUp))
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
        //gameObject.GetComponent<BoxCollider>().center = new Vector3(0, 0, gameObject.GetComponent<BoxCollider>().size.z * 0.66f - 0.5f);
        transform.Rotate(-transform.localEulerAngles.x, 0, 0, Space.Self);
        transform.Rotate(DestinationRotationX + Mathf.Acos(MakePositive(transform.localPosition.z) / Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f)))), 0, 0, Space.Self);

        //transform.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0.17f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isColliding)
        {
            isColliding = other.gameObject.layer == 8;
            if (transform.parent.eulerAngles.x < 180 && isColliding)
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYUp(isColliding);
            }
            else
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYDown(isColliding);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isColliding)
        {
            isColliding = !(other.gameObject.layer == 8);
            if (transform.parent.eulerAngles.x < 180 && !isColliding)
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYUp(isColliding);
            }
            else
            {
                transform.parent.parent.GetComponent<CameraController>().FreezeYDown(isColliding);
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
