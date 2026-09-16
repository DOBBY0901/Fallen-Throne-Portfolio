using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string hitTriggerName = "Hit";

    [Header("VFX")]
    [SerializeField] private GameObject hitVfxPrefab;
    [SerializeField] private Transform vfxPoint;

    [Header("SFX")]
    [SerializeField] private AudioClip[] hitSfx;
    [SerializeField] private float pitchMin = 0.95f;
    [SerializeField] private float pitchMax = 1.05f;

    [Range(0f, 1f)]
    [SerializeField] private float hitVolume = 0.8f;

    private int hitHash;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (vfxPoint == null)
            vfxPoint = transform;

        hitHash = Animator.StringToHash(hitTriggerName);
    }

    public void PlayHitFeedback(Vector3 attackerPosition)
    {
        if (animator != null && !string.IsNullOrEmpty(hitTriggerName))
            animator.SetTrigger(hitHash);

        if (hitVfxPrefab != null)
        {
            GameObject vfx = Instantiate(
                hitVfxPrefab,
                vfxPoint.position,
                Quaternion.identity
            );

            Destroy(vfx, 2f);
        }

        if (hitSfx != null && hitSfx.Length > 0)
        {
            AudioManager.Instance?.PlayRandom3DSfx(
                hitSfx,
                transform.position,
                hitVolume,
                pitchMin,
                pitchMax
            );
        }
    }
}
