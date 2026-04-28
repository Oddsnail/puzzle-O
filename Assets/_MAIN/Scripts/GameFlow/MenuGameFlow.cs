using UnityEngine;
using origin.audio;
using System.Collections;
using UnityEngine.UI;

public class MenuGameFlow : MonoBehaviour
{
	public Button continueButton;
	public Button[] storyButtons;
	private const string storySawPrefix = "SawStory.";

	void Start() {
		RefreshContinueButton();
		StartCoroutine(bgm());
	}

	private IEnumerator bgm() {
		yield return new WaitForSeconds(0.2f);
		AudioManager.instance.PlaySoundGradient("bgm/spaceship_earth", AudioManager.instance.musicMixer, 1, 1, true);
	}

	public void RefreshContinueButton() {
		continueButton.gameObject.SetActive(PlayerPrefs.GetInt(storySawPrefix + "1", 0) == 1);
		int i = 1;
		foreach (Button button in storyButtons) {
			button.interactable = PlayerPrefs.GetInt(storySawPrefix + $"{i}", 0) == 1;
			i++;
		}
	}

	public void SetDestinationStory(int index) {
		PlayerPrefs.SetInt("_DEST", index);
		PlayerPrefs.Save();
	}
}
