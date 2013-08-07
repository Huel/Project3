using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Bomb : MonoBehaviour 
{
	public double m_InterpolationBackTime = 0.1;
	public double m_ExtrapolationLimit = 0.5;
	public GameObject WaypointA;
	public GameObject WaypointB;
	private const float forceMultiplier = 0.01f;
	private float movementSpeed;
    private GameObject towardsDestination;
	public List<GameObject> valvesA = new List<GameObject>();
	public List<GameObject> valvesB = new List<GameObject>();

	private float ForceA
	{
		get
		{
			return (valvesA.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier);
		}
	}

	private float ForceB
	{
		get
		{
			return (valvesB.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier);
		}
	}

	private float Speed
	{
		get
		{
			return movementSpeed*Time.deltaTime*(ForceA < ForceB ? ForceB-ForceA : ForceA-ForceB);
		}
	}


	internal struct State {
		internal double timestamp;
		internal Vector3 pos;
		internal Quaternion rot;
	}

	State[] m_BufferedState = new State[20];
	int m_TimestampCount;
    private bool gameOver = false;

    void Start() 
	{
		movementSpeed = float.Parse(new XMLReader("Bomb.xml").GetXML().GetElementsByTagName("speed")[0].InnerText);
        towardsDestination = new GameObject();
	}

	void Update()
	{
		if (!Network.isServer || ForceA - ForceB == 0f || gameOver) return;

		if (ForceA > ForceB)
		{
			if (!HaveReached(WaypointB))
			{
                WaypointB.transform.position = new Vector3(WaypointB.transform.position.x, transform.position.y, WaypointB.transform.position.z);
				transform.position += Vector3.ClampMagnitude(WaypointB.transform.position - transform.position, Speed);
			    towardsDestination.transform.position = transform.position;
                towardsDestination.transform.LookAt(transform.position + transform.position - WaypointB.transform.position);
			    transform.rotation = Quaternion.Slerp(transform.rotation, towardsDestination.transform.rotation, 0.01f);
			}
			else
			{
				WaypointA = WaypointB;
				WaypointB = WaypointB.GetComponent<BombWaypoint>().WaypointB;
				if (WaypointB == null)
				{
					networkView.RPC("GameOver", RPCMode.OthersBuffered);
					GameOver();
				}
			}
		}
		else
		{
			if (!HaveReached(WaypointA))
			{
                WaypointA.transform.position = new Vector3(WaypointA.transform.position.x, transform.position.y, WaypointA.transform.position.z);
                transform.position += Vector3.ClampMagnitude(WaypointA.transform.position - transform.position, Speed);
                towardsDestination.transform.position = transform.position;
                towardsDestination.transform.LookAt(WaypointA.transform);
                transform.rotation = Quaternion.Slerp(transform.rotation, towardsDestination.transform.rotation, 0.01f);
			}
			else
			{
				WaypointB = WaypointA;
				WaypointA = WaypointA.GetComponent<BombWaypoint>().WaypointA;
				if (WaypointA == null)
				{
					networkView.RPC("GameOver", RPCMode.OthersBuffered);
					GameOver();
				}
			}
		}
	}

	[RPC]
	private void GameOver()
	{
	    gameOver = true;
		throw new System.NotImplementedException();
	}

	private bool HaveReached(GameObject target)
	{
		return Mathf.Abs((target.transform.position-transform.position).magnitude)<0.5f;
	}

	public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		//GetComponent<ObjectInfo>().OnSerializeNetworkView(stream,info);
		// Send data to server
		if (stream.isWriting) {
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;

			//Vector3 angularVelocity = rigidbody.angularVelocity;

			stream.Serialize(ref pos);

			stream.Serialize(ref rot);
			//stream.Serialize(ref angularVelocity);
		}
			// Read data from remote client
		else {
			Vector3 pos = Vector3.zero;

			Quaternion rot = Quaternion.identity;
			//Vector3 angularVelocity = Vector3.zero;
			stream.Serialize(ref pos);

			stream.Serialize(ref rot);
			//stream.Serialize(ref angularVelocity);

			// Shift the buffer sideways, deleting state 20
			for (int i = m_BufferedState.Length - 1; i >= 1; i--) {
				m_BufferedState[i] = m_BufferedState[i - 1];
			}

			// Record current state in slot 0
			State state = new State();
			state.timestamp = info.timestamp;
			state.pos = pos;

			state.rot = rot;
			//state.angularVelocity = angularVelocity;
			m_BufferedState[0] = state;

			// Update used slot count, however never exceed the buffer size
			// Slots aren't actually freed so this just makes sure the buffer is
			// filled up and that uninitalized slots aren't used.
			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

			// Check if states are in order, if it is inconsistent you could reshuffel or 
			// drop the out-of-order state. Nothing is done here
			for (int i = 0; i < m_TimestampCount - 1; i++) {
				if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
					Debug.Log("State inconsistent");
			}
		}
	}

	void LateUpdate() {
		if (networkView.isMine)
			return;

		// This is the target playback time of the rigid body
		double interpolationTime = Network.time - m_InterpolationBackTime;

		// Use interpolation if the target playback time is present in the buffer
		if (m_BufferedState[0].timestamp > interpolationTime) {
			// Go through buffer and find correct state to play back
			for (int i = 0; i < m_TimestampCount; i++) {
				if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount - 1) {
					// The state one slot newer (<100ms) than the best playback state
					State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];
					// The best playback state (closest to 100 ms old (default time))
					State lhs = m_BufferedState[i];

					// Use the time between the two slots to determine if interpolation is necessary
					double length = rhs.timestamp - lhs.timestamp;
					float t = 0.0F;
					// As the time difference gets closer to 100 ms t gets closer to 1 in 
					// which case rhs is only used
					// Example:
					// Time is 10.000, so sampleTime is 9.900 
					// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
					// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
					if (length > 0.0001)
						t = (float)((interpolationTime - lhs.timestamp) / length);

					// if t=0 => lhs is used directly
					transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
					transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
					return;
				}
			}
		}
			// Use extrapolation
		else {
			State latest = m_BufferedState[0];

			float extrapolationLength = (float)(interpolationTime - latest.timestamp);
			// Don't extrapolation for more than 500 ms, you would need to do that carefully
			if (extrapolationLength < m_ExtrapolationLimit) {
				//float axisLength = extrapolationLength * latest.angularVelocity.magnitude * Mathf.Rad2Deg;
				//Quaternion angularRotation = Quaternion.AngleAxis(axisLength, latest.angularVelocity);
				Quaternion angularRotation = Quaternion.identity;

				transform.localPosition = latest.pos + new Vector3(0f,0f,extrapolationLength);
				transform.localRotation = angularRotation * latest.rot;
				//charctrl.velocity = latest.velocity;
				//rigidbody.angularVelocity = latest.angularVelocity;
			}
		}
	}
}
