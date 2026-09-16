using System.Collections;
using StarterAssets;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [Min(0f)]
    [SerializeField] private float interactAnimationDuration = 2f;

    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator animator;

    private IInteractable currentInteractable;
    private Coroutine interactionCoroutine;

    public bool IsInteracting => interactionCoroutine != null;

    private static readonly int InteractHash =
        Animator.StringToHash("Interact");

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (input == null || IsInteracting)
            return;

        if (!input.interaction)
            return;

        input.interaction = false;

        if (currentInteractable == null)
            return;

        IInteractable target = currentInteractable;

        interactionCoroutine =
            StartCoroutine(InteractionRoutine(target));
    }

    public void SetInteractable(IInteractable interactable)
    {
        if (interactable != null)
            currentInteractable = interactable;
    }

    public void ClearInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
            currentInteractable = null;
    }

    private IEnumerator InteractionRoutine(
        IInteractable target)
    {
        SetPlayerMovementEnabled(false);

        if (animator != null)
            animator.SetTrigger(InteractHash);

        if (interactAnimationDuration > 0f)
        {
            yield return new WaitForSeconds(
                interactAnimationDuration
            );
        }

        target?.Interact();

        SetPlayerMovementEnabled(true);
        interactionCoroutine = null;
    }

    private void OnDisable()
    {
        if (interactionCoroutine != null)
        {
            StopCoroutine(interactionCoroutine);
            interactionCoroutine = null;
        }

        SetPlayerMovementEnabled(true);
    }

    private void SetPlayerMovementEnabled(bool enabled)
    {
        if (controller != null)
            controller.enabled = enabled;
    }
}
