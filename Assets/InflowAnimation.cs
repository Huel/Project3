using UnityEngine;

[ExecuteInEditMode]
public class InflowAnimation : MonoBehaviour
{
    public bool mirrored = false;
    public Valve valve;
    public ParticleEmitter fountain;
    public ParticleEmitter waterStream;
    public ParticleAnimator waterStreamParticle;
    public ParticleEmitter steam;

    [SerializeField]
    private int _currentState = 0;

    private Vector3 _fountainDirection;
    private bool _mirrored;

    private float _maxEmission = 100;
    private float _minEmission = 10;
    private float _minSize = 3;
    private float _maxSize = 6;
    private float _minForce = 11;
    private float _maxForce = 20;
    private float _minParticleForce = 3f;
    private float _maxParticleForce = 7.6f;
    private float _minSteam = 3f;
    private float _maxSteam = 10f;
    private float _steamOffset = 5.33f;
    // Use this for initialization
    [ExecuteInEditMode]
    void Awake()
    {
    }

    [ExecuteInEditMode]
    void Update()
    {

        if (mirrored != _mirrored)
            Mirror(mirrored);
        if (!valve)
            return;

        int state = (int)Mathf.Ceil(valve.State / 10f);
        if (state != _currentState)
        {
            _currentState = state;
            SetAnimation(_currentState / 10f);
        }

    }

    private void Mirror(bool m)
    {
        Transform tRotation = transform.FindChild("rotation");
        Transform tFountain = tRotation.FindChild("fountain");

        if (m)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            tFountain.localRotation = Quaternion.Euler(0f, 200f, 110f);
            _fountainDirection = fountain.transform.up;
            _fountainDirection.y = 0;
            _fountainDirection.Normalize();
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            tFountain.localRotation = Quaternion.Euler(0f, 90f, 90f);
            _fountainDirection = fountain.transform.up;
            _fountainDirection.y = 0;
            _fountainDirection.Normalize();
        }

        _mirrored = m;
    }

    private void SetAnimation(float value)
    {
        if (value == 0f)
        {
            fountain.emit = false;
            steam.emit = false;
            waterStream.emit = false;
            return;
        }

        if (_fountainDirection == Vector3.zero)
        {
            _fountainDirection = fountain.transform.up;
            _fountainDirection.y = 0;
            _fountainDirection.Normalize();
        }
        float emission = Mathf.Lerp(_minEmission, _maxEmission, value);
        fountain.maxEmission = emission;
        fountain.minEmission = emission;

        fountain.minSize = Mathf.Lerp(_minSize, _maxSize, value);

        Vector3 force = new Vector3(0f, Mathf.Lerp(_minForce, _maxForce, value));
        fountain.localVelocity = force;



        Vector3 particleForce = _fountainDirection * Mathf.Lerp(_minParticleForce, _maxParticleForce, value);
        waterStreamParticle.force = particleForce;

        float steamEmission = Mathf.Lerp(_minSteam, _maxSteam, value);
        steam.minSize = steamEmission;
        steam.maxSize = steamEmission;

        steam.transform.position = transform.FindChild("rotation").FindChild("steamOrigin").position + _fountainDirection * Mathf.Lerp(0, _steamOffset, value);

        fountain.emit = true;
        steam.emit = true;
        waterStream.emit = true;
    }
}
