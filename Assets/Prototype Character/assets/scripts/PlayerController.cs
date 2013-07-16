using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 1;
    public float normalSpeed = 0F;
    public bool sprintEnabled = false;
    public float cooldownDash = 5;
    private float passedTimeCoolDown = 0;
    public float duration = 3;
    private float passedTimeSprint = 0;
    private bool checkSchlaganimState = true;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        passedTimeCoolDown = cooldownDash;
        GameObject.FindGameObjectWithTag("Schlagarm").animation.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.FindChild("cameraPivot").FindChild("cameraCorrection").FindChild("GUI").gameObject.activeInHierarchy)
        {
            //Blickrichtung der Kamera
            Vector3 forward = transform.FindChild("cameraPivot").FindChild("cameraCorrection").FindChild("Camera").TransformDirection(Vector3.forward);
            //Höhe ist egal
            forward.y = 0;
            forward.Normalize();
            Vector3 right = new Vector3(forward.z, 0, -forward.x);

            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            Vector3 targetDirection = h * right + v * forward;
            targetDirection = targetDirection.normalized * normalSpeed;
            targetDirection.y = -1;
            controller.Move(targetDirection);

            SprintLogic();
            SchlagAnim();
        }
    }

    void SchlagAnim()
    {
        GameObject myHitArm = transform.FindChild("Schlagarm_Pivot").gameObject;

        if (myHitArm.animation.isPlaying && checkSchlaganimState && Input.GetMouseButtonDown(0))
        {
            GetComponent<HitAnimationLogic>().HitArmAnimationRewind();
        }

        if (Input.GetMouseButtonDown(0) && !myHitArm.animation.isPlaying && checkSchlaganimState)
        {
            GetComponent<HitAnimationLogic>().HitArmAnimationPlay();
            checkSchlaganimState = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            checkSchlaganimState = true;
        }
    }

    void SprintLogic()
    {
        passedTimeSprint += Time.deltaTime;
        passedTimeCoolDown += Time.deltaTime;

        if (!sprintEnabled && Input.GetKeyDown(KeyCode.Space) && passedTimeCoolDown >= cooldownDash)
        {
            passedTimeSprint = 0;
            passedTimeCoolDown = 0;
            normalSpeed = speed * 3;
        }
        if (!sprintEnabled && passedTimeSprint >= duration)
            normalSpeed = speed;

        if (sprintEnabled && Input.GetKey(KeyCode.Space))
        {
            normalSpeed = speed * 3;
        }
        else if (sprintEnabled && normalSpeed != speed)
            normalSpeed = speed;
    }

}
