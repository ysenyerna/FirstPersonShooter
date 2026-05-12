using System;
using UnityEngine;

public class SpiderBot : MonoBehaviour, IShootable
{
	public float speed = 1f;
	public GameObject explodable;
	public GameObject explosion;
	public Action Died;

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
	Vector3 currentDirection = new();


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
		if (Aggro && !dead)
		{
			currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.deltaTime * 5f);
			currentDirection = Vector3.Normalize(Player.current.transform.position - transform.position);
		}
		else  if (dead)
		{
			currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 5f);
		}
		else
		{
			currentSpeed = 0f;
		}
		var velocity = currentDirection * currentSpeed;
		body.position += new Vector3 (velocity.x, 0f, velocity.z);

		// Set light color
		redLight.intensity = Aggro ? Mathf.Lerp(redLight.intensity, 0.5f, Time.deltaTime * 2f) : Mathf.Lerp(redLight.intensity, 0f, Time.deltaTime * 2f);

	}


	public float deathExplosionForce = 10f;
	public float deathExplosionRadius = 5f;
	public void WasShot()
	{
		currentSpeed = -speed * 4;
		if (!Aggro)
			Aggro = true;

		// Take damage
		health -= 1;
		if (health <= 0)
		{
			Died?.Invoke();
			// Trigger explosion
			Instantiate(explosion, transform.position, transform.rotation);
			var duplicate = Instantiate(explodable, transform.position, transform.rotation);
			foreach (var part in duplicate.GetComponentsInChildren<Rigidbody>())
			{
				print(part.name);
				part.AddExplosionForce(deathExplosionForce, transform.position, deathExplosionRadius);
			}
			Destroy(gameObject);

		}
	}

}
