using UnityEngine;

public class CharController : MonoBehaviour
{

    private Animator _animator;
    private CharacterController _controller;
    private OrbitCamera _gamecam;
    private Health _healthComponent;
    private Speed _speedComponent;

    private float _speed = 0f;
    private const float _maxSideSpeed = 0.75f;
    private const float _maxBackSpeed = 0.5f;
    private bool _targeting = false;

    public Skill basicAttack;
    public Skill skill1;
    public Skill skill2;
    public Skill skill3;
    public Skill skill4;
    public Skill addSquad;
    public Skill removeSquad;
    public Skill heroicAura;

    public const float speedThreshold = 0.1f;
    private const float sprintFOV = 75.0f;
    private const float normalFOV = 60.0f;
    private const float fovDampTime = 3f;

    private bool _buttonPushed;

    public float Speed
    {
        get { return _speed; }
    }

    void Start()
    {
        name = "Hero(own)";
        _animator = GetComponent<Animator>();
        _healthComponent = GetComponent<Health>();
        _speedComponent = GetComponent<Speed>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //If it's not my character the CharController should be disabled.
        if (!networkView.isMine)
        {
            name = "Hero(enemy)";
            enabled = false;
            return;
        }


        //Get Inputs from left analogue stick:
        float horizontal = 0f;
        float vertical = 0f;
        //If the character isn't alive anymore horizontal and vertical inputs should stay 0f.
        if (_healthComponent.IsAlive())
            HandleInput(ref horizontal, ref vertical);
        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);


        //Calculate the move direction by the stick inputs:
        float charSpeed = 0f;
        float charAngle = 0f;
        //In the target-mode there is a maximum speed for moving sideways and backwards.
        if (_targeting)
        {
            stickDirection.x = Mathf.Clamp(stickDirection.x, -_maxSideSpeed, _maxSideSpeed);
            if (vertical < 0)
                stickDirection.z = Mathf.Max(stickDirection.z, -_maxBackSpeed);
        }
        Vector3 moveDir = StickToWorldspace(stickDirection, ref charSpeed, ref charAngle);


        //Execute Sprinting effects. The right speed is handled through the speed component.
        if (_speedComponent.IsSprinting && Input.GetButton(InputTags.sprint) && charSpeed >= speedThreshold && !(_targeting && vertical < 0))
        {
            //Accelerate when sprinting
            _speed = Mathf.Lerp(_speed, _speedComponent.CurrentSpeed, Time.deltaTime);
            //Camera effect for sprinting
            _gamecam.camera.fieldOfView = Mathf.Lerp(_gamecam.camera.fieldOfView, sprintFOV, fovDampTime * Time.deltaTime);
        }
        else
        {
            //End sprint mode
            _speedComponent.IsSprinting = false;
            _speed = charSpeed * _speedComponent.CurrentSpeed;
            //Normalize camera effect
            _gamecam.camera.fieldOfView = Mathf.Lerp(_gamecam.camera.fieldOfView, normalFOV, fovDampTime * Time.deltaTime);
        }


        //Move the character:
        if (_targeting && _speed >= speedThreshold)
        {
            //In target-mode just use the calculated move direction related to world space because the character shall not rotate.
            _controller.Move(moveDir * _speed * Time.deltaTime);
            //Send the angle between character-forward-vector and move direction to the animator for the right animation.
            _animator.SetFloat(AnimatorTags.angle, charAngle);
        }
        else
        {
            if (_speed >= speedThreshold)
            {
                //Rotate the character
                transform.Rotate(Vector3.up, charAngle);
                //... and move forward
                _controller.Move(transform.forward * _speed * Time.deltaTime);
            }
            //No moving sideways and backwards, that's why send the angle 0 to the animator    
            _animator.SetFloat(AnimatorTags.angle, 0f);
        }
        //Use gravity.
        _controller.Move(Physics.gravity);
        //Send the character speed (related to his default speed) to the animator to blend the animation
        _animator.SetFloat(AnimatorTags.speed, _speed / 5.5f);
    }

    private void HandleInput(ref float horizontal, ref float vertical)
    {

        if (GameObject.FindGameObjectWithTag(Tags.minionManager).GetComponent<MinionManager>().GetMinionManagerState() == MinionManager.MinionManagerState.Invisible)
        {
            horizontal = Input.GetAxis(InputTags.horizontal);
            vertical = Input.GetAxis(InputTags.vertical);

            _targeting = CustomInput.GetTrigger(InputTags.target);

            if (Input.GetButtonDown(InputTags.sprint) && !(_targeting && vertical < 0))
                GetComponent<Speed>().IsSprinting = true;

            if (CustomInput.GetTriggerDown(InputTags.basicAttack))
            {
                if (basicAttack)
                    basicAttack.Execute();
            }
            if (Input.GetButtonDown(InputTags.skill1) || Input.GetKeyDown(KeyCode.A))
            {
                if (skill1)
                    skill1.Execute();
            }
            if (Input.GetButtonDown(InputTags.skill2))
            {
                if (skill2)
                    skill2.Execute();
            }
            if (Input.GetButtonDown(InputTags.skill3))
            {
                if (skill3)
                    skill3.Execute();
            }
            if (Input.GetButtonDown(InputTags.skill4))
            {
                if (skill4)
                    skill4.Execute();
            }

            HandleSquadInput();
        }
    }

    private void HandleSquadInput()
    {
        if (!_buttonPushed)
        {
            if (Input.GetAxisRaw(InputTags.squadLane) != 0 || Input.GetAxisRaw(InputTags.squadSelection) != 0)
            {
                _buttonPushed = true;
            }
            if (Input.GetAxisRaw(InputTags.squadSelection) > 0.1 && !(Input.GetAxisRaw(InputTags.squadLane) < -0.1 || Input.GetAxisRaw(InputTags.squadLane) > 0.1))
                addSquad.Execute();
            if (Input.GetAxisRaw(InputTags.squadSelection) < -0.1 && !(Input.GetAxisRaw(InputTags.squadLane) < -0.1 || Input.GetAxisRaw(InputTags.squadLane) > 0.1))
                removeSquad.Execute();
        }
        else
        {
            if (Input.GetAxisRaw(InputTags.squadLane) == 0 && Input.GetAxisRaw(InputTags.squadSelection) == 0)
            {
                _buttonPushed = false;
            }
        }
    }


    //This method is based on the tutorial by John McElmurray. Visit https://github.com/jm991/UnityThirdPersonTutorial/.
    private Vector3 StickToWorldspace(Vector3 stickDirection, ref float speedOut, ref float angleOut)
    {

        Vector3 charDirection = transform.forward;

        //Get camera direction:
        FindCamera();
        Vector3 cameraDirection;
        if (_targeting)
        {
            //In target-mode use the character's forward-vector as camera direction
            cameraDirection = transform.forward;
        }
        else
        {
            cameraDirection = _gamecam.transform.forward;
        }
        //Kill y and normalize
        cameraDirection.y = 0.0f;
        cameraDirection.Normalize();

        //The speed is between 0 and 1 based on the stick inputs. It's just the magnitude of the stick direction.
        speedOut = stickDirection.magnitude;

        //Convert the Vector3 cameraDirection to a Quaternion (rotation with world z-axis as origin)
        Quaternion cameraRotation = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
        //Rotate the stick direction by the camera rotation than you get the move direction
        Vector3 moveDirection = cameraRotation * stickDirection;

        //Get the angle and the stick direction:
        if (speedOut != 0f)
        {
            //Use the cross product to find out if the angle between the character (look) direction 
            //  and his move direction is positive or negative.
            int axisSign = Vector3.Cross(moveDirection, charDirection).y >= 0 ? -1 : 1;
            //Get the angle between the character (look) direction and his move direction
            angleOut = Vector3.Angle(charDirection, moveDirection) * axisSign;
        }

        return moveDirection;
    }

    private void FindCamera()
    {
        if (_gamecam == null)
            _gamecam = GameObject.FindGameObjectWithTag(Tags.camera).GetComponent<OrbitCamera>();
    }
}
