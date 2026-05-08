using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

	[SerializeField] float moveSpeed = 3f;
	[SerializeField] float jumpVelocity = 5f;
	[SerializeField] float mouseSensitivity = 1f;
	float maxVerticalRotation = 80f;
	

	Camera cam;
	Rigidbody body;
	InputActionMap input; 
	CapsuleCollider hitbox;
	Timer jumpBuffer;

	LayerMask terrain;

	private void Start()
	{
		// Connect inputs
		input = InputSystem.actions.actionMaps.First(m => m.name == "Player");
		input["Jump"].performed += OnJumpPressed;

		// Get components
		body = GetComponent<Rigidbody>();
		hitbox = GetComponent<CapsuleCollider>();
		cam = transform.Find("Camera").GetComponent<Camera>();
		jumpBuffer = GetComponent<Timer>();
	
		// Physics
		terrain = LayerMask.GetMask("Terrain");
		print(LayerMask.LayerToName(terrain));

	}


	private void Update()
	{
		HandleLooking();
		print(IsOnFloor());
	}

	private void FixedUpdate()
	{
		HandleMovement();
	}

	private void OnJumpPressed(InputAction.CallbackContext ctx)
	{
		jumpBuffer.Run();
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

	private bool IsOnFloor()
		=> Physics.SphereCast(new (transform.position, Vector3.down), hitbox.radius, (hitbox.height / 2f) - (hitbox.radius - 0.05f), terrain);

		


}
