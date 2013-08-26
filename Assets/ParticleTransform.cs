using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ParticleTransform : MonoBehaviour
{
    public bool mirrored = false;
    [SerializeField]
    public Quaternion _originalRotation;
	[ExecuteInEditMode]
	void Update () 
    {
        if (!mirrored && transform.parent.parent.localScale.x <0)
        {
            MirrorX();
            mirrored = true;
        }
        else if (mirrored && transform.parent.parent.localScale.x >= 0)
        {
            mirrored = false;
            transform.localRotation = _originalRotation;
            _originalRotation = Quaternion.identity;
        }
	    enabled = false;

    }

    void MirrorX()
    {
        Vector3 container = -transform.parent.parent.forward;
        Vector3 direction = transform.forward;
        container.y = 0;
        direction.y = 0;
        float angle = Vector3.Angle(direction, container);
        _originalRotation = transform.localRotation;
        transform.Rotate(Vector3.up,angle*2);

    }
}
