using UnityEngine;

public class CombatRoom : MonoBehaviour
{

	const float OPEN_DOOR_ELEVATION = -4.45f;
	const float CLOSED_DOOR_ELEVATION = -1.7f;
	float currentElevation = OPEN_DOOR_ELEVATION;

	[SerializeField] Transform[] doors;

	bool inCombat = false;
	bool combatFinished = false;



	private void Start()
	{
		foreach (var bot in GetComponent<SpiderBotTriggerZone>().spiderBots)
		{
			bot.Died += EnemyDied;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (combatFinished || !collider.CompareTag("Player"))
			return;

		// Trigger the combat room
		inCombat = true;
	}



	private void Update()
	{

		if (inCombat && currentElevation < CLOSED_DOOR_ELEVATION)
		{
			currentElevation = Mathf.Lerp(currentElevation, CLOSED_DOOR_ELEVATION, Time.deltaTime * 5f);
		}
		else if (!inCombat && currentElevation > OPEN_DOOR_ELEVATION)
		{
			currentElevation = Mathf.Lerp(currentElevation, OPEN_DOOR_ELEVATION, Time.deltaTime * 5f);
		}

		foreach (var door in doors)
		{
			var pos = door.transform.localPosition;
			door.transform.localPosition = new (pos.x, currentElevation, pos.z);
		}
		
	}


	int enemyDiedCount = 0;
	private void EnemyDied()
	{
		enemyDiedCount += 1;
		if (enemyDiedCount >= 4)
		{
			inCombat = false;
			combatFinished = true;
		}
			
	}


}
