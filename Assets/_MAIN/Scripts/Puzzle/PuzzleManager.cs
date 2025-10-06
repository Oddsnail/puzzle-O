using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

	public static PuzzleManager instance;

	public PuzzleContainer puzzleContainer = new();
	public RectTransform sofa;

	public Color strike;
	public Color semiStrike;
	public Color ball;

	public delegate void PuzzleEvent(char guess);
	public event PuzzleEvent onCharacterGuess;

	private Coroutine co_animating = null;
	public bool isAnimating => co_animating != null;

	bool initialized = false;
	bool guessing = false;

	public void Awake() {
		if (instance == null) {
			instance = this;
			Initialize();
		} else DestroyImmediate(gameObject);
	}

	public void Initialize() {
		if (initialized) return;

		initialized = true;
		onCharacterGuess += CharacterTaken;
	}

	public void OnCharacterGuess(char guess) => onCharacterGuess.Invoke(guess);

	//========================================================================
	//                       <!!!> Puzzle system <!!!>
	//========================================================================

	private const string Starter = "<mspace=0.51em>";
	private const string Ender = "</mspace>";
	private const string TABLE = "ASDFGQWERT";
	private char recentChoice = ' ';

	private void UpdateHistory(string history) => puzzleContainer.historyText.text = Starter + history + Ender;

    public IEnumerator StartPuzzle(string charID, int trial, Action<bool> onResult) {

		string ID = charID + "-client";
		puzzleContainer.trialsText.text = $"{trial}/{trial}";

		if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
		CHARACTER character = CharacterManager.instance.GetCharacter(ID);

		Color theme = character.config.themeColor;
		puzzleContainer.puzzleColor.color = theme;
		yield return new WaitForSeconds(0.7f);

		string answer = GenerateUniqueFourDigitNumber();
		string history = "";
		UpdateHistory(history);
		
		bool successed = false;
		

		Debug.Log($"answer is : {answer}");

		Show();
		character.Appear();

		for (int i = 0; i < trial; i++) {
			int score = 0;
			for (int choice = 0; choice < 4; choice ++) {
				guessing = true;

				yield return new WaitUntil(() => recentChoice != ' ');
				
				if (answer[choice] == recentChoice) {
					// RED
					history += $"<color=#{ColorUtility.ToHtmlStringRGB(strike)}>" + recentChoice + "</color> ";
					AudioManager.Instance.PlaySoundEffect("SFX/puzzle-strike", pitch: 1.3f);
					score += 1;
					character.Shiver();
				} else if (TABLE.IndexOf(answer[choice]) % 5 == TABLE.IndexOf(recentChoice) % 5) {
					// PURPLE
					history += $"<color=#{ColorUtility.ToHtmlStringRGB(semiStrike)}>" + recentChoice + "</color> ";
					AudioManager.Instance.PlaySoundEffect("SFX/puzzle-strike", pitch: 1.0f);
				} else if (answer.Contains(recentChoice)) {
					// BLUE
					history += $"<color=#{ColorUtility.ToHtmlStringRGB(ball)}>" + recentChoice + "</color> ";
					AudioManager.Instance.PlaySoundEffect("SFX/puzzle-strike", pitch: 0.7f);
				} else {
					// GRAY
					history += $"<color=#BBBBBB>" + recentChoice + "</color> ";
					AudioManager.Instance.PlaySoundEffect("SFX/puzzle-fail", pitch: 1.0f);
				}

				UpdateHistory(history);

				guessing = false;
				recentChoice = ' ';
			}
			history += "\n";
			puzzleContainer.trialsText.text = $"{trial - 1 - i}/{trial}";
			AudioManager.Instance.PlaySoundEffect("SFX/dialogue-3");

			if (score == 4) {
				successed = true;
				break;
			}
		}

		yield return new WaitForSeconds(0.1f);
		Hide();
		character.Disappear();
		onResult?.Invoke(successed);
		yield return new WaitForSeconds(0.1f);
	}

	private void CharacterTaken(char guess) {
		if (guessing) recentChoice = guess;
	}

	public static string GenerateUniqueFourDigitNumber() {
        List<char> numbers = new();
        HashSet<char> uniqueness = new();

        while (numbers.Count < 4)
        {
            int digit = UnityEngine.Random.Range(0, 10);
            if (!uniqueness.Contains(TABLE[digit])) {
				numbers.Add(TABLE[digit]);
				uniqueness.Add(TABLE[digit]);
			}
        }

        return new(numbers.ToArray());
    }

	//========================================================================
	//        <!!!> show / hide puzzle UI by puzzle start / end <!!!>
	//========================================================================

	private const float defaultShowAndHideDuration = 0.6f;
	private const float defaultSofaYPos = -300.0f;
	private const float defaultContainerXPos = 1700.0f;

	private void Show() {
		if(isAnimating) return;

		co_animating = StartCoroutine(Showing());
	}

	private void Hide() {
		if(isAnimating) return;
		co_animating = StartCoroutine(Hiding());
	}

	private IEnumerator Showing() {
		sofa.gameObject.SetActive(true);
		puzzleContainer.containerRoot.gameObject.SetActive(true);

		yield return DOTween.Sequence()
			.Join(sofa.DOAnchorPosY(0.0f, defaultShowAndHideDuration))
			.Join(puzzleContainer.containerRoot.DOAnchorPosX(0.0f, defaultShowAndHideDuration))
			.WaitForCompletion();

		co_animating = null;
	}

	private IEnumerator Hiding() {

		yield return DOTween.Sequence()
			.Join(sofa.DOAnchorPosY(defaultSofaYPos, defaultShowAndHideDuration))
			.Join(puzzleContainer.containerRoot.DOAnchorPosX(defaultContainerXPos, defaultShowAndHideDuration))
			.WaitForCompletion();

		sofa.gameObject.SetActive(false);
		puzzleContainer.containerRoot.gameObject.SetActive(false);
		co_animating = null;
	}

	//========================================================================
	//========================================================================
}
