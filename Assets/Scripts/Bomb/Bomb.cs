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
    private GameObject target;
    private bool[] checkTriggeredSound = new bool[3];

    public int explodeTeam = 0;
    private float explosiontimer = 0;
    private GameObject currentBase;
    private GameObject current_fw1;
    private GameObject current_fw2;

    public GameObject explosionBase1;
    public GameObject firework_base1_1;
    public GameObject firework_base1_2;
    public GameObject explosionBase2;
    public GameObject firework_base2_1;
    public GameObject firework_base2_2;

    private float ForceA
    {
        get { return (valvesA.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier); }
    }

    private float ForceB
    {
        get { return (valvesB.Sum(valve => valve.GetComponent<Valve>().State) * forceMultiplier); }
    }

    private float Speed
    {
        get { return movementSpeed * Time.deltaTime * (ForceA < ForceB ? ForceB - ForceA : ForceA - ForceB); }
    }

    private bool BombDoesntMove
    {
        get { return ForceA - ForceB == 0f; }
    }

    internal struct State
    {
        internal double timestamp;
        internal Vector3 pos;
        internal Quaternion rot;
    }

    State[] m_BufferedState = new State[20];
    int m_TimestampCount;
    private bool gameOver = false;
    private XmlDocument document;
    private AudioLibrary soundLibrary;
    private AudioLibrary soundLibraryCamera;

    public bool GameOver
    {
        get { return gameOver; }
    }

    void Start()
    {
        soundLibrary = transform.FindChild("sound_bomb").FindChild("sounds_SFX").GetComponent<AudioLibrary>();
        document = new XMLReader("GameSettings.xml").GetXML();
        soundLibraryCamera = GameObject.Find("sounds_Vocal").GetComponent<AudioLibrary>();
        movementSpeed = float.Parse(new XMLReader("Bomb.xml").GetXML().GetElementsByTagName("speed")[0].InnerText);
        towardsDestination = new GameObject();
    }

    void Update()
    {

        if (!networkView.isMine)
        {
            SmoothNetworkMovement();
            return;
        }

        if (CheckTriggeredPlaySound()) PlayTriggeredSound();

        if (explodeTeam != 0)
            Explode();

        if (BombDoesntMove || gameOver) return;

        target = WaypointA;
        if (ForceA > ForceB) target = WaypointB;

        if (!HaveReached(target)) { MoveTowards(target); }
        else
        {
            if (CanIPassValve(target)) SwitchStatus(target);
        }
    }

    private void SwitchStatus(GameObject waypoint)
    {
        if (!CanIPassValve(waypoint)) return;


        if (waypoint == WaypointB)
        {
            WaypointA = WaypointB;
            WaypointB = WaypointB.GetComponent<BombWaypoint>().WaypointB;
            if (WaypointB == null)
            {
                explodeTeam = 2;
                PlayExplosionSound(); //This is the vocal. Not the actual explosion.
            }
        }
        else
        {
            WaypointB = WaypointA;
            WaypointA = WaypointA.GetComponent<BombWaypoint>().WaypointA;
            if (WaypointA == null)
            {
                explodeTeam = 1;
                PlayExplosionSound();
            }
        }
    }

    [RPC]
    public void CameraShake()
    {
        GameObject cameraPlayer = GameObject.FindGameObjectWithTag(Tags.camera);
        cameraPlayer.GetComponent<Animation>().Play("CameraShake");
    }

    private void Explode()
    {
        if (explodeTeam == 1 && explosiontimer == 0)
        {
            PlaySound("BombExplosion");
            currentBase = explosionBase1;
            current_fw1 = firework_base1_1;
            current_fw2 = firework_base1_2;
        }
        else if (explodeTeam == 2 && explosiontimer == 0)
        {
            PlaySound("BombExplosion");
            currentBase = explosionBase2;
            current_fw1 = firework_base2_1;
            current_fw2 = firework_base2_2;
        }
        explosiontimer += Time.deltaTime;
        if (explosiontimer >= 2.25f)
        {
            CountPoints(explodeTeam);
            Network.Instantiate(Resources.Load("Fireworks"), current_fw1.transform.position, current_fw1.transform.rotation, 1);
            Network.Instantiate(Resources.Load("Fireworks"), current_fw2.transform.position, current_fw2.transform.rotation, 1);
            Network.Instantiate(Resources.Load("Fireworks"), currentBase.transform.position, currentBase.transform.rotation, 1);
            networkView.RPC("DestroyBomb", RPCMode.AllBuffered);
        }
        else if (explosiontimer >= 1.87f)
        {
            Network.Instantiate(Resources.Load("detonator"), currentBase.transform.position, currentBase.transform.rotation, 1);
            Network.Instantiate(Resources.Load("detonator"), gameObject.transform.position, gameObject.transform.rotation, 1);
        }
        else if (explosiontimer >= 1.37f)
        {
            networkView.RPC("CameraShake", RPCMode.AllBuffered);
            Network.Instantiate(Resources.Load("Fireworks"), current_fw1.transform.position, current_fw1.transform.rotation, 1);
            Network.Instantiate(Resources.Load("Fireworks"), current_fw2.transform.position, current_fw2.transform.rotation, 1);
        }
        else if (explosiontimer >= 0.87f)
        {
            Network.Instantiate(Resources.Load("Fireworks"), current_fw1.transform.position, current_fw1.transform.rotation, 1);
            Network.Instantiate(Resources.Load("Fireworks"), current_fw2.transform.position, current_fw2.transform.rotation, 1);
            Network.Instantiate(Resources.Load("Fireworks"), currentBase.transform.position, currentBase.transform.rotation, 1);
        }
    }

    [RPC]
    public void DestroyBomb()
    {
        Destroy(gameObject);
    }

    private bool CanIPassValve(GameObject target)
    {
        if (target == WaypointB) return WaypointB.GetComponent<BombWaypoint>().GetAllowPassage() || WaypointB.GetComponent<BombWaypoint>().BombStopper.GetComponent<Team>().ID == Team.TeamIdentifier.Team1;
        return WaypointA.GetComponent<BombWaypoint>().GetAllowPassage() || WaypointA.GetComponent<BombWaypoint>().BombStopper.GetComponent<Team>().ID == Team.TeamIdentifier.Team2;
    }

    private void MoveTowards(GameObject target)
    {
        if (WaypointB == null || WaypointA == null) return;

        target.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.position += Vector3.ClampMagnitude(target.transform.position - transform.position, Speed);
        towardsDestination.transform.position = transform.position;
        if (target == WaypointB)
        {
            towardsDestination.transform.LookAt(transform.position + transform.position - target.transform.position);
        }
        else
        {
            towardsDestination.transform.LookAt(target.transform);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, towardsDestination.transform.rotation, 0.01f);
    }

    private void CountPoints(int team)
    {
        GameController gameController = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameController>();
        gameController.networkView.RPC("IncreaseTeamPoints", RPCMode.AllBuffered, team);
        Debug.Log("Punkte f�r" + ((Team.TeamIdentifier)team).ToString());
    }

    private bool HaveReached(GameObject target)
    {
        if (target == null) return false;
        Vector3 a = transform.position;
        Vector3 b = target.transform.position;
        a.y = 0;
        b.y = 0;
        return Mathf.Abs((b - a).magnitude) < 0.5f;
    }

    public virtual void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        {
            Vector3 pos = Vector3.zero;

            Quaternion rot = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            for (int i = m_BufferedState.Length - 1; i >= 1; i--)
            {
                m_BufferedState[i] = m_BufferedState[i - 1];
            }

            State state = new State();
            state.timestamp = info.timestamp;
            state.pos = pos;
            state.rot = rot;

            m_BufferedState[0] = state;

            m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

            for (int i = 0; i < m_TimestampCount - 1; i++)
            {
                if (m_BufferedState[i].timestamp < m_BufferedState[i + 1].timestamp)
                    Debug.Log("State inconsistent");
            }
        }
    }

    private void SmoothNetworkMovement()
    {
        if (networkView.isMine)
            return;

        double interpolationTime = Network.time - m_InterpolationBackTime;

        if (m_BufferedState[0].timestamp > interpolationTime)
        {
            for (int i = 0; i < m_TimestampCount; i++)
            {
                if (!(m_BufferedState[i].timestamp <= interpolationTime) && i != m_TimestampCount - 1) continue;

                State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];

                State lhs = m_BufferedState[i];

                double length = rhs.timestamp - lhs.timestamp;
                float t = 0.0F;

                if (length > 0.0001)
                    t = (float)((interpolationTime - lhs.timestamp) / length);

                transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
                return;
            }
        }
        else
        {
            State latest = m_BufferedState[0];

            float extrapolationLength = (float)(interpolationTime - latest.timestamp);

            if (extrapolationLength < m_ExtrapolationLimit)
            {
                Quaternion angularRotation = Quaternion.identity;

                transform.localPosition = latest.pos + new Vector3(0f, 0f, extrapolationLength);
                transform.localRotation = angularRotation * latest.rot;
            }
        }
    }

    public int BombDirection()
    {
        if (ForceA > ForceB)
        {
            return (int)Team.TeamIdentifier.Team1;
        }

        if (ForceA < ForceB)
        {
            return (int)Team.TeamIdentifier.Team2;
        }

        return -1;
    }

    public void PlayTriggeredSound()
    {
        networkView.RPC("TriggerSound", RPCMode.All);
    }

    [RPC]
    public void TriggerSound()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player).Where(player => player.networkView.isMine))
        {
            if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
            {
                soundLibraryCamera
                         .StartSound(
                             GetDistance(transform.position, GameObject.Find("base02").transform.position) < GetDistance(transform.position, GameObject.Find("base01").transform.position)
                                 ? document.GetElementsByTagName("bombCloseToEnemy")[0].InnerText
                                 : document.GetElementsByTagName("bombCloseToOwn")[0].InnerText, 0f);
                return;
            }
            soundLibraryCamera
                     .StartSound(
                         GetDistance(transform.position, GameObject.Find("base01").transform.position) < GetDistance(transform.position, GameObject.Find("base02").transform.position)
                             ? document.GetElementsByTagName("bombCloseToEnemy")[0].InnerText
                             : document.GetElementsByTagName("bombCloseToOwn")[0].InnerText, 0f);
            return;
        }
    }

    /// <summary>
    /// Tries to play sound.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from an XML!</param>
    public void PlayExplosionSound()
    {
        networkView.RPC("ExplosionSound", RPCMode.All);
    }

    [RPC]
    public void ExplosionSound()
    {
        //soundLibrary.StartSound(document.GetElementsByTagName("explosion")[0].InnerText, 0f);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(Tags.player).Where(player => player.networkView.isMine))
        {
            if (player.GetComponent<Team>().ID == Team.TeamIdentifier.Team1)
            {
                soundLibraryCamera
                         .StartSound(
                             GetDistance(transform.position, GameObject.Find("base02").transform.position) < GetDistance(transform.position, GameObject.Find("base01").transform.position)
                                 ? document.GetElementsByTagName("explosionInEnemyBase")[0].InnerText
                                 : document.GetElementsByTagName("explosionInOwnBase")[0].InnerText, 0f);
            }
            else
            {
                soundLibraryCamera
                         .StartSound(
                             GetDistance(transform.position, GameObject.Find("base01").transform.position) < GetDistance(transform.position, GameObject.Find("base02").transform.position)
                                 ? document.GetElementsByTagName("explosionInEnemyBase")[0].InnerText
                                 : document.GetElementsByTagName("explosionInOwnBase")[0].InnerText, 0f);
            }
        }
    }

    private float GetDistance(Vector3 from, Vector3 to)
    {
        return Mathf.Abs((from - to).magnitude);
    }

    private bool CheckTriggeredPlaySound()
    {
        for (int i = 0; i < 3; i++)
        {
            if (checkTriggeredSound[i] ==
                GameObject.FindGameObjectWithTag(Tags.soundManager).GetComponent<SoundController>().MySounds[i]) continue;
            checkTriggeredSound[i] =
                GameObject.FindGameObjectWithTag(Tags.soundManager).GetComponent<SoundController>().MySounds[i];
            return !(soundLibraryCamera.aSources[document.GetElementsByTagName("explosionInEnemyBase")[0].InnerText].isPlaying || soundLibraryCamera.aSources[document.GetElementsByTagName("explosionInOwnBase")[0].InnerText].isPlaying);
        }
        return false;
    }

    public void PlaySound(string name, float delay = 0f)
    {
        networkView.RPC("StartSound", RPCMode.All, name, delay);
    }

    [RPC]
    public void StartSound(string name, float delay)
    {
        if (soundLibrary == null)
            soundLibrary = transform.FindChild("sound_bomb").FindChild("sounds_SFX").GetComponent<AudioLibrary>();
        soundLibrary.StartSound(name, delay);
    }
}
