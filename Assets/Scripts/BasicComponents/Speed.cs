using UnityEngine;

public class Speed : MonoBehaviour
{
    private float _defaultSpeed = 3f;
    private float _sprintSpeed = 3.3f;
    private float _speedMultiplier =1.0f;
    private float _stamina = 10f;
    private float _maxStamina = 10f;
    private float _minStamina = 1f;
    private float _staminaRegenaration = 0.2f;     //staminaPoints per second
    private float _staminaDecay = 0.3f;
    private bool _isSprinting;

    /// <summary>
    ///     only set isSprinting true if stamina is greater than the stamina Minimum
    /// </summary>
    public bool IsSprinting
    {
        get { return _isSprinting; }
        set
        {
            if (Stamina < MinStamina && !_isSprinting && value)
                return;
            else
                _isSprinting = value;
        }
    }
    /// <summary>
    ///     returns the current Speed (default or sprint)
    /// </summary>
    public float CurrentSpeed
    {
        get
        {
            if (IsSprinting)
                return _sprintSpeed;
            return _defaultSpeed;
        }
    }
    public float DefaultSpeed
    {
        get { return _defaultSpeed; }
    }
    public float SprintSpeed
    {
        get { return _sprintSpeed; }
    }
    public float SpeedMultiplier
    {
        get { return _speedMultiplier; }
    }
    public float Stamina
    {
        get { return _stamina; }
    }
    public float MaxStamina
    {
        get { return _maxStamina; }
    }
    public float MinStamina
    {
        get { return _minStamina; }
    }
    public float StaminaRegenaration
    {
        get { return _staminaRegenaration; }
    }
    public float StaminaDecay
    {
        get { return _staminaDecay; }
    }
    /// <summary>
    ///     set the default speed
    /// </summary>
    /// <param name="defaultSpeed"></param>
    [RPC]
    public void SetDefaultSpeed(float defaultSpeed)
    {
        _defaultSpeed = defaultSpeed;
        if (networkView.isMine)
            networkView.RPC("SetDefaultSpeed", RPCMode.Others, defaultSpeed);
    }
    /// <summary>
    ///     set the sprint speed
    /// </summary>
    /// <param name="sprintSpeed"></param>
    [RPC]
    public void SetSprintSpeed(float sprintSpeed)
    {
        _sprintSpeed = sprintSpeed;
        if (networkView.isMine)
            networkView.RPC("SetSprintSpeed", RPCMode.Others, sprintSpeed);
    }
    /// <summary>
    ///     set the sprint speed by multiply default speed with speed multiplier
    /// </summary>
    [RPC]
    public void SetSprintSpeedByMultiplier()
    {
        _sprintSpeed = _defaultSpeed * _speedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetSprintSpeedByMultiplier", RPCMode.Others);
    }
    /// <summary>
    ///     set the speed multiplier
    /// </summary>
    /// <param name="speedMultiplier"></param>
    [RPC]
    public void SetSpeedMultiplier(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetSpeedMultiplier", RPCMode.Others, speedMultiplier);
    }
    /// <summary>
    ///     set the current stamina value, which is between maximum stamina and 0
    /// </summary>
    /// <param name="stamina"></param>
    [RPC]
    public void SetStamina(float stamina)
    {
        if (stamina > _maxStamina)
            _stamina = _maxStamina;
        else if (stamina >= 0)
            _stamina = stamina;
        else
            _stamina = 0;

        if (networkView.isMine)
            networkView.RPC("SetStamina", RPCMode.Others, stamina);
    }
    /// <summary>
    ///     set the maximum stamina, minimum value 0
    /// </summary>
    /// <param name="maxStamina"></param>
    [RPC]
    public void SetMaxStamina(float maxStamina)
    {
        if (maxStamina >= 0)
            _maxStamina = maxStamina;
        else
            _maxStamina = 0;

        if (networkView.isMine)
            networkView.RPC("SetMaxStamina", RPCMode.Others, maxStamina);
    }
    /// <summary>
    ///     set the minimum stamina, the minimum value which is needed for sprint activation
    /// </summary>
    /// <param name="minStamina"></param>
    [RPC]
    public void SetMinStamina(float minStamina)
    {
        if (minStamina >= 0 && minStamina <= _maxStamina)
            _minStamina = minStamina;
        else
            _minStamina = 0;

        if (networkView.isMine)
            networkView.RPC("SetMinStamina", RPCMode.Others, minStamina);
    }
    /// <summary>
    ///     set the stamina regenaration speed
    /// </summary>
    /// <param name="staminaRegenaration"></param>
    [RPC]
    public void SetStaminaRegenaration(float staminaRegenaration)
    {
        _staminaRegenaration = staminaRegenaration;
        if (networkView.isMine)
            networkView.RPC("SetStaminaRegenaration", RPCMode.Others, staminaRegenaration);
    }
    /// <summary>
    ///     set the stamina decay speed
    /// </summary>
    /// <param name="staminaDecay"></param>
    [RPC]
    public void SetStaminaDecay(float staminaDecay)
    {
        _staminaDecay = staminaDecay;
        if (networkView.isMine)
            networkView.RPC("SetStaminaDecay", RPCMode.Others, staminaDecay);
    }
    /// <summary>
    ///     increases the current stamina by adding the value
    /// </summary>
    /// <param name="staminaValue"></param>
    [RPC]
    public void IncStamina(float staminaValue)
    {
        _stamina += staminaValue;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;
        if (networkView.isMine)
            networkView.RPC("IncStamina", RPCMode.Others, staminaValue);
    }
    /// <summary>
    ///     decreases the current stamina by adding the value
    /// </summary>
    /// <param name="staminaValue"></param>
    [RPC]
    public void DecStamina(float staminaValue)
    {
        _stamina -= staminaValue;
        if (_stamina < 0)
            _stamina = 0;
        if (networkView.isMine)
            networkView.RPC("DecStamina", RPCMode.Others, staminaValue);
    }

    void Update()
    {
        if (_stamina < _maxStamina && !IsSprinting)
            _stamina += Mathf.Clamp(Time.deltaTime * _staminaRegenaration, 0, _maxStamina);
        else if (_stamina > 0 && IsSprinting)
            _stamina -= Mathf.Clamp(Time.deltaTime * _staminaDecay, 0, _maxStamina);
        else if (_stamina > _maxStamina)
            _stamina = _maxStamina;
        else if (_stamina < 0)
            _stamina = 0;

        if (_stamina <= 0)
            IsSprinting = false;
    }
}