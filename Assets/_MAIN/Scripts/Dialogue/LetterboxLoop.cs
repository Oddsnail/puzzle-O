using UnityEngine;
using UnityEngine.UI;

public class LetterboxLoop : MonoBehaviour
{
    [Range(-1f, 1f)]
    public float speed = 0.5f;
    private float offset;
    private Material material;
	private bool copied = false;

    void Start() {
		Image image = gameObject.GetComponent<Image>();
		if (image != null) {
			material = new(image.material);
			image.material = material;
			copied = true;
		} else {
			Debug.LogError("[ERROR] : no image found for letterbox.");
		}
    }

    void Update() {
		if (copied) {
			offset += Time.deltaTime * speed / 10f;
			material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		}
    }

	void OnDestroy() {
		if (copied) Destroy(material);
	}
}
