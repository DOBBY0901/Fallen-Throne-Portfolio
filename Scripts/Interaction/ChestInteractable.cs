using System.Collections;
using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform hudPosition;
    [SerializeField] private Animation chestAnimation;
    [SerializeField] private Light chestLight;
    [SerializeField] private AudioClip chestOpenSfx;

    [Header("Loot")]
    [SerializeField] private DropTableSO chestDropTable;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;

    [Header("World")]
    [SerializeField] private TrapManager trapManager;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private string openAnimationName = "ChestAnim";

    [Header("Presentation")]
    [SerializeField] private float maxLightIntensity = 5f;
    [SerializeField] private float lightDuration = 3f;
    [SerializeField] private float lightFadeSpeed = 2f;

    private PlayerInteraction playerInteraction;

    private bool isOpened;
    private bool isOpening;
    private bool hasDropped;

    private void Start()
    {
        ResolvePlayer();

        if (chestAnimation == null)
            chestAnimation = GetComponent<Animation>();

        if (hudPosition == null)
            hudPosition = transform;

        if (chestLight != null)
        {
            chestLight.intensity = 0f;
            chestLight.enabled = false;
        }
    }

    private void Update()
    {
        UpdateInteractionPrompt();
    }

    public void Interact()
    {
        if (isOpened || isOpening)
            return;

        ClearInteractionPrompt();
        StartCoroutine(OpenChestRoutine());
    }

    private IEnumerator OpenChestRoutine()
    {
        isOpening = true;

        PlayOpenPresentation();

        float animationLength = GetOpenAnimationLength();

        if (animationLength > 0f)
            yield return new WaitForSeconds(animationLength);

        yield return FadeLight(0f, maxLightIntensity);

        DropLootOnce();
        trapManager?.StopAllTraps();

        if (lightDuration > 0f)
            yield return new WaitForSeconds(lightDuration);

        yield return FadeLight(
            chestLight != null ? chestLight.intensity : 0f,
            0f
        );

        if (chestLight != null)
            chestLight.enabled = false;

        isOpened = true;
        isOpening = false;
    }

    private void PlayOpenPresentation()
    {
        if (chestAnimation != null &&
            !string.IsNullOrEmpty(openAnimationName))
        {
            chestAnimation.Play(openAnimationName);
        }

        if (chestOpenSfx != null)
        {
            AudioManager.Instance?.Play3DSfx(
                chestOpenSfx,
                transform.position
            );
        }
    }

    private float GetOpenAnimationLength()
    {
        if (chestAnimation == null ||
            string.IsNullOrEmpty(openAnimationName))
        {
            return 0f;
        }

        AnimationState state =
            chestAnimation[openAnimationName];

        return state != null ? state.length : 0f;
    }

    private IEnumerator FadeLight(
        float startIntensity,
        float targetIntensity)
    {
        if (chestLight == null)
            yield break;

        chestLight.enabled = true;

        if (lightFadeSpeed <= 0f)
        {
            chestLight.intensity = targetIntensity;
            yield break;
        }

        float duration =
            Mathf.Abs(targetIntensity - startIntensity) /
            lightFadeSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = duration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / duration);

            chestLight.intensity =
                Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        chestLight.intensity = targetIntensity;
    }

    private void DropLootOnce()
    {
        if (hasDropped)
            return;

        if (chestDropTable == null ||
            database == null ||
            inventory == null)
        {
            return;
        }

        hasDropped = true;

        var rolledItems = chestDropTable.Roll();

        foreach (var (data, amount) in rolledItems)
        {
            if (data == null || amount <= 0)
                continue;

            inventory.AddItem(
                database,
                data.Id,
                amount
            );
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (isOpened || isOpening)
        {
            ClearInteractionPrompt();
            return;
        }

        if (player == null || hudPosition == null)
            return;

        float distance =
            Vector3.Distance(
                player.position,
                hudPosition.position
            );

        if (distance <= interactDistance)
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
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerInteraction = player.GetComponent<PlayerInteraction>();
    }
}
