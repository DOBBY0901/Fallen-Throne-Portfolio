using System.Collections;
using UnityEngine;

public class BossRockFallPattern : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossPhaseController phaseController;

    [Header("Area")]
    [SerializeField] private BoxCollider spawnArea;

    [Header("Rock Prefabs")]
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private GameObject[] flameRockPrefabs;
    [SerializeField] private GameObject warningCirclePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float warningTime = 1.2f;
    [SerializeField] private float interval = 0.1f;
    [SerializeField] private float rockLifeTime = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float warningYOffset = 0.05f;

    public void StartRockFallPattern(float duration)
    {
        if (duration <= 0f)
            return;

        StartCoroutine(RockFallRoutine(duration));
    }

    private IEnumerator RockFallRoutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 targetPosition = GetRandomPositionInSpawnArea();
            StartCoroutine(SpawnRockWithWarning(targetPosition));

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    private IEnumerator SpawnRockWithWarning(Vector3 targetPosition)
    {
        GameObject warning = CreateWarning(targetPosition);

        yield return new WaitForSeconds(warningTime);

        GameObject selectedRockPrefab = GetRandomRockPrefab();

        if (selectedRockPrefab == null)
        {
            if (warning != null)
                Destroy(warning);

            yield break;
        }

        Vector3 spawnPosition =
            targetPosition + Vector3.up * spawnHeight;

        GameObject rock = Instantiate(
            selectedRockPrefab,
            spawnPosition,
            Quaternion.identity
        );

        BossRock bossRock = rock.GetComponent<BossRock>();
        bossRock?.SetWarningObject(warning);

        Rigidbody rigidbody = rock.GetComponent<Rigidbody>();

        if (rigidbody == null)
            rigidbody = rock.AddComponent<Rigidbody>();

        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        // 충돌하지 못한 경우에도 낙석과 경고 오브젝트가 남지 않도록 정리한다.
        Destroy(rock, rockLifeTime);

        if (warning != null && bossRock == null)
            Destroy(warning, rockLifeTime);
    }

    private GameObject CreateWarning(Vector3 targetPosition)
    {
        if (warningCirclePrefab == null)
            return null;

        return Instantiate(
            warningCirclePrefab,
            targetPosition,
            Quaternion.Euler(90f, 0f, 0f)
        );
    }

    private GameObject GetRandomRockPrefab()
    {
        GameObject[] currentPrefabs = GetCurrentRockPrefabs();

        if (currentPrefabs == null || currentPrefabs.Length == 0)
            return null;

        return currentPrefabs[
            Random.Range(0, currentPrefabs.Length)
        ];
    }

    private GameObject[] GetCurrentRockPrefabs()
    {
        bool isFlamePhase =
            phaseController != null && phaseController.IsFlamePhase;

        if (isFlamePhase &&
            flameRockPrefabs != null &&
            flameRockPrefabs.Length > 0)
        {
            return flameRockPrefabs;
        }

        return rockPrefabs;
    }

    private Vector3 GetRandomPositionInSpawnArea()
    {
        if (spawnArea == null)
            return transform.position;

        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 rayStart =
            new Vector3(x, bounds.max.y + 10f, z);

        if (Physics.Raycast(
                rayStart,
                Vector3.down,
                out RaycastHit hit,
                100f,
                groundLayer,
                QueryTriggerInteraction.Ignore))
        {
            return hit.point + Vector3.up * warningYOffset;
        }

        return new Vector3(
            x,
            bounds.min.y + warningYOffset,
            z
        );
    }
}
