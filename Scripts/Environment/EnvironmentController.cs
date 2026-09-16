using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;

public class EnvironmentController : MonoBehaviour
{
    public enum EnvironmentState
    {
        Normal,
        Cave,
        StrongBlizzard,
        Castle
    }

    [Header("Current State")]
    [SerializeField] private EnvironmentState currentState =
        EnvironmentState.Normal;

    [Header("Particles")]
    [SerializeField] private ParticleSystem normalBlizzardParticle;
    [SerializeField] private ParticleSystem strongBlizzardParticle;

    [Header("Fog")]
    [SerializeField] private bool useFog = true;
    [SerializeField] private float caveFogDensity = 0.02f;
    [SerializeField] private float strongBlizzardFogDensity = 0.03f;
    [SerializeField] private float fogLerpSpeed = 2f;

    [Header("Lighting")]
    [SerializeField] private Light sunLight;
    [SerializeField] private float lightLerpSpeed = 2f;

    [Header("Normal Lighting")]
    [SerializeField] private float normalSunIntensity = 1f;
    [SerializeField] private Color normalAmbientColor;
    [SerializeField] private float normalAmbientIntensity = 0.8f;
    [SerializeField] private float normalReflectionIntensity = 1f;

    [Header("Cave Lighting")]
    [SerializeField] private float caveSunIntensity = 1f;
    [SerializeField] private Color caveAmbientColor = Color.black;
    [SerializeField] private float caveAmbientIntensity;
    [SerializeField] private float caveReflectionIntensity = 0.2f;

    [Header("Castle Lighting")]
    [SerializeField] private float castleSunIntensity = 1f;
    [SerializeField] private Color castleAmbientColor = Color.black;
    [SerializeField] private float castleAmbientIntensity;
    [SerializeField] private float castleReflectionIntensity = 0.2f;

    [Header("Player")]
    [SerializeField] private ThirdPersonController playerController;
    [SerializeField] private float normalMoveSpeedMultiplier = 1f;
    [SerializeField] private float strongBlizzardMoveSpeedMultiplier = 0.6f;

    public EnvironmentState CurrentState => currentState;

    private Coroutine fogCoroutine;
    private Coroutine lightCoroutine;

    private float defaultFogDensity;

    private void Awake()
    {
        RenderSettings.fog = useFog;
        defaultFogDensity = RenderSettings.fogDensity;

        ApplyState(currentState);
    }

    public void SetEnvironmentState(EnvironmentState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        ApplyState(currentState);
    }

    public void ForceApplyState(EnvironmentState state)
    {
        currentState = state;
        ApplyState(currentState);
    }

    private void ApplyState(EnvironmentState state)
    {
        UpdateParticles(state);
        UpdatePlayerSpeed(state);
        UpdateAmbientAudio(state);
        UpdateFog(state);
        UpdateLighting(state);
    }

    private void UpdateParticles(EnvironmentState state)
    {
        switch (state)
        {
            case EnvironmentState.Normal:
                PlayParticle(normalBlizzardParticle);
                StopParticle(strongBlizzardParticle);
                break;

            case EnvironmentState.Cave:
            case EnvironmentState.Castle:
                StopParticle(normalBlizzardParticle);
                StopParticle(strongBlizzardParticle);
                break;

            case EnvironmentState.StrongBlizzard:
                StopParticle(normalBlizzardParticle);
                PlayParticle(strongBlizzardParticle);
                break;
        }
    }

    private void UpdatePlayerSpeed(EnvironmentState state)
    {
        if (playerController == null)
            return;

        float multiplier =
            state == EnvironmentState.StrongBlizzard
                ? strongBlizzardMoveSpeedMultiplier
                : normalMoveSpeedMultiplier;

        playerController.SetEnvironmentMoveMultiplier(multiplier);
    }

    private void UpdateAmbientAudio(EnvironmentState state)
    {
        AudioManager.Instance?.SetAmbientState(state);
    }

    private void UpdateFog(EnvironmentState state)
    {
        if (!useFog)
        {
            RenderSettings.fog = false;
            return;
        }

        RenderSettings.fog = true;

        float targetDensity = state switch
        {
            EnvironmentState.Cave => caveFogDensity,
            EnvironmentState.Castle => caveFogDensity,
            EnvironmentState.StrongBlizzard => strongBlizzardFogDensity,
            _ => defaultFogDensity
        };

        StartFogTransition(targetDensity);
    }

    private void UpdateLighting(EnvironmentState state)
    {
        if (lightCoroutine != null)
            StopCoroutine(lightCoroutine);

        switch (state)
        {
            case EnvironmentState.Cave:
                lightCoroutine = StartCoroutine(
                    LightTransition(
                        caveSunIntensity,
                        caveAmbientIntensity,
                        caveReflectionIntensity,
                        caveAmbientColor,
                        AmbientMode.Flat
                    )
                );
                break;

            case EnvironmentState.Castle:
                lightCoroutine = StartCoroutine(
                    LightTransition(
                        castleSunIntensity,
                        castleAmbientIntensity,
                        castleReflectionIntensity,
                        castleAmbientColor,
                        AmbientMode.Flat
                    )
                );
                break;

            case EnvironmentState.Normal:
            case EnvironmentState.StrongBlizzard:
                lightCoroutine = StartCoroutine(
                    LightTransition(
                        normalSunIntensity,
                        normalAmbientIntensity,
                        normalReflectionIntensity,
                        normalAmbientColor,
                        AmbientMode.Skybox
                    )
                );
                break;
        }
    }

    private void StartFogTransition(float targetDensity)
    {
        if (fogCoroutine != null)
            StopCoroutine(fogCoroutine);

        fogCoroutine =
            StartCoroutine(FogDensityTransition(targetDensity));
    }

    private IEnumerator FogDensityTransition(float targetDensity)
    {
        float startDensity = RenderSettings.fogDensity;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fogLerpSpeed;

            RenderSettings.fogDensity =
                Mathf.Lerp(
                    startDensity,
                    targetDensity,
                    Mathf.Clamp01(t)
                );

            yield return null;
        }

        RenderSettings.fogDensity = targetDensity;
        fogCoroutine = null;
    }

    private IEnumerator LightTransition(
        float targetSunIntensity,
        float targetAmbientIntensity,
        float targetReflectionIntensity,
        Color targetAmbientColor,
        AmbientMode targetAmbientMode)
    {
        float startSunIntensity =
            sunLight != null ? sunLight.intensity : 0f;

        float startAmbientIntensity =
            RenderSettings.ambientIntensity;

        float startReflectionIntensity =
            RenderSettings.reflectionIntensity;

        Color startAmbientColor =
            RenderSettings.ambientLight;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lightLerpSpeed;
            float progress = Mathf.Clamp01(t);

            if (sunLight != null)
            {
                sunLight.intensity =
                    Mathf.Lerp(
                        startSunIntensity,
                        targetSunIntensity,
                        progress
                    );
            }

            RenderSettings.ambientIntensity =
                Mathf.Lerp(
                    startAmbientIntensity,
                    targetAmbientIntensity,
                    progress
                );

            RenderSettings.reflectionIntensity =
                Mathf.Lerp(
                    startReflectionIntensity,
                    targetReflectionIntensity,
                    progress
                );

            RenderSettings.ambientLight =
                Color.Lerp(
                    startAmbientColor,
                    targetAmbientColor,
                    progress
                );

            yield return null;
        }

        if (sunLight != null)
            sunLight.intensity = targetSunIntensity;

        RenderSettings.ambientIntensity =
            targetAmbientIntensity;

        RenderSettings.reflectionIntensity =
            targetReflectionIntensity;

        RenderSettings.ambientLight =
            targetAmbientColor;

        RenderSettings.ambientMode =
            targetAmbientMode;

        lightCoroutine = null;
    }

    private static void PlayParticle(ParticleSystem particle)
    {
        if (particle == null)
            return;

        if (!particle.gameObject.activeSelf)
            particle.gameObject.SetActive(true);

        particle.Clear();
        particle.Play();
    }

    private static void StopParticle(ParticleSystem particle)
    {
        if (particle == null)
            return;

        particle.Stop(
            true,
            ParticleSystemStopBehavior.StopEmitting
        );
    }
}
