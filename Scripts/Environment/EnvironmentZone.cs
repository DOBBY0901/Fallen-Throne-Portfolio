using UnityEngine;

public class EnvironmentZone : MonoBehaviour
{
    [SerializeField] private EnvironmentController environment;
    [SerializeField] private
        EnvironmentController.EnvironmentState zoneState;

    private int playerColliderCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other) || environment == null)
            return;

        playerColliderCount++;

        if (playerColliderCount == 1)
        {
            environment.EnterZone(
                this,
                zoneState
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other) || environment == null)
            return;

        playerColliderCount =
            Mathf.Max(
                0,
                playerColliderCount - 1
            );

        if (playerColliderCount == 0)
            environment.ExitZone(this);
    }

    private void OnDisable()
    {
        if (playerColliderCount <= 0 ||
            environment == null)
        {
            return;
        }

        playerColliderCount = 0;
        environment.ExitZone(this);
    }

    private static bool IsPlayer(Collider other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        Transform root = other.transform.root;

        return root != null &&
            root.CompareTag("Player");
    }
}
