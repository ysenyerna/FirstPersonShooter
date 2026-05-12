using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] float moveSpeed = 3f;
	[SerializeField] float jumpVelocity = 5f;
	[SerializeField] float mouseSensitivity = 1f;
	float maxVerticalRotation = 80f;
	
	[SerializeField] GameObject bulletImpactFx;

	Camera cam;
	Rigidbody body;
	InputActionMap input; 
	CapsuleCollider hitbox;
	Timer jumpBuffer;
	Animator gunAnim;
	ParticleSystem muzzleFlash;

	private bool controllingCamera = false;
	private bool canShoot = true;
	LayerMask terrain;

	public static Player current { get; private set; }

	private void Start()
	{
		current = this;
		// Connect inputs
		input = InputSystem.actions.actionMaps.First(m => m.name == "Player");
		input["Jump"].performed += OnJumpPressed;
		input["Attack"].performed += OnShootPressed;
		input["Escape"].performed += OnEscapePressed;

		// Get components
		body = GetComponent<Rigidbody>();
		hitbox = GetComponent<CapsuleCollider>();
		cam = transform.Find("Camera").GetComponent<Camera>();
		jumpBuffer = GetComponent<Timer>();
		gunAnim = transform.Find("Camera/Gun").GetComponent<Animator>();
		muzzleFlash = gunAnim.transform.Find("MuzzleFlash").GetComponent<ParticleSystem>();
	
		// Physics
		terrain = LayerMask.GetMask("Terrain");

	}


	private void Update()
	{
		if (controllingCamera)
			HandleLooking();
	}

	private void FixedUpdate()
	{
		HandleMovement();
	}

	private void HandleLooking()
	{
		Vector2 lookVelocity = input["Look"].ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

		// Handle rotation
		var horizontalRotation = transform.eulerAngles.y + lookVelocity.x;
		transform.eulerAngles = new (transform.eulerAngles.x, horizontalRotation, 0);
	
		var verticalRotation = cam.transform.eulerAngles.x - lookVelocity.y;
		if (verticalRotation >= 180) verticalRotation -= 360f;
		verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalRotation, maxVerticalRotation);
		cam.transform.eulerAngles = new (verticalRotation, cam.transform.eulerAngles.y, 0);

	}

	private void HandleMovement()
	{
		// Horizontal movement
		Vector2 dir = input["Move"].ReadValue<Vector2>();
		var velocity = (transform.forward * dir.y + transform.right * dir.x) * moveSpeed * Time.deltaTime;

		body.position += velocity;

		// Jumping
		if (jumpBuffer.Running && IsOnFloor())
		{
			jumpBuffer.Stop();
			var currentVelocity = body.linearVelocity;
			body.linearVelocity = new(currentVelocity.x, jumpVelocity, currentVelocity.z);
		}

	}

	public Mesh mesh;
	private void Shoot()
	{
		// Enable camera -- temporary for testing
		controllingCamera = true;
		Cursor.lockState = CursorLockMode.Locked;

		// Shoot
		canShoot = false;
		gunAnim.Play("GunShoot");
		muzzleFlash.Play();


		// Make raycast
		var hit = Physics.Raycast(new (cam.transform.position, cam.transform.forward), out var info);
		if (hit)
		{
			// Create impact effect
			var effect = bulletImpactFx;
			if (info.collider.TryGetComponent<CustomBulletImpactEffect>(out var c))
				effect = c.customEffect;

			var rotation = Vector3.Normalize(cam.transform.position - info.point);
			Instantiate(effect, info.point, Quaternion.LookRotation(rotation));

			// Trigger IShootable event
			if (info.collider.TryGetComponent<IShootable>( out var shootable ))
			{
				shootable.WasShot();
			}
		}
		
		
	}




	// HELPER METHODS
	private bool IsOnFloor()
		=> Physics.SphereCast(new (transform.position, Vector3.down), hitbox.radius, (hitbox.height / 2f) - (hitbox.radius - 0.05f), terrain);


		

	// EVENTS
	private void OnJumpPressed(InputAction.CallbackContext ctx)
	{
		jumpBuffer.Run();
	}

	private void OnShootPressed(InputAction.CallbackContext ctx)
	{
		if (canShoot)
			Shoot();
	}

	public void ShootAnimationFinished()
	{
		canShoot = true;
	}


	private void OnEscapePressed(InputAction.CallbackContext ctx)
	{
		controllingCamera = false;
		Cursor.lockState = CursorLockMode.None;
	}


}
