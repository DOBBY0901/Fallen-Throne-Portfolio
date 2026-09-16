using System.Collections;
using System.Collections.Generic;
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

    private struct ActiveZone
    {
        public EnvironmentZone zone;
        public EnvironmentState state;

        public ActiveZone(
            EnvironmentZone zone,
            EnvironmentState state)
        {
            this.zone = zone;
            this.state = state;
        }
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

    private readonly List<ActiveZone> activeZones =
        new List<ActiveZone>();

    private Coroutine fogCoroutine;
    private Coroutine lightCoroutine;

    private float defaultFogDensity;
    private bool initialized;

    private void Awake()
    {
        defaultFogDensity = RenderSettings.fogDensity;
        initialized = true;
    }

    private void OnEnable()
    {
        if (initialized)
            ApplyState(currentState);
    }

    private void OnDisable()
    {
        if (fogCoroutine != null)
        {
            StopCoroutine(fogCoroutine);
            fogCoroutine = null;
        }

        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }
    }

    public void SetEnvironmentState(EnvironmentState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        if (isActiveAndEnabled)
            ApplyState(currentState);
    }

    public void EnterZone(
        EnvironmentZone zone,
        EnvironmentState state)
    {
        if (zone == null)
            return;

        RemoveActiveZone(zone);
        activeZones.Add(new ActiveZone(zone, state));

        SetEnvironmentState(state);
    }

    public void ExitZone(EnvironmentZone zone)
    {
        if (zone == null)
            return;

        RemoveActiveZone(zone);

        EnvironmentState nextState =
            activeZones.Count > 0
                ? activeZones[activeZones.Count - 1].state
                : EnvironmentState.Normal;

        SetEnvironmentState(nextState);
    }

    public void ForceApplyState(EnvironmentState state)
    {
        activeZones.Clear();

        currentState = state;

        if (isActiveAndEnabled)
            ApplyState(currentState);
    }

    private void RemoveActiveZone(EnvironmentZone zone)
    {
        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            if (activeZones[i].zone == zone)
                activeZones.RemoveAt(i);
        }
    }

    private void ApplyState(EnvironmentState state)
    {
        if (!isActiveAndEnabled)
            return;

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
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }

        switch (state)
        {
            case EnvironmentState.Cave:
                StartLightTransition(
                    caveSunIntensity,
                    caveAmbientIntensity,
                    caveReflectionIntensity,
                    caveAmbientColor,
                    AmbientMode.Flat
                );
                break;

            case EnvironmentState.Castle:
                StartLightTransition(
                    castleSunIntensity,
                    castleAmbientIntensity,
                    castleReflectionIntensity,
                    castleAmbientColor,
                    AmbientMode.Flat
                );
                break;

            case EnvironmentState.Normal:
            case EnvironmentState.StrongBlizzard:
                StartLightTransition(
                    normalSunIntensity,
                    normalAmbientIntensity,
                    normalReflectionIntensity,
                    normalAmbientColor,
                    AmbientMode.Skybox
                );
                break;
        }
    }

    private void StartFogTransition(float targetDensity)
    {
        if (fogCoroutine != null)
        {
            StopCoroutine(fogCoroutine);
            fogCoroutine = null;
        }

        if (!isActiveAndEnabled || fogLerpSpeed <= 0f)
        {
            RenderSettings.fogDensity = targetDensity;
            return;
        }

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

    private void StartLightTransition(
        float targetSunIntensity,
        float targetAmbientIntensity,
        float targetReflectionIntensity,
        Color targetAmbientColor,
        AmbientMode targetAmbientMode)
    {
        if (!isActiveAndEnabled || lightLerpSpeed <= 0f)
        {
            ApplyLightingImmediately(
                targetSunIntensity,
                targetAmbientIntensity,
                targetReflectionIntensity,
                targetAmbientColor,
                targetAmbientMode
            );

            return;
        }

        lightCoroutine =
            StartCoroutine(
                LightTransition(
                    targetSunIntensity,
                    targetAmbientIntensity,
                    targetReflectionIntensity,
                    targetAmbientColor,
                    targetAmbientMode
                )
            );
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

        ApplyLightingImmediately(
            targetSunIntensity,
            targetAmbientIntensity,
            targetReflectionIntensity,
            targetAmbientColor,
            targetAmbientMode
        );

        lightCoroutine = null;
    }

    private void ApplyLightingImmediately(
        float sunIntensity,
        float ambientIntensity,
        float reflectionIntensity,
        Color ambientColor,
        AmbientMode ambientMode)
    {
        if (sunLight != null)
            sunLight.intensity = sunIntensity;

        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.reflectionIntensity = reflectionIntensity;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientMode = ambientMode;
    }

    private static void PlayParticle(ParticleSystem particle)
    {
        if (particle == null || !particle.gameObject.activeInHierarchy)
            return;

        particle.Clear();
        particle.Play();
    }

    private static void StopParticle(ParticleSystem particle)
    {
        if (particle == null || !particle.gameObject.activeInHierarchy)
            return;

        particle.Stop(
            true,
            ParticleSystemStopBehavior.StopEmitting
        );
    }
}
