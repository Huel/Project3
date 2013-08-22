using System.Collections;
using UnityEngine;

public class CharController : MonoBehaviour
{

    private Animator animator;
    private CharacterController controller;
    private OrbitCamera gamecam;
    private Health health;
    private Speed speed;
    private System.Xml.XmlDocument document;
    private AudioLibrary soundLibrary;

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

    private bool buttonPushed;

    public float Speed
    {
        get { return _speed; }
    }

    void Start()
    {
        soundLibrary = transform.FindChild("sounds_hero01").GetComponent<AudioLibrary>();
        document = new XMLReader("Hero01.xml").GetXML();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        speed = GetComponent<Speed>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!networkView.isMine)
            return;

        float horizontal = 0f;
        float vertical = 0f;
        if (health.IsAlive())
            HandleInput(ref horizontal, ref vertical);

        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);
        float charDirection = 0f;
        float charSpeed = 0f;
        float charAngle = 0f;

        if (_targeting)
        {
            stickDirection.x = Mathf.Clamp(stickDirection.x, -_maxSideSpeed, _maxSideSpeed);
            if (vertical < 0)
                stickDirection.z = Mathf.Max(stickDirection.z, -_maxBackSpeed);
        }

        Vector3 moveDir = StickToWorldspace(stickDirection, ref charDirection, ref charSpeed, ref charAngle);

        if (speed.IsSprinting && Input.GetButton(InputTags.sprint) && charSpeed >= speedThreshold && !(_targeting && vertical < 0))
        {

            _speed = Mathf.Lerp(_speed, speed.CurrentSpeed, Time.deltaTime);
            gamecam.camera.fieldOfView = Mathf.Lerp(gamecam.camera.fieldOfView, sprintFOV, fovDampTime * Time.deltaTime);
        }
        else
        {
            speed.IsSprinting = false;
            _speed = charSpeed * speed.CurrentSpeed;
            gamecam.camera.fieldOfView = Mathf.Lerp(gamecam.camera.fieldOfView, normalFOV, fovDampTime * Time.deltaTime);
        }



        if (_targeting)
        {
            controller.Move(moveDir * 0.1f);
            animator.SetFloat(AnimatorTags.angle, charAngle);
        }
        else
        {
            controller.Move(transform.forward * _speed * 0.1f);
            if (_speed > speedThreshold)
                transform.Rotate(Vector3.up, charAngle);
            animator.SetFloat(AnimatorTags.angle, 0f);
        }

        if (soundLibrary != null && soundLibrary.aSources != null && soundLibrary.aSources[document.GetElementsByTagName("run")[0].InnerText] != null && !soundLibrary.aSources[document.GetElementsByTagName("run")[0].InnerText].isPlaying)
        {
            PlaySound(document.GetElementsByTagName("run")[0].InnerText);
        }

        controller.Move(Physics.gravity);
        animator.SetFloat(AnimatorTags.speed, _speed);


    }

    IEnumerator PlayAnimation(string animBoolName)
    {
        GetComponent<Animator>().SetBool(animBoolName, true);
        yield return null;
        GetComponent<Animator>().SetBool(animBoolName, false);
    }

    private void HandleInput(ref float horizontal, ref float vertical)
    {

        horizontal = Input.GetAxis(InputTags.horizontal);
        vertical = Input.GetAxis(InputTags.vertical);

        _targeting = CustomInput.GetTrigger(InputTags.target);

        if (Input.GetButtonDown(InputTags.sprint) && !(_targeting && vertical < 0))
            GetComponent<Speed>().IsSprinting = true;

        if (CustomInput.GetTriggerDown(InputTags.basicAttack))
        {
            if (basicAttack)
            {
                if (basicAttack.Execute())
                {
                    int rnd = Random.Range(1, 3);
                    switch (rnd)
                    {
                        case 1:
                            PlaySound(document.GetElementsByTagName("basicAttackVariation1")[0].InnerText);
                            break;
                        case 2:
                            PlaySound(document.GetElementsByTagName("basicAttackVariation2")[0].InnerText);
                            break;
                        case 3:
                            PlaySound(document.GetElementsByTagName("basicAttackVariation3")[0].InnerText);
                            break;
                    }
                }
            }
        }
        if (Input.GetButtonDown(InputTags.skill1) || Input.GetKeyDown(KeyCode.A))
        {
            if (skill1)
            {
                if (skill1.State == SkillState.Ready) PlaySound(document.GetElementsByTagName("shieldwall")[0].InnerText);
                skill1.Execute();
            }
        }
        if (Input.GetButtonDown(InputTags.skill2) || Input.GetKeyDown(KeyCode.S))
        {
            if (skill2)
            {
                if (skill2.State == SkillState.Ready) PlaySound(document.GetElementsByTagName("kamikazeMission")[0].InnerText);
                skill2.Execute();
            }
        }
        if (Input.GetButtonDown(InputTags.skill3) || Input.GetKeyDown(KeyCode.D))
        {
            if (skill3)
            {
                if (skill3.State == SkillState.Ready) PlaySound(document.GetElementsByTagName("freshMeat")[0].InnerText);
                skill3.Execute();
            }
        }
        if (Input.GetButtonDown(InputTags.skill4) || Input.GetKeyDown(KeyCode.F))
        {
            if (skill4)
            {
                if (skill4.State == SkillState.Ready) PlaySound(document.GetElementsByTagName("battlecry")[0].InnerText);
                skill4.Execute();
            }
        }
        HandleInput();
    }

    private void HandleInput()
    {
        if (!buttonPushed)
        {
            if (Input.GetAxisRaw(InputTags.squadLane) != 0 || Input.GetAxisRaw(InputTags.squadSelection) != 0)
            {
                buttonPushed = true;
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
                buttonPushed = false;
            }
        }
    }

    private Vector3 StickToWorldspace(Vector3 stickDirection, ref float directionOut, ref float speedOut, ref float angleOut)
    {
        FindCamera();
        Vector3 charDirection = transform.forward;
        Vector3 cameraDirection;
        if (_targeting)
            cameraDirection = transform.forward;
        else
            cameraDirection = gamecam.transform.forward;
        cameraDirection.y = 0.0f;
        cameraDirection.Normalize();

        speedOut = stickDirection.magnitude;


        Quaternion cameraRotation = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
        Vector3 moveDirection = cameraRotation * stickDirection;

        int axisSign = Vector3.Cross(moveDirection, charDirection).y >= 0 ? -1 : 1;

        float angleRootToMove = Vector3.Angle(charDirection, moveDirection) * axisSign;

        angleOut = angleRootToMove;
        if (stickDirection.magnitude == 0)
            angleOut = 0;

        angleRootToMove /= 180f;
        directionOut = angleRootToMove;

        return moveDirection;
    }

    private void FindCamera()
    {
        if (gamecam == null)
            gamecam = GameObject.FindGameObjectWithTag(Tags.camera).GetComponent<OrbitCamera>();
    }

    /// <summary>
    /// Tries to play sound.
    /// </summary>
    /// <param name="name">Name of the Sound file, should be extracted from an XML!</param>
    public void PlaySound(string name, float delay = 0f)
    {
        networkView.RPC("StartSound", RPCMode.All, name, delay);
    }

    [RPC]
    public void StartSound(string name, float delay)
    {
        soundLibrary.StartSound(name, delay);
    }
}
