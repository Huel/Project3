using UnityEngine;
using System.Collections;

public class CameraClippingCorrection : MonoBehaviour 
{
    public float ZoomStep = 0.03f;
    public float ZoomStepForward = 0.3f;
    //private float Offset = 0.0000008f;
    private bool isColliding = false;
    public bool LeavingZoomAllowed = true;
    private Vector3 newPos;
    private float RotationCheck;
    private float DestinationY;
    private float DestinationZ;
    private float DestinationRotationX;
	// Use this for initialization
	void Start () 
    {
        DestinationY = transform.localPosition.y;
        DestinationZ = transform.localPosition.z;
        DestinationRotationX = transform.localEulerAngles.x;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isColliding
            && transform.localPosition.y - ZoomStep >= 0f)
        {
            transform.Translate(0f, -ZoomStep, 0f, Space.Self);//falls du kollidierst, zoom an kamera pivot heran
        }

        if (isColliding
            && transform.localPosition.z + ZoomStep * (MakePositive(DestinationZ) / DestinationY) <= 0f)
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
                                                                  Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f)))/2);
        gameObject.GetComponent<BoxCollider>().center = new Vector3(0, 0, gameObject.GetComponent<BoxCollider>().size.z*0.66f-0.5f);
        transform.Rotate(-transform.localEulerAngles.x,0,0,Space.Self);
        transform.Rotate(DestinationRotationX + Mathf.Acos(MakePositive(transform.localPosition.z) / Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f)))), 0, 0, Space.Self);

        //if (transform.localPosition.x != 0f) transform.Translate(-transform.localPosition.x, 0f, 0f, Space.Self);//falls x != dann x = 0

        //if (transform.localPosition.y <= Offset) { Debug.Log("Old Y =" + transform.localPosition.y); transform.Translate(0f, -transform.localPosition.y + Offset, 0f, Space.Self); Debug.Log("New Y =" + transform.localPosition.y); }
        //if (transform.localPosition.y > 1.25f) transform.Translate(0f, -(transform.localPosition.y - 1.25f), 0f, Space.Self);//y>1.25 dann y=1.25

        //if (transform.localPosition.z >= -Offset) { Debug.Log("Old Z =" + transform.localPosition.z); transform.Translate(0f, -transform.localPosition.z - Offset, 0f, Space.Self); Debug.Log("New Z =" + transform.localPosition.z); }
        //if (transform.localPosition.z < -4.3f) transform.Translate(0f, 0f, -(transform.localPosition.z + 4.3f), Space.Self);//z<-4.3 dann z=-4.3
        //Debug.Log(16.0f+Mathf.Acos(MakePositive(transform.localPosition.z) / Mathf.Sqrt(Mathf.Pow(transform.localPosition.y, 2f) + (Mathf.Pow(transform.localPosition.z, 2f)))));
        Debug.Log(Random.Range(10, 100000));
	}

    void OnTriggerEnter(Collider other)
    {
        if (!isColliding)
        {
            //Debug.Log("Ist mit etwas kollidiert und kollidiert noch nicht mit 8. Ist es tatsächlich 8=" + (other.gameObject.layer == 8));
            isColliding = other.gameObject.layer == 8;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isColliding)
        {
            //Debug.Log("Ein Collider hat die sphere verlassen und es kollidiert mit 8. Ist es tatsächlich 8=" + (other.gameObject.layer == 8));
            isColliding = !(other.gameObject.layer == 8);
        }
    }

    float MakePositive(float Input)
    {
        if(Input<0)
            return -Input;
        return Input;
    }
}
