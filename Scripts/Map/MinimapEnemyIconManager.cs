using System.Collections.Generic;
using UnityEngine;

public class MinimapEnemyIconManager : MonoBehaviour
{
    public static MinimapEnemyIconManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RectTransform iconRoot;

    [Header("Enemy Icons")]
    [SerializeField] private GameObject enemyIconPrefab;
    [SerializeField] private List<Transform> initialEnemies =
        new List<Transform>();

    [Header("Minimap")]
    [SerializeField] private float mapRadius = 90f;

    private readonly Dictionary<Transform, RectTransform> enemyIcons =
        new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < initialEnemies.Count; i++)
            RegisterEnemy(initialEnemies[i]);
    }

    private void LateUpdate()
    {
        if (player == null ||
            minimapCamera == null ||
            enemyIcons.Count == 0)
        {
            return;
        }

        UpdateEnemyIcons();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void RegisterEnemy(Transform enemy)
    {
        if (enemy == null ||
            enemyIcons.ContainsKey(enemy) ||
            enemyIconPrefab == null ||
            iconRoot == null)
        {
            return;
        }

        GameObject iconObject =
            Instantiate(enemyIconPrefab, iconRoot);

        RectTransform icon =
            iconObject.GetComponent<RectTransform>();

        if (icon == null)
        {
            Destroy(iconObject);
            return;
        }

        enemyIcons.Add(enemy, icon);
    }

    public void UnregisterEnemy(Transform enemy)
    {
        if (enemy == null)
            return;

        if (!enemyIcons.TryGetValue(enemy, out RectTransform icon))
            return;

        if (icon != null)
            Destroy(icon.gameObject);

        enemyIcons.Remove(enemy);
    }

    private void UpdateEnemyIcons()
    {
        float worldHalfSize = minimapCamera.orthographicSize;

        if (worldHalfSize <= 0f)
            return;

        float worldToUiScale =
            mapRadius / worldHalfSize;

        foreach (KeyValuePair<Transform, RectTransform> pair in enemyIcons)
        {
            Transform enemy = pair.Key;
            RectTransform icon = pair.Value;

            if (icon == null)
                continue;

            if (enemy == null)
            {
                icon.gameObject.SetActive(false);
                continue;
            }

            Vector3 worldOffset =
                enemy.position - player.position;

            Vector2 horizontalOffset =
                new Vector2(worldOffset.x, worldOffset.z);

            bool isInRange =
                horizontalOffset.sqrMagnitude <=
                worldHalfSize * worldHalfSize;

            icon.gameObject.SetActive(isInRange);

            if (!isInRange)
                continue;

            icon.anchoredPosition =
                horizontalOffset * worldToUiScale;
        }
    }
}
