using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WPGameController : MonoBehaviour
{

	[FormerlySerializedAs("countdown")] public WPCountdown wpCountdown;
	[FormerlySerializedAs("mapController")] public WPMapController wpMapController;

	public Text countdownText;
	public Text finishText;
	public GameObject textPanel;

	void Start()
	{
		wpMapController.LevelComplete += HandleLevelComplete;
		wpCountdown.NoTimeLeft += HandleGameOver;
		wpCountdown.OnTimeChange += SetWpCountdownText;

		textPanel.SetActive(false);
	}

	private void SetWpCountdownText(float s)
	{
		countdownText.text = ((int)s).ToString();
	}

	private void HandleLevelComplete()
	{
		Time.timeScale = 0;
		textPanel.SetActive(true);
		finishText.text = "JE HEBT GEWONNEN!";
	}

	private void HandleGameOver()
	{
		Time.timeScale = 0;
		textPanel.SetActive(true);
		finishText.text = "Helaas, je hebt verloren...";
	}

}