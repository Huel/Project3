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
[RequireComponent(typeof(BarsEffect))]
public class OrbitCamera : MonoBehaviour
{
    private Transform parentRig;
    private float distanceAway = 5f;
    private float distanceAwayMultipler = 2f;
    private float distanceUp = 1.5f;
    private float distanceUpMultiplier = 2.5f;
    private CharController player;
    private float widescreen = 0.2f;
    private float targetingTime = 0.5f;
    private float firstPersonLookSpeed = 3.0f;

    private float firstPersonThreshold = 0.5f;
    private float freeThreshold = 0.1f;
    private Vector2 camMinDistFromChar = new Vector2(3f, 0f);
    private float rightStickThreshold = 0.3f;
    private const float freeRotationDegreePerSecond = -5f;


    // Smoothing and damping
    private Vector3 velocityCamSmooth = Vector3.zero;
    private float camSmoothDampTime = 0.1f;
    private Vector3 velocityLookDir = Vector3.zero;
    private float lookDirDampTime = 0.1f;


    // Private global only
    private Vector3 lookDir;
    private Vector3 curLookDir;
    private BarsEffect barEffect;
    private CamStates camState = CamStates.Behind;
    private float xAxisRot = 0.0f;
    private const float TARGETING_THRESHOLD = 0.01f;
    private Vector3 savedRigToGoal;
    private float distanceAwayFree;
    private float distanceUpFree;
    private Vector2 rightStickPrevFrame = Vector2.zero;

    public Vector3 LookDir
    {
        get
        {
            return this.curLookDir;
        }
    }

    public CamStates CamState
    {
        get
        {
            return this.camState;
        }
    }

    public enum CamStates
    {
        Behind,
        Target,
        Free
    }


    void Start()
    {
        parentRig = this.transform.parent;
        barEffect = GetComponent<BarsEffect>();

    }


    void Update()
    {
        if (player == null)
            FindPlayer();
    }

    void FindPlayer()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag(Tags.player);
        foreach (GameObject p in playerList)
        {
            if (p.networkView.isMine)
            {
                player = p.GetComponent<CharController>();
                break;
            }
        }

        lookDir = player.transform.forward;
        curLookDir = player.transform.forward;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        //Get Inputs for camera (right analogue stick)
        float cameraX = Input.GetAxis(InputTags.cameraX);
        float cameraY = Input.GetAxis(InputTags.cameraY);

        //Camera shouldn't look at the feet
        Vector3 characterOffset = player.transform.position + new Vector3(0f, distanceUp, 0f);
        Vector3 lookAt = characterOffset;
        Vector3 targetPosition = Vector3.zero;

        // Determine camera state:
        // * Targeting *
        if (CustomInput.GetTrigger(InputTags.target))
        {
            //Cinematic effect for target mode
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, widescreen, targetingTime);
            //Set camera state to target mode
            camState = CamStates.Target;
        }
        else
        {
            //Remove the effect for target mode
            barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);

            //If you aren't in the camera state "Free" and you are using the right analogue stick switch to camera state "Free"
            if (camState != CamStates.Free && (Mathf.Abs(cameraX) >= freeThreshold || Mathf.Abs(cameraY) >= freeThreshold))
            {
                camState = CamStates.Free;
                savedRigToGoal = Vector3.zero;
                //Reset the distance from the character to the camera with the default values
                distanceAwayFree = distanceAway;
                distanceUpFree = distanceUp;
            }

            // * Behind the back *
            if (camState == CamStates.Target)
            {
                camState = CamStates.Behind;
            }
        }

        // Execute camera state:
        switch (camState)
        {
            case CamStates.Behind:
                ResetCamera();

                // Only update camera look direction if moving
                if (player.Speed >= CharController.speedThreshold)
                {
                    lookDir = player.transform.forward;
                    Debug.DrawRay(transform.position, lookDir, Color.white);

                    // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude
                    curLookDir = Vector3.Normalize(characterOffset - transform.position);
                    curLookDir.y = 0;
                    Debug.DrawRay(transform.position, curLookDir, Color.green);

                    // Damping makes it so we don't update targetPosition while pivoting; camera shouldn't rotate around player
                    curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
                }

                targetPosition = characterOffset + player.transform.up * distanceUp - Vector3.Normalize(curLookDir) * distanceAway;
                Debug.DrawLine(player.transform.position, targetPosition, Color.magenta);

                break;
            case CamStates.Target:
                ResetCamera();
                lookDir = player.transform.forward;
                curLookDir = player.transform.forward;

                targetPosition = characterOffset + player.transform.up * distanceUp - lookDir * distanceAway;

                break;
            case CamStates.Free:
                // Move height and distance from character in separate parentRig transform since RotateAround has control of both position and rotation
                Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - transform.position);
                // Can't calculate distanceAway from a vector with Y axis rotation in it; zero it out
                rigToGoalDirection.y = 0f;

                Vector3 rigToGoal = characterOffset - parentRig.position;
                rigToGoal.y = 0;
                Debug.DrawRay(parentRig.transform.position, rigToGoal, Color.red);

                // Panning in and out
                // If statement works for positive values; don't tween if stick not increasing in either direction; also don't tween if user is rotating
                // Checked against rightStickThreshold because very small values for rightY mess up the Lerp function
                if (cameraY < -1f * rightStickThreshold && cameraY <= rightStickPrevFrame.y && Mathf.Abs(cameraX) < rightStickThreshold)
                {
                    distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs(cameraY));
                    distanceAwayFree = Mathf.Lerp(distanceAway, distanceAway * distanceAwayMultipler, Mathf.Abs(cameraY));
                    targetPosition = characterOffset + player.transform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
                }
                else if (cameraY > rightStickThreshold && cameraY >= rightStickPrevFrame.y && Mathf.Abs(cameraX) < rightStickThreshold)
                {
                    // Subtract height of camera from height of player to find Y distance
                    distanceUpFree = Mathf.Lerp(Mathf.Abs(transform.position.y - characterOffset.y), camMinDistFromChar.y, cameraY);
                    // Use magnitude function to find X distance	
                    distanceAwayFree = Mathf.Lerp(rigToGoal.magnitude, camMinDistFromChar.x, cameraY);

                    targetPosition = characterOffset + player.transform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
                }
                // Store direction only if right stick inactive
                if (cameraX != 0 || cameraY != 0)
                {
                    savedRigToGoal = rigToGoalDirection;
                }


                // Rotating around character
                parentRig.RotateAround(characterOffset, player.transform.up, freeRotationDegreePerSecond * (Mathf.Abs(cameraX) > rightStickThreshold ? cameraX : 0f));

                // Still need to track camera behind player even if they aren't using the right stick; achieve this by saving distanceAwayFree every frame
                if (targetPosition == Vector3.zero)
                {
                    targetPosition = characterOffset + player.transform.up * distanceUpFree - savedRigToGoal * distanceAwayFree;
                }

                //				SmoothPosition(transform.position, targetPosition);
                //				transform.LookAt(lookAt);	
                break;
        }



        CompensateForWalls(characterOffset, ref targetPosition);
        SmoothPosition(parentRig.position, targetPosition);
        transform.LookAt(lookAt);

        rightStickPrevFrame = new Vector2(cameraX, cameraY);
    }


    private void SmoothPosition(Vector3 fromPos, Vector3 toPos)
    {
        // Making a smooth transition between camera's current position and the position it wants to be in
        parentRig.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
    {

        // Compensate for walls between camera
        RaycastHit wallHit = new RaycastHit();
        int layerMask = 1 << Layers.environment;
        if (Physics.Linecast(fromObject, toTarget, out wallHit, layerMask))
        {
            Debug.DrawRay(wallHit.point, wallHit.normal, Color.red);
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
        }
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
    }


    private void ResetCamera()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
    }
}
