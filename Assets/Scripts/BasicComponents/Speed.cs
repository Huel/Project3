using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Speed : MonoBehaviour
{
    private float _defaultSpeed = 3f;
    private float _sprintSpeed = 3.3f;
    private float _speedMultiplier = 1.0f;
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
            if (!networkView.isMine)
            {
                _isSprinting = value;
                return;
            }
            if (Stamina < MinStamina && !_isSprinting && value)
                return;
            _isSprinting = value;
            updateSprintingState(value);
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
                return SprintSpeed;
            return DefaultSpeed;
        }
    }
    public float DefaultSpeed
    {
        get { return _defaultSpeed * SpeedMultiplier; }
    }
    public float SprintSpeed
    {
        get { return _sprintSpeed * SpeedMultiplier; }
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
            networkView.RPC("SetDefaultSpeed", RPCMode.OthersBuffered, defaultSpeed);
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
            networkView.RPC("SetSprintSpeed", RPCMode.OthersBuffered, sprintSpeed);
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
            networkView.RPC("SetSpeedMultiplier", RPCMode.OthersBuffered, speedMultiplier);
    }
    /// <summary>
    ///     set the current stamina value, which is between maximum stamina and 0
    /// </summary>
    /// <param name="stamina"></param>
    public void SetStamina(float stamina)
    {
        if (networkView.isMine)
            _stamina = Mathf.Clamp(stamina, 0, MaxStamina);
    }
    /// <summary>
    ///     set the maximum stamina, minimum value 0
    /// </summary>
    /// <param name="maxStamina"></param>
    [RPC]
    public void SetMaxStamina(float maxStamina)
    {
        _maxStamina = Mathf.Max(0, maxStamina);
        SetStamina(_maxStamina);

        if (networkView.isMine)
            networkView.RPC("SetMaxStamina", RPCMode.OthersBuffered, maxStamina);
    }
    /// <summary>
    ///     set the minimum stamina, the minimum value which is needed for sprint activation
    /// </summary>
    /// <param name="minStamina"></param>
    [RPC]
    public void SetMinStamina(float minStamina)
    {
        _minStamina = Mathf.Clamp(minStamina, 0, MaxStamina);

        if (networkView.isMine)
            networkView.RPC("SetMinStamina", RPCMode.OthersBuffered, minStamina);
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
            networkView.RPC("SetStaminaRegenaration", RPCMode.OthersBuffered, staminaRegenaration);
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
            networkView.RPC("SetStaminaDecay", RPCMode.OthersBuffered, staminaDecay);
    }
    /// <summary>
    ///     increases the current stamina by adding the value
    /// </summary>
    /// <param name="staminaValue"></param>
    public void IncStamina(float staminaValue)
    {

        SetStamina(Stamina + staminaValue);
    }
    /// <summary>
    ///     decreases the current stamina by adding the value
    /// </summary>
    /// <param name="staminaValue"></param>
    public void DecStamina(float staminaValue)
    {
        IncStamina(-staminaValue);
    }

    [RPC]
    public void updateSprintingState(bool isSprinting)
    {
        if (networkView.isMine)
            networkView.RPC("updateSprintingState", RPCMode.OthersBuffered, isSprinting);
        else
            IsSprinting = isSprinting;
    }

    void Update()
    {
        if (!networkView.isMine)
            return;
        if (Stamina < MaxStamina && !IsSprinting)
            IncStamina(Time.deltaTime * StaminaRegenaration);
        else if (Stamina > 0 && IsSprinting)
            DecStamina(Time.deltaTime * StaminaDecay);
        if (Stamina <= 0)
            IsSprinting = false;
    }
}