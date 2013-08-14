/// <summary>
/// UnityTutorials - A Unity Game Design Prototyping Sandbox
/// <copyright>(c) John McElmurray and Julian Adams 2013</copyright>
/// 
/// UnityTutorials homepage: https://github.com/jm991/UnityTutorials
/// 
/// This software is provided 'as-is', without any express or implied
/// warranty.  In no event will the authors be held liable for any damages
/// arising from the use of this software.
///
/// Permission is granted to anyone to use this software for any purpose,
/// and to alter it and redistribute it freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; you must not
/// claim that you wrote the original software. If you use this software
/// in a product, an acknowledgment in the product documentation would be
/// appreciated but is not required.
/// 2. Altered source versions must be plainly marked as such, and must not be
/// misrepresented as being the original software.
/// 3. This notice may not be removed or altered from any source distribution.
/// </summary>

using UnityEngine;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class CharacterControllerLogic : MonoBehaviour
{

    public Skill basicAttack;   //= new Skill();
    public Skill skill1;        //= new Skill();
    public Skill skill2;        //= new Skill();
    public Skill skill3;        //= new Skill();
    public Skill skill4;        //= new Skill();
    public Skill heroicAura;    //= new Skill();

    #region Variables (private)

    // Inspector serialized
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private ThirdPersonCamera gamecam;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;
    [SerializeField]
    private float directionSpeed = 1.5f;
    [SerializeField]
    private float directionDampTime = 0.25f;
    [SerializeField]
    private float speedDampTime = 0.05f;
    [SerializeField]
    private float fovDampTime = 3f;
    [SerializeField]
    private CapsuleCollider capCollider;


    // Private global only
    private float leftX = 0f;
    private float leftY = 0f;
    private AnimatorStateInfo stateInfo;
    private AnimatorTransitionInfo transInfo;
    private float speed = 0f;
    private float direction = 0f;
    private float charAngle = 0f;
    private const float SPRINT_FOV = 75.0f;
    private const float NORMAL_FOV = 60.0f;
    private float capsuleHeight;
    private Vector3 stickDirection;


    // Hashes
    private int m_LocomotionId = 0;
    private int m_LocomotionPivotLId = 0;
    private int m_LocomotionPivotRId = 0;
    private int m_LocomotionPivotLTransId = 0;
    private int m_LocomotionPivotRTransId = 0;



    private const float TARGETING_THRESHOLD = 0.01f;

    #endregion


    #region Properties (public)

    public Vector3 StickInput
    {
        get
        {
            return stickDirection;
        }
    }

    public Animator Animator
    {
        get
        {
            return this.animator;
        }
    }

    public float Speed
    {
        get
        {
            return this.speed;
        }
    }

    public float LocomotionThreshold
    {
        get
        {
            return 0.2f;
        }
    }

    #endregion


    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        PlayerPrefs.DeleteAll();
        animator = GetComponent<Animator>();
        capCollider = GetComponent<CapsuleCollider>();
        capsuleHeight = capCollider.height;

        if (animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }

        // Hash all animation names for performance
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
        m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
        m_LocomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        //Debug.Log(basicAttack +", "+ skill1 +", "+ skill2 +", "+ skill3 +", "+ skill4 +", "+ heroicAura);
        if (!GetComponent<NetworkView>().isMine)
            return;
        bool alive = GetComponent<Health>().IsAlive();

        if (alive)
            HandleInput();

        if (gamecam == null)
        {
            gamecam = GameObject.FindGameObjectWithTag(Tags.camera).GetComponent<ThirdPersonCamera>();
        }
        if (animator)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            transInfo = animator.GetAnimatorTransitionInfo(0);

            if (alive)
            {
                // Pull values from controller/keyboard
                leftX = Input.GetAxis(InputTags.horizontal);
                leftY = Input.GetAxis(InputTags.vertical);

                stickDirection = new Vector3(leftX, 0, leftY);
            }
            else
            {
                stickDirection = new Vector3();
            }


            charAngle = 0f;
            direction = 0f;
            float charSpeed = 0f;
            Speed speedComp = GetComponent<Speed>();

            // Translate controls stick coordinates into world/cam/character space
            StickToWorldspace(this.transform, gamecam.transform, ref direction, ref charSpeed, ref charAngle, IsInPivot());


            // Press B to sprint
            if (speedComp.IsSprinting && Input.GetButton(InputTags.sprint) && charSpeed >= 0.1f)
            {

                speed = Mathf.Lerp(speed, speedComp.CurrentSpeed, Time.deltaTime);
                gamecam.camera.fieldOfView = Mathf.Lerp(gamecam.camera.fieldOfView, SPRINT_FOV, fovDampTime * Time.deltaTime);
            }
            else
            {
                speedComp.IsSprinting = false;
                speed = charSpeed * speedComp.CurrentSpeed;
                gamecam.camera.fieldOfView = Mathf.Lerp(gamecam.camera.fieldOfView, NORMAL_FOV, fovDampTime * Time.deltaTime);
            }

            animator.SetFloat(AnimatorTags.speed, speed, speedDampTime, Time.deltaTime);
            animator.SetFloat(AnimatorTags.direction, direction, directionDampTime, Time.deltaTime);

            if (speed > LocomotionThreshold)	// Dead zone
            {
                if (!IsInPivot())
                {
                    Animator.SetFloat(AnimatorTags.angle, charAngle);
                }
            }
            if (speed < LocomotionThreshold && Mathf.Abs(leftX) < 0.05f)    // Dead zone
            {
                animator.SetFloat(AnimatorTags.direction, 0f);
                animator.SetFloat(AnimatorTags.angle, 0f);
            }
        }
    }



    private void HandleInput()
    {
        if (Input.GetButtonDown(InputTags.sprint))
        {
            GetComponent<Speed>().IsSprinting = true;
        }
        if (CustomInput.GetTriggerDown(InputTags.basicAttack))
        {
            if (basicAttack)
            {
                basicAttack.Execute();
            }
        }

        if (Input.GetButtonDown(InputTags.skill1))
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
    }

    /// <summary>
    /// Any code that moves the character needs to be checked against physics
    /// </summary>
    void FixedUpdate()
    {
        if (!GetComponent<NetworkView>().isMine || !GetComponent<Health>().IsAlive())
            return;
        // Rotate character model if stick is tilted right or left, but only if character is moving in that direction
        if (IsInLocomotion() && gamecam.CamState != ThirdPersonCamera.CamStates.Free && !IsInPivot() && ((direction >= 0 && leftX >= 0) || (direction < 0 && leftX < 0)))
        {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }
    }

    /// <summary>
    /// Debugging information should be put here.
    /// </summary>
    void OnDrawGizmos()
    {

    }

    #endregion


    #region Methods

    public bool IsInPivot()
    {
        return stateInfo.nameHash == m_LocomotionPivotLId ||
            stateInfo.nameHash == m_LocomotionPivotRId ||
            transInfo.nameHash == m_LocomotionPivotLTransId ||
            transInfo.nameHash == m_LocomotionPivotRTransId;
    }

    public bool IsInLocomotion()
    {
        return stateInfo.nameHash == m_LocomotionId;
    }

    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 rootDirection = root.forward;

        speedOut = stickDirection.sqrMagnitude;

        // Get camera rotation
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f; // kill Y
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(CameraDirection));

        // Convert joystick input in Worldspace coordinates
        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisSign, Color.red);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        if (!isPivoting)
        {
            angleOut = angleRootToMove;
        }
        angleRootToMove /= 180f;

        directionOut = angleRootToMove * directionSpeed;
    }

    #endregion Methods
}
