using UnityEngine;

public class EnvironmentZone : MonoBehaviour
{
    [SerializeField] private EnvironmentController environment;
    [SerializeField] private EnvironmentController.EnvironmentState zoneState;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || environment == null)
            return;

        environment.SetEnvironmentState(zoneState);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || environment == null)
            return;

        if (zoneState == EnvironmentController.EnvironmentState.Normal)
            return;

        environment.SetEnvironmentState(
            EnvironmentController.EnvironmentState.Normal
        );
    }
}
