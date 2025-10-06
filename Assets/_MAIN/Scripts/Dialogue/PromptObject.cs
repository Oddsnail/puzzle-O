using TMPro;
using UnityEngine;

public class PromptObject : MonoBehaviour
{
    private RectTransform root;

	[SerializeField] private Animator anim;
	[SerializeField] private TextMeshProUGUI tmpro;

	public bool isShowing => anim.gameObject.activeSelf;

	void Start() {
		root = GetComponent<RectTransform>();
	}
}
