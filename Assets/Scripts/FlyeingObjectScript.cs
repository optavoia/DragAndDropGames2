using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlyeingObjectScript : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;
    public float fadeDuration = 1.5f;
    public float waveAmplitude = 25f;
    public float waveFrequency = 1f;
    private ObjectScript objectScript;
    private ScreenBoundriesScript screenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private Image image;
    private Color originalColor;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;
        objectScript = FindAnyObjectByType<ObjectScript>();
        screenBoundriesScript = FindAnyObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);

        // <-
        if(speed > 0 && transform.position.x < (screenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        // ->
        if (speed < 0 && transform.position.x > (screenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        if(ObjectScript.drag && !isFadingOut && RectTransformUtility.RectangleContainsScreenPoint
            (rectTransform, Input.mousePosition, Camera.main))
        {
            Debug.Log("The cursor collided with flying object!");
            //...........

            if(ObjectScript.lastDragged != null)
            {
                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            image.color = Color.cyan;
            StartCoroutine(RecoverColor());

            objectScript.effects.PlayOneShot(objectScript.audioCli[4]);
            StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elapsed = 0f;
        float intensity = 5f;

        while(elapsed < duration)
        {
            rectTransform.anchoredPosition =
                originalPosition + Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        float StartAlpha = canvasGroup.alpha;

        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(StartAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 originalScale = target.transform.localScale;
        Quaternion originalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }
        Destroy(target);
    }

    IEnumerator RecoverColor()
    {
        yield return new WaitForSeconds(0.5f);
        image.color = originalColor;
    }
}
