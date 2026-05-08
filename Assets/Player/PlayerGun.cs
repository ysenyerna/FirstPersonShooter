using UnityEngine;

public class PlayerGun : MonoBehaviour
{



	private void ShootAnimationFinished()
	{
		Player.current.ShootAnimationFinished();
	}


}
