using UnityEngine;

public class IsBackZoomAllowed : MonoBehaviour
{
    private int CollisionCounter = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CollisionCounter++;
            transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            CollisionCounter--;
            if (CollisionCounter == 0)
                transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = true;
        }
    }
}
