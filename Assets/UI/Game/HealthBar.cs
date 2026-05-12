using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	[SerializeField] float healthUpdateSpeed = 10f;
	private float defaultPos;
	Image img;
	Player player;


	private void Start()
	{
		img = GetComponent<Image>();
		defaultPos = img.rectTransform.anchoredPosition.x;

		player = GameObject.FindWithTag("Player").GetComponent<Player>();
	}


	float healthPercent;
	private void Update()
	{
		// Get health value
		var targetHealthPercent = Mathf.Clamp01((float)player.health / player.maxHealth);
		healthPercent = Mathf.Lerp(healthPercent, targetHealthPercent, Time.deltaTime * healthUpdateSpeed);

		// Update health bar UI
		if (healthPercent == 0)
			img.enabled = false;
		else
		{
			img.enabled = true;
			var trs = img.rectTransform;
			var halfWidth = trs.sizeDelta.x / 2f;
			img.rectTransform.anchoredPosition = new (-halfWidth * (1 - healthPercent) + defaultPos, trs.anchoredPosition.y);
			trs.localScale = new (healthPercent, trs.localScale.y, trs.localScale.z);
		}

	}
}
