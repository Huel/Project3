
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
public class MinionVisualsController : MonoBehaviour
{
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
        if (!GetComponent<NetworkView>().isMine)
            return;

        if (animator)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            transInfo = animator.GetAnimatorTransitionInfo(0);

            stickDirection = GetComponent<NavMeshAgent>().desiredVelocity;

            charAngle = 0f;
            direction = 0f;
            float charSpeed = 0f;

            // Translate controls stick coordinates into world/cam/character space
            StickToWorldspace(this.transform, ref direction, ref charSpeed, ref charAngle, IsInPivot());
            speed = charSpeed;

            animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
            animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

            if (speed > LocomotionThreshold)	// Dead zone
            {
                if (!IsInPivot())
                {
                    Animator.SetFloat("Angle", charAngle);
                }
            }
            if (speed < LocomotionThreshold && Mathf.Abs(leftX) < 0.05f)    // Dead zone
            {
                animator.SetFloat("Direction", 0f);
                animator.SetFloat("Angle", 0f);
            }
        }
    }

    /// <summary>
    /// Any code that moves the character needs to be checked against physics
    /// </summary>
    void FixedUpdate()
    {
        if (!GetComponent<NetworkView>().isMine)
            return;
        // Rotate character model if stick is tilted right or left, but only if character is moving in that direction
        if (IsInLocomotion() && !IsInPivot() && ((direction >= 0 && leftX >= 0) || (direction < 0 && leftX < 0)))
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

    public void StickToWorldspace(Transform root, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 rootDirection = root.forward;

        speedOut = stickDirection.sqrMagnitude;


        // Convert joystick input in Worldspace coordinates
        Vector3 moveDirection = stickDirection;
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

    [RPC]
    public void killObject()
    {
        Destroy(gameObject);
    }

    #endregion Methods
}
