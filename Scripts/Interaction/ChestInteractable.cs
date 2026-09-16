using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Inventory inventory;

    [Header("World")]
    [SerializeField] private TrapManager trapManager;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private string openAnimationName =
        "ChestAnim";

    [Header("Presentation")]
    [SerializeField] private float maxLightIntensity = 5f;
    [SerializeField] private float lightDuration = 3f;
    [SerializeField] private float lightFadeSpeed = 2f;

    private PlayerInteraction playerInteraction;

    private List<(ItemDataSO data, int amount)>
        preparedLoot;

    private bool isOpened;
    private bool isOpening;
    private bool lootRolled;
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

    private void OnDisable()
    {
        ClearInteractionPrompt();
    }

    public void Interact()
    {
        if (isOpened || isOpening)
            return;

        // 보상을 전부 수용할 공간이 있을 때만 상자를 연다.
        // 부족하면 같은 드롭 결과를 유지한 채 다시 시도할 수 있다.
        if (!PrepareLoot())
            return;

        ClearInteractionPrompt();
        StartCoroutine(OpenChestRoutine());
    }

    private IEnumerator OpenChestRoutine()
    {
        isOpening = true;

        PlayOpenPresentation();

        float animationLength =
            GetOpenAnimationLength();

        if (animationLength > 0f)
        {
            yield return new WaitForSeconds(
                animationLength
            );
        }

        yield return FadeLight(
            0f,
            maxLightIntensity
        );

        if (!GrantPreparedLoot())
        {
            isOpening = false;
            yield break;
        }

        trapManager?.StopAllTraps();

        if (lightDuration > 0f)
        {
            yield return new WaitForSeconds(
                lightDuration
            );
        }

        yield return FadeLight(
            chestLight != null
                ? chestLight.intensity
                : 0f,
            0f
        );

        if (chestLight != null)
            chestLight.enabled = false;

        isOpened = true;
        isOpening = false;
    }

    private bool PrepareLoot()
    {
        if (hasDropped)
            return true;

        if (chestDropTable == null ||
            inventory == null)
        {
            return false;
        }

        if (!lootRolled)
        {
            preparedLoot = chestDropTable.Roll();
            lootRolled = true;
        }

        if (preparedLoot == null ||
            preparedLoot.Count == 0)
        {
            return true;
        }

        return inventory.CanAddItems(preparedLoot);
    }

    private bool GrantPreparedLoot()
    {
        if (hasDropped)
            return true;

        if (preparedLoot == null ||
            preparedLoot.Count == 0)
        {
            hasDropped = true;
            return true;
        }

        if (!inventory.AddItems(preparedLoot))
            return false;

        hasDropped = true;
        return true;
    }

    private void PlayOpenPresentation()
    {
        if (chestAnimation != null &&
            !string.IsNullOrEmpty(openAnimationName))
        {
            chestAnimation.Play(
                openAnimationName
            );
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

        return state != null
            ? state.length
            : 0f;
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
            chestLight.intensity =
                targetIntensity;

            yield break;
        }

        float duration =
            Mathf.Abs(
                targetIntensity -
                startIntensity
            ) / lightFadeSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                duration <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        elapsed / duration
                    );

            chestLight.intensity =
                Mathf.Lerp(
                    startIntensity,
                    targetIntensity,
                    t
                );

            yield return null;
        }

        chestLight.intensity =
            targetIntensity;
    }

    private void UpdateInteractionPrompt()
    {
        if (isOpened || isOpening)
        {
            ClearInteractionPrompt();
            return;
        }

        if (player == null ||
            hudPosition == null)
        {
            return;
        }

        Vector3 offset =
            player.position -
            hudPosition.position;

        bool isInRange =
            offset.sqrMagnitude <=
            interactDistance *
            interactDistance;

        if (isInRange)
        {
            InteractionUIManager.Instance
                ?.ShowWorldKey(
                    this,
                    hudPosition
                );

            playerInteraction
                ?.SetInteractable(this);
        }
        else
        {
            ClearInteractionPrompt();
        }
    }

    private void ClearInteractionPrompt()
    {
        InteractionUIManager.Instance
            ?.HideWorldKey(this);

        playerInteraction
            ?.ClearInteractable(this);
    }

    private void ResolvePlayer()
    {
        if (player == null)
        {
            player =
                GameObject
                    .FindGameObjectWithTag(
                        "Player"
                    )
                    ?.transform;
        }

        if (player != null)
        {
            playerInteraction =
                player.GetComponent<
                    PlayerInteraction
                >();
        }
    }
}
