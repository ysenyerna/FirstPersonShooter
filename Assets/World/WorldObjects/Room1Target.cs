using UnityEngine;

public class Room1Target : MonoBehaviour, IShootable
{


	Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void WasShot()
	{
		anim.Play("Room1DoorOpen");
	}



}
