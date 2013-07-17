using UnityEngine;

public class IsBackZoomAllowed : MonoBehaviour
{
    public int collisionCounter = 0;
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
            collisionCounter++;
            transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = false;
            Debug.Log("CameraCorrection2 kollidiert jetzt mit layer 8");
            Debug.Log(Random.Range(0, 10000));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            collisionCounter--;
            if (collisionCounter == 0)
                transform.parent.GetComponent<CameraClippingCorrection>().LeavingZoomAllowed = true;
        }
    }
}
