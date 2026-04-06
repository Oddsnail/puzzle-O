using UnityEngine;

public class LetterboxCamera : MonoBehaviour
{
    [SerializeField] private Color letterboxColor;

    private Camera _cam;
    private static Color boxColor;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        Camera.onPreRender += DrawLetterbox;
    }

    void Start()
    {
        boxColor = letterboxColor;
        EnforceAspect();
    }

    void EnforceAspect()
    {
        float targetAspect = 16f / 9f;
        float screenAspect = (float)Screen.width / Screen.height;

        Rect viewportRect;

        if (Mathf.Approximately(screenAspect, targetAspect))
        {
            viewportRect = new Rect(0, 0, 1, 1);
        }
        else if (screenAspect > targetAspect)
        {
            // Screen is wider → pillarbox (bars on sides)
            float width = targetAspect / screenAspect;
            viewportRect = new Rect((1f - width) / 2f, 0, width, 1);
        }
        else
        {
            // Screen is taller → letterbox (bars top/bottom)
            // This is your 16:10 case
            float height = screenAspect / targetAspect;
            viewportRect = new Rect(0, (1f - height) / 2f, 1, height);
        }

        _cam.rect = viewportRect;
    }

    // Fills the bars with your chosen color instead of the default clear
    static void DrawLetterbox(Camera cam)
    {
        GL.Clear(true, true, boxColor);
    }

    void OnDestroy()
    {
        Camera.onPreRender -= DrawLetterbox;
    }
}