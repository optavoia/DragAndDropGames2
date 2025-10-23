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
    private ScreenBoundriesScript scrreenBoundriesScript;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isFadingOut = false;
    private bool isExploading = false;
    private Image image;
    private Color originalColor;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();

        image = GetComponent<Image>();
        originalColor = image.color;
        objectScript = FindFirstObjectByType<ObjectScript>();
        scrreenBoundriesScript = FindFirstObjectByType<ScreenBoundriesScript>();
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.gameActive)
            return;

        float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, waveOffset * Time.deltaTime);
        // <-
        if (speed > 0 && transform.position.x < (scrreenBoundriesScript.minX + 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        // ->
        if (speed < 0 && transform.position.x > (scrreenBoundriesScript.maxX - 80) && !isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;
        }

        Vector2 inputPosition;
        if (!TryGetInputPosition(out inputPosition))
            return;
        ///////////////////////////
        if (CompareTag("bomb") && !isExploading &&
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, inputPosition, Camera.main))
        {
            Debug.Log("The cursor collided with a bomb! (without car)");
            TriggerExplosion();

        }

        bool TryGetInputPosition(out Vector2 position)
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
                position = Input.mousePosition;
                return true;
            #elif UNITY_ANDROID
                if(Input.touchCount > 0)
                {
                    position = Input.GetTouch(0).position;
                    return true;
                }
                else
                {
                    position = Vector2.zero;
                    return false;
                }
            #endif
        }

        // Caurskatīt no šejienes


        if (ObjectScript.drag && !isFadingOut &&
            RectTransformUtility.RectangleContainsScreenPoint(rectTransform, inputPosition, Camera.main))
        {
            if (ObjectScript.lastDragged != null)
            {
                DraggableItem draggable = ObjectScript.lastDragged.GetComponent<DraggableItem>();
                if (draggable != null && draggable.locked)
                {
                    // если машина уже на правильном месте, летающий объект её не трогает
                    return;
                }

                // если не заблокирована — уничтожаем
                StartCoroutine(ShrinkAndDestroy(ObjectScript.lastDragged, 0.5f));
                ObjectScript.lastDragged = null;
                ObjectScript.drag = false;
            }

            StartToDestroy();
        }

    }

    public void TriggerExplosion()
    {
        isExploading = true;
        objectScript.effects.PlayOneShot(objectScript.audioCli[12], 5f);

        if (TryGetComponent<Animator>(out Animator animator))
        {
            animator.SetBool("explode", true);
        }

        image.color = Color.red;
        StartCoroutine(RecoverColor(0.4f));

        StartCoroutine(Vibrate());
        StartCoroutine(WaitBeforeExpload());
    }

    IEnumerator WaitBeforeExpload()
    {
        float radius = 0f;
        if (TryGetComponent<CircleCollider2D>(out CircleCollider2D circleCollider))
        {
            radius = circleCollider.radius * transform.lossyScale.x;
        }
        ExploadAndDestroy(radius);
        yield return new WaitForSeconds(1f);
        ExploadAndDestroy(radius);
        Destroy(gameObject);
    }

    void ExploadAndDestroy(float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.gameObject != gameObject)
            {
                FlyeingObjectScript obj =
                    hitCollider.gameObject.GetComponent<FlyeingObjectScript>();

                if (obj != null && !obj.isExploading)
                {
                    obj.StartToDestroy();
                }
            }
        }
    }

    public void StartToDestroy()
    {
        if (!isFadingOut)
        {
            StartCoroutine(FadeOutAndDestroy());
            isFadingOut = true;

            image.color = Color.cyan;
            StartCoroutine(RecoverColor(0.5f));

            objectScript.effects.PlayOneShot(objectScript.audioCli[11]);

            StartCoroutine(Vibrate());
            GameManager.Instance.CarEaten();
        }
    }

    IEnumerator Vibrate()
    {

        #if UNITY_ANDROID
            Handheld.Vibrate();
        #endif

        Vector2 originalPosition = rectTransform.anchoredPosition;
        float duration = 0.3f;
        float elpased = 0f;
        float intensity = 5f;

        while (elpased < duration)
        {
            rectTransform.anchoredPosition =
                originalPosition + Random.insideUnitCircle * intensity;
            elpased += Time.deltaTime;
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
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    IEnumerator ShrinkAndDestroy(GameObject target, float duration)
    {
        Vector3 orginalScale = target.transform.localScale;
        Quaternion orginalRotation = target.transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.transform.localScale = Vector3.Lerp(orginalScale, Vector3.zero, t / duration);
            float angle = Mathf.Lerp(0f, 360f, t / duration);
            target.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }
        Destroy(target);
    }

    IEnumerator RecoverColor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        image.color = originalColor;
    }
}
