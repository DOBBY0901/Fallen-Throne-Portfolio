using System.Collections;
using UnityEngine;

public class FlameTrap : MonoBehaviour
{
    private static readonly int EmissionColorId =
        Shader.PropertyToID("_EmissionColor");

    [Header("References")]
    [SerializeField] private Renderer trapRenderer;
    [SerializeField] private GameObject flameGroup;
    [SerializeField] private Light flameLight;
    [SerializeField] private AudioClip flameClip;

    [Header("Emission")]
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color warningColor =
        new Color(1f, 0.45f, 0f);
    [SerializeField] private Color dangerColor = Color.red;

    [Header("Timing")]
    [SerializeField] private float idleMinTime = 1f;
    [SerializeField] private float idleMaxTime = 3f;
    [SerializeField] private float warningTime = 1f;
    [SerializeField] private float dangerTime = 0.5f;
    [SerializeField] private float flameTime = 1.5f;

    private Material runtimeMaterial;
    private Coroutine trapCoroutine;

    private void Awake()
    {
        if (trapRenderer != null)
            runtimeMaterial = trapRenderer.material;

        ResetVisualState();
    }

    private void Start()
    {
        StartTrap();
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }

    public void StopTrap()
    {
        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
            trapCoroutine = null;
        }

        ResetVisualState();
        enabled = false;
    }

    private void StartTrap()
    {
        if (trapCoroutine == null && enabled)
            trapCoroutine = StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        while (true)
        {
            float idleTime = Random.Range(
                Mathf.Min(idleMinTime, idleMaxTime),
                Mathf.Max(idleMinTime, idleMaxTime)
            );

            yield return new WaitForSeconds(idleTime);

            yield return ChangeEmission(
                idleColor,
                warningColor,
                warningTime,
                1f,
                2f
            );

            yield return ChangeEmission(
                warningColor,
                dangerColor,
                dangerTime,
                2f,
                4f
            );

            SetFlameActive(true);
            PlayFlameAudio();

            yield return new WaitForSeconds(
                Mathf.Max(0f, flameTime)
            );

            SetFlameActive(false);

            yield return ChangeEmission(
                dangerColor,
                idleColor,
                0.4f,
                4f,
                1f
            );
        }
    }

    private IEnumerator ChangeEmission(
        Color startColor,
        Color endColor,
        float duration,
        float startPower,
        float endPower)
    {
        if (duration <= 0f)
        {
            SetEmission(endColor, endPower);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            Color color = Color.Lerp(startColor, endColor, t);
            float power = Mathf.Lerp(startPower, endPower, t);

            SetEmission(color, power);
            yield return null;
        }

        SetEmission(endColor, endPower);
    }

    private void SetFlameActive(bool active)
    {
        if (flameGroup != null)
            flameGroup.SetActive(active);

        if (flameLight != null)
            flameLight.enabled = active;
    }

    private void ResetVisualState()
    {
        SetFlameActive(false);
        SetEmission(idleColor, 1f);
    }

    private void SetEmission(Color color, float power)
    {
        if (runtimeMaterial == null)
            return;

        runtimeMaterial.SetColor(
            EmissionColorId,
            color * power
        );
    }

    private void PlayFlameAudio()
    {
        if (flameClip != null)
        {
            AudioManager.Instance?.Play3DSfx(
                flameClip,
                transform.position
            );
        }
    }
}
