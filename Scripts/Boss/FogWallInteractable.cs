using UnityEngine;

public class FogWallInteractable : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject fogWallRoot;
    [SerializeField] private BossAI bossAI;
    [SerializeField] private BossHpUI bossHpUI;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject exitBlockWall;

    [Header("Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private string promptText = "안개 속으로 들어간다";

    private PlayerInteraction playerInteraction;
    private bool isUsed;

    private void Start()
    {
        ResolvePlayerReferences();

        if (menuManager == null)
            menuManager = FindFirstObjectByType<MenuManager>();

        bossHpUI?.Hide();

        if (exitBlockWall != null)
            exitBlockWall.SetActive(false);
    }

    private void Update()
    {
        UpdatePrompt();
    }

    public void Interact()
    {
        if (isUsed)
            return;

        isUsed = true;

        InteractionUIManager.Instance?.HideScreenPrompt(this);
        playerInteraction?.ClearInteractable(this);

        if (fogWallRoot != null)
            fogWallRoot.SetActive(false);

        if (exitBlockWall != null)
            exitBlockWall.SetActive(true);

        bossHpUI?.Show();
        bossAI?.StartBoss();
    }

    private void ResolvePlayerReferences()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerInteraction = player.GetComponent<PlayerInteraction>();
    }

    private void UpdatePrompt()
    {
        if (player == null)
            return;

        if (isUsed)
        {
            ClearPrompt();
            return;
        }

        if (menuManager != null && menuManager.IsOpen)
        {
            ClearPrompt();
            return;
        }

        float distance =
            Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            InteractionUIManager.Instance?.ShowScreenPrompt(this, promptText);
            playerInteraction?.SetInteractable(this);
        }
        else
        {
            ClearPrompt();
        }
    }

    private void ClearPrompt()
    {
        InteractionUIManager.Instance?.HideScreenPrompt(this);
        playerInteraction?.ClearInteractable(this);
    }
}
