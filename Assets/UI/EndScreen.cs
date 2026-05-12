using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{

	[SerializeField] List<Object> endScreenFadeObjects;
	private bool gameEnded = false;

	private float endScreenFadePercent = 0;
	private float oldEndScreenPercent = -1;

	private TMP_Text endScreenMessage;
	private SpriteButton restartButton;
	private Player player;


	private void Start()
	{
		endScreenMessage = transform.Find("Message").GetComponent<TMP_Text>();
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
		player.GameEnded += Player_GameEnded;
		restartButton = transform.Find("RestartButton").GetComponent<SpriteButton>();
		restartButton.Pressed += RestartButton_Pressed;
		restartButton.enabled = false;
	}

	private void Update()
	{

		// Enable/Disable end screen
		const float fadeSpeed = 10f;
		if (gameEnded && endScreenFadePercent < 1f)
		{
			endScreenFadePercent = Mathf.Lerp(endScreenFadePercent, 1f, fadeSpeed * Time.deltaTime);
		}
		else if (endScreenFadePercent > 0f)
		{
			endScreenFadePercent = Mathf.Lerp(endScreenFadePercent, 0f, fadeSpeed * Time.deltaTime);
		}


		// Update end screen fade
		endScreenFadePercent = Mathf.Clamp01(endScreenFadePercent);
		if (endScreenFadePercent != oldEndScreenPercent)
		{
			oldEndScreenPercent = endScreenFadePercent;

			foreach (object obj in endScreenFadeObjects)
			{
				if (obj is Image i)
					i.color = new (i.color.r, i.color.g, i.color.b, endScreenFadePercent);
				if (obj is TMP_Text t)
					t.color = new (t.color.r, t.color.g, t.color.b, endScreenFadePercent);
			}
		}
	}

	private void RestartButton_Pressed()
	{
		SceneManager.LoadScene("World");
	}

	private void Player_GameEnded()
	{
		endScreenMessage.text = player.IsDead ? "You Died!" : "You Win!";
		gameEnded = true;
		restartButton.enabled = true;
	}

}
