using UnityEngine;

public class SpiderBot : MonoBehaviour, IShootable
{
	public float speed = 1f;

	Transform head;
	Rigidbody body;
	Animator anim;
	Light redLight;
	bool dead = false;

	private bool _aggro = false;
	public bool Aggro { get { return _aggro; } set { 
		if (value)
			anim.Play("SpiderBotMove"); 
		else
			anim.Play("Empty");
		_aggro = value; }}

	private float currentSpeed = 0f;


	public int health = 5;

	private void Start()
	{
		head = transform.Find("Model/Bone/Head");
		body = GetComponent<Rigidbody>();
		anim = transform.Find("Model").GetComponent<Animator>();
		redLight = head.Find("Light").GetComponent<Light>();
	}


	private void Update()
	{

		// Rotate the head
		if (Aggro)
		{
			var dir = Player.current.transform.position - head.transform.position;
			head.transform.forward = Vector3.Lerp(head.transform.forward, dir, Time.deltaTime * 5f);
		}



	}

	private void FixedUpdate()
	{
		// Move
		if (Aggro || dead)
		{
			currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * 5f);
			var dir = Vector3.Normalize(Player.current.transform.position - transform.position);
			var velocity = dir * currentSpeed;
			body.position += new Vector3 (velocity.x, 0f, velocity.z);
		}

		redLight.intensity = Aggro ? Mathf.Lerp(redLight.intensity, 0.5f, Time.deltaTime * 2f) : Mathf.Lerp(redLight.intensity, 0f, Time.deltaTime * 2f);

	}

	public void WasShot()
	{
		currentSpeed = -speed * 2;
		if (!Aggro)
			Aggro = true;

		// Take damage
		health -= 1;
		if (health <= 0)
		{
			dead = true;
			currentSpeed = -speed * 4;
			Aggro = false;
			body.freezeRotation = false;
		}
	}

}
