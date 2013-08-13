using UnityEngine;

public class CharController : MonoBehaviour {

	private Animator animator;
	private CharacterController controller;
	private ThirdPersonCamera gamecam;
	private Health health;
	private Speed speed;

	private float _speed = 0f;
	private const float _maxSideSpeed = 0.75f;
	private const float _maxBackSpeed = 0.5f;
	private bool _targeting = false;

	public Skill basicAttack;
	public Skill skill1;
	public Skill skill2;
	public Skill skill3;
	public Skill skill4;
	public Skill heroicAura;

	public const float speedThreshold = 0.1f;
	private const float sprintFOV = 75.0f;
	private const float normalFOV = 60.0f;
	private const float fovDampTime = 3f;

	public float Speed
	{
		get { return _speed; }
	}

	void Start () {
		animator = GetComponent<Animator>();
		health = GetComponent<Health>();
		speed = GetComponent<Speed>();
		controller = GetComponent<CharacterController>();
	}
	

	void Update ()
	{
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
			if(vertical<0)
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
			controller.Move(moveDir*0.1f);
		}
		else
		{
			controller.Move(transform.forward * _speed * 0.1f);
			if (_speed > speedThreshold)
				transform.Rotate(Vector3.up, charAngle);
		}
			
		animator.SetFloat(AnimatorTags.speed, _speed);

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
				basicAttack.Execute();
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

	private Vector3 StickToWorldspace(Vector3 stickDirection, ref float directionOut, ref float speedOut, ref float angleOut)
	{
		FindCamera();
		Vector3 charDirection = transform.forward;
		Vector3 cameraDirection;
		if(_targeting)
			cameraDirection = transform.forward;
		else
			cameraDirection = Vector3.Normalize(gamecam.transform.forward);
		cameraDirection.y = 0.0f;

		speedOut = stickDirection.magnitude;


		Quaternion cameraRotation = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
		Vector3 moveDirection = cameraRotation * stickDirection;

		int axisSign = Vector3.Cross(moveDirection, charDirection).y >= 0 ? -1 : 1;

		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), moveDirection, Color.green);
		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), charDirection, Color.magenta);
		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), stickDirection, Color.blue);
		Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Vector3.up, Color.red);

		float angleRootToMove = Vector3.Angle(charDirection, moveDirection) * axisSign;
		angleOut = angleRootToMove;

		angleRootToMove /= 180f;
		directionOut = angleRootToMove;

		return moveDirection;
	}

	private void FindCamera()
	{
		if (gamecam == null)
			gamecam = GameObject.FindGameObjectWithTag(Tags.camera).GetComponent<ThirdPersonCamera>();
	}
}
