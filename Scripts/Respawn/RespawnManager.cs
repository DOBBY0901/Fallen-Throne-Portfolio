using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Header("Default Respawn")]
    [SerializeField] private Transform defaultRespawnPoint;
    [SerializeField] private EnvironmentController.EnvironmentState
        defaultEnvironmentState =
            EnvironmentController.EnvironmentState.Normal;

    [Header("Environment")]
    [SerializeField] private EnvironmentController environmentController;

    private Transform currentRespawnPoint;
    private RespawnStatueInteractable currentStatue;

    private EnvironmentController.EnvironmentState
        currentEnvironmentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentRespawnPoint = defaultRespawnPoint;
        currentEnvironmentState = defaultEnvironmentState;

        if (environmentController == null)
        {
            environmentController =
                FindFirstObjectByType<EnvironmentController>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetRespawnPoint(
        Transform point,
        RespawnStatueInteractable statue)
    {
        if (point == null)
            return;

        if (currentStatue != null &&
            currentStatue != statue)
        {
            currentStatue.SetAura(false);
        }

        currentRespawnPoint = point;
        currentStatue = statue;

        if (currentStatue == null)
            return;

        currentStatue.SetAura(true);
        currentEnvironmentState =
            currentStatue.EnvironmentState;
    }

    public Vector3 GetRespawnPosition()
    {
        return currentRespawnPoint != null
            ? currentRespawnPoint.position
            : Vector3.zero;
    }

    public Quaternion GetRespawnRotation()
    {
        return currentRespawnPoint != null
            ? currentRespawnPoint.rotation
            : Quaternion.identity;
    }

    public EnvironmentController.EnvironmentState
        GetRespawnEnvironmentState()
    {
        return currentEnvironmentState;
    }

    public void ApplyRespawnEnvironment()
    {
        environmentController?.ForceApplyState(
            currentEnvironmentState
        );
    }
}
