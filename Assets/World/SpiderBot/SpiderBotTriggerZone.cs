using UnityEngine;

public class SpiderBotTriggerZone : MonoBehaviour
{

	public SpiderBot[] spiderBots;

	bool triggered = false;

	public void OnTriggerEnter(Collider collider)
	{
		if (triggered || !collider.CompareTag("Player"))
			return;

		triggered = true;

		foreach (var bot in spiderBots)
		{
			if (bot == null)
				continue;

			bot.Aggro = true;
		}

	}

}
