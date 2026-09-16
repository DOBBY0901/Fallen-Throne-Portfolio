using System.Collections;
using UnityEngine;

public class RiseFromGround : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float riseHeight = 3f;
    [SerializeField] private float riseDuration = 1.5f;

    [Header("Shake")]
    [SerializeField] private float shakeAmount = 0.1f;
    [SerializeField] private float shakeDuration = 0.5f;

    [Header("Delay")]
    [SerializeField] private float startDelay;

    private Vector3 finalPosition;
    private Vector3 startPosition;
    private Coroutine riseCoroutine;

    private void OnEnable()
    {
        finalPosition = transform.position;
        startPosition = finalPosition - Vector3.up * riseHeight;
        transform.position = startPosition;

        riseCoroutine = StartCoroutine(RiseRoutine());
    }

    private void OnDisable()
    {
        if (riseCoroutine != null)
        {
            StopCoroutine(riseCoroutine);
            riseCoroutine = null;
        }
    }

    private IEnumerator RiseRoutine()
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            Vector3 offset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                0f,
                Random.Range(-shakeAmount, shakeAmount)
            );

            transform.position = startPosition + offset;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;

            float t = riseDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / riseDuration);

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position =
                Vector3.Lerp(startPosition, finalPosition, t);

            yield return null;
        }

        transform.position = finalPosition;
        riseCoroutine = null;
    }
}
