using System.Collections;
using StarterAssets;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private float knockbackDuration = 0.25f;

    private Coroutine knockbackCoroutine;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (thirdPersonController == null)
            thirdPersonController = GetComponent<ThirdPersonController>();
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        if (controller == null)
            return;

        if (thirdPersonController != null && thirdPersonController._isRolling)
            return;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            KnockbackRoutine(direction.normalized, power)
        );
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float power)
    {
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;

            controller.Move(
                direction * power * Time.deltaTime
            );

            yield return null;
        }

        knockbackCoroutine = null;
    }

    private void OnDisable()
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }
    }
}
