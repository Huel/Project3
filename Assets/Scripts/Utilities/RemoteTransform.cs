using UnityEngine;

[ExecuteInEditMode]
public class RemoteTransform : MonoBehaviour
{
	public Transform remoteTransform;
	public bool position;
	public bool rotation;
	public bool scaling;
	
	void Update ()
	{
		if(!remoteTransform)
			return;
		if(position)
			transform.position = remoteTransform.position;
		if (rotation)
			transform.rotation = remoteTransform.rotation;
		if (scaling)
			transform.localScale = remoteTransform.localScale;
	}
}
