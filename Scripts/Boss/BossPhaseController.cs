using System.Collections;
using UnityEngine;

public class BossPhaseController : MonoBehaviour
{
    [Header("Boss References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer[] bossRenderers;

    [Header("Boss Materials")]
    [SerializeField] private Material lavaBossMaterial;

    [Header("Map Sets")]
    [SerializeField] private GameObject normalMapSet;
    [SerializeField] private GameObject lavaMapSet;

    [Header("Audio")]
    [SerializeField] private AudioClip roarSfx;

    [Header("Phase Settings")]
    [SerializeField] private float roarDuration = 2f;

    public bool IsFlamePhase { get; private set; }

    private static readonly int RoarHash = Animator.StringToHash("Roar");

    private void Awake()
    {
        if (normalMapSet != null)
            normalMapSet.SetActive(true);

        if (lavaMapSet != null)
            lavaMapSet.SetActive(false);
    }

    public IEnumerator EnterFlamePhaseRoutine()
    {
        if (IsFlamePhase)
            yield break;

        if (animator != null)
            animator.SetTrigger(RoarHash);

        if (roarSfx != null)
            AudioManager.Instance?.Play3DSfx(roarSfx, transform.position);

        yield return new WaitForSeconds(roarDuration);

        ApplyFlamePhase();
    }

    public void ApplyFlamePhase()
    {
        if (IsFlamePhase)
            return;

        ApplyLavaBossMaterial();
        ChangeMapToLava();

        IsFlamePhase = true;
    }

    private void ApplyLavaBossMaterial()
    {
        if (lavaBossMaterial == null || bossRenderers == null)
            return;

        foreach (Renderer bossRenderer in bossRenderers)
        {
            if (bossRenderer == null)
                continue;

            Material[] materials = bossRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
                materials[i] = lavaBossMaterial;

            bossRenderer.materials = materials;
        }
    }

    private void ChangeMapToLava()
    {
        if (normalMapSet != null)
            normalMapSet.SetActive(false);

        if (lavaMapSet != null)
            lavaMapSet.SetActive(true);
    }
}
