using UnityEngine;

public class SpiderBotTriggerZone : MonoBehaviour
{

	public SpiderBot[] spiderBots;

	public void OnTriggerEnter(Collider collider)
	{
		if (!collider.CompareTag("Player")) 
			return;

		foreach (var bot in spiderBots)
		{
			if (bot == null)
				continue;

			bot.Aggro = true;
		}
	
	Destroy(gameObject);
	}

}
