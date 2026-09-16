using UnityEngine;

public class WarningCirclePulse : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 5f;

    private Material materialInstance;
    private Color baseEmissionColor;

    private static readonly int EmissionColorId =
        Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
            return;

        materialInstance = targetRenderer.material;
        baseEmissionColor =
            materialInstance.GetColor(EmissionColorId);
    }

    private void Update()
    {
        if (materialInstance == null)
            return;

        float pulse =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        float intensity =
            Mathf.Lerp(minIntensity, maxIntensity, pulse);

        materialInstance.SetColor(
            EmissionColorId,
            baseEmissionColor * intensity
        );
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}
