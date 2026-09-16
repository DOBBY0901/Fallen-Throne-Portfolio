using INab.VFXAssets;
using UnityEngine;

public class RespawnStatueInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform hudPosition;
    [SerializeField] private CharacterEffect auraEffect;
    [SerializeField] private Transform respawnPoint;

    [Header("Respawn Environment")]
    [SerializeField] private
        EnvironmentController.EnvironmentState environmentState;

    [Header("Feedback")]
    [SerializeField] private AudioClip activateSfx;

    [Range(0f, 1f)]
    [SerializeField] private float activateVolume = 1f;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 3f;

    public EnvironmentController.EnvironmentState EnvironmentState =>
        environmentState;

    private PlayerInteraction playerInteraction;

    private void Start()
    {
        ResolvePlayer();

        if (respawnPoint == null)
            respawnPoint = transform;

        if (hudPosition == null)
            hudPosition = transform;

        SetAura(false);
    }

    private void Update()
    {
        UpdateInteractionPrompt();
    }

    private void OnDisable()
    {
        ClearInteractionPrompt();
    }

    public void Interact()
    {
        ClearInteractionPrompt();

        RespawnManager.Instance?.SetRespawnPoint(
            respawnPoint,
            this
        );

        if (activateSfx != null)
        {
            AudioManager.Instance?.Play3DSfx(
                activateSfx,
                transform.position,
                activateVolume
            );
        }
    }

    public void SetAura(bool active)
    {
        if (auraEffect == null)
            return;

        if (active)
            auraEffect.PlayEffect_CharacterEffect();
        else
            auraEffect.StopEffect_CharacterEffect();
    }

    private void UpdateInteractionPrompt()
    {
        if (player == null || hudPosition == null)
            return;

        Vector3 offset =
            player.position - transform.position;

        bool isInRange =
            offset.sqrMagnitude <=
            interactDistance * interactDistance;

        if (isInRange)
        {
            InteractionUIManager.Instance?.ShowWorldKey(
                this,
                hudPosition
            );

            playerInteraction?.SetInteractable(this);
        }
        else
        {
            ClearInteractionPrompt();
        }
    }

    private void ClearInteractionPrompt()
    {
        InteractionUIManager.Instance?.HideWorldKey(this);
        playerInteraction?.ClearInteractable(this);
    }

    private void ResolvePlayer()
    {
        if (player == null)
        {
            player =
                GameObject
                    .FindGameObjectWithTag("Player")
                    ?.transform;
        }

        if (player != null)
        {
            playerInteraction =
                player.GetComponent<PlayerInteraction>();
        }
    }
}
