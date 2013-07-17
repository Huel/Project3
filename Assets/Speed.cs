using UnityEngine;
using System.Collections;

public class Speed : MonoBehaviour
{
    private float _defaultSpeed;
    private float _sprintSpeed;
    private float _speedMultiplier;
    private float _stamina;
    private float _maxStamina;
    private float _minStamina;
    private float _staminaRegenaration;     //staminaPoints per second
    private float _staminaDecay;
    public bool isSprinting;

    public float CurrentSpeed
    {
        get 
        {
            if (isSprinting)
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

    [RPC]
    public void SetDefaultSpeed(float defaultSpeed)
    {
        _defaultSpeed = defaultSpeed;
        if (networkView.isMine)
            networkView.RPC("SetDefaultSpeed", RPCMode.Others);
    }
    [RPC]
    public void SetSprintSpeed(float sprintSpeed)
    {
        _sprintSpeed = sprintSpeed;
        if (networkView.isMine)
            networkView.RPC("SetSprintSpeed", RPCMode.Others);
    }
    [RPC]
    public void SetSprintSpeedByMultiplier()
    {
        _sprintSpeed = _defaultSpeed * _speedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetSprintSpeedByMultiplier", RPCMode.Others);
    }
    [RPC]
    public void SetSpeedMultiplier(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
        if (networkView.isMine)
            networkView.RPC("SetSpeedMultiplier", RPCMode.Others);
    }
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
            networkView.RPC("SetStamina", RPCMode.Others);
    }
    [RPC]
    public void SetMaxStamina(float maxStamina)
    {
        if (maxStamina >= 0)
            _maxStamina = maxStamina;
        else
            _maxStamina = 0;

        if (networkView.isMine)
            networkView.RPC("SetMaxStamina", RPCMode.Others);
    }
    [RPC]
    public void SetMinStamina(float minStamina)
    {
        if (minStamina >= 0 && minStamina <= _maxStamina)
            _minStamina = minStamina;
        else
            _minStamina = 0;

        if (networkView.isMine)
            networkView.RPC("SetMinStamina", RPCMode.Others);
    }
    [RPC]
    public void SetStaminaRegenaration(float staminaRegenaration)
    {
        _staminaRegenaration = staminaRegenaration;
        if (networkView.isMine)
            networkView.RPC("SetStaminaRegenaration", RPCMode.Others);
    }
    [RPC]
    public void SetStaminaDecay(float staminaDecay)
    {
        _staminaDecay = staminaDecay;
        if (networkView.isMine)
            networkView.RPC("SetStaminaDecay", RPCMode.Others);
    }
    [RPC]
    public void IncStamina(float staminaValue)
    {   
        _stamina += staminaValue;
        if (_stamina > _maxStamina)
            _stamina = _maxStamina;
        if (networkView.isMine)
            networkView.RPC("IncStamina", RPCMode.Others);
    }
    [RPC]
    public void DecStamina(float staminaValue)
    {
        _stamina -= staminaValue;
        if (_stamina < 0)
            _stamina = 0;
        if (networkView.isMine)
            networkView.RPC("DecStamina", RPCMode.Others);
    }

    void Update()
    {
        if (_stamina < _maxStamina && !isSprinting)
            _stamina += Time.deltaTime * _staminaRegenaration;
        else if (_stamina > MinStamina && isSprinting)
            _stamina -= Time.deltaTime * _staminaDecay;
        else if (_stamina > _maxStamina)
            _stamina = _maxStamina;
        else if (_stamina <= 0)
            isSprinting = false;
    }
}