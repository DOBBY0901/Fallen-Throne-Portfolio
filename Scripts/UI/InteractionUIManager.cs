using TMPro;
using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MenuManager menuManager;

    [Header("World Key HUD")]
    [SerializeField] private RectTransform worldKeyHud;

    [Header("Screen Prompt HUD")]
    [SerializeField] private GameObject screenPromptRoot;
    [SerializeField] private TMP_Text screenPromptText;

    private Camera mainCamera;

    private IInteractable worldOwner;
    private Transform worldTarget;

    private IInteractable screenOwner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;

        if (menuManager == null)
            menuManager = FindFirstObjectByType<MenuManager>();

        SetWorldKeyVisible(false);
        SetScreenPromptVisible(false);
    }

    private void LateUpdate()
    {
        if (menuManager != null && menuManager.IsOpen)
        {
            HideAll();
            return;
        }

        UpdateWorldKeyHud();
    }

    public void ShowWorldKey(
        IInteractable owner,
        Transform target)
    {
        if (owner == null || target == null)
            return;

        if (menuManager != null && menuManager.IsOpen)
            return;

        worldOwner = owner;
        worldTarget = target;

        SetWorldKeyVisible(true);
    }

    public void HideWorldKey(IInteractable owner)
    {
        if (worldOwner != owner)
            return;

        worldOwner = null;
        worldTarget = null;

        SetWorldKeyVisible(false);
    }

    public void ShowScreenPrompt(
        IInteractable owner,
        string text)
    {
        if (owner == null)
            return;

        if (menuManager != null && menuManager.IsOpen)
            return;

        screenOwner = owner;

        if (screenPromptText != null)
            screenPromptText.text = text;

        SetScreenPromptVisible(true);
    }

    public void HideScreenPrompt(IInteractable owner)
    {
        if (screenOwner != owner)
            return;

        screenOwner = null;
        SetScreenPromptVisible(false);
    }

    public void HideAll()
    {
        SetWorldKeyVisible(false);
        SetScreenPromptVisible(false);
    }

    public void ShowAll()
    {
        if (worldOwner != null && worldTarget != null)
            SetWorldKeyVisible(true);

        if (screenOwner != null)
            SetScreenPromptVisible(true);
    }

    private void UpdateWorldKeyHud()
    {
        if (worldOwner == null ||
            worldTarget == null ||
            worldKeyHud == null ||
            mainCamera == null)
        {
            return;
        }

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(worldTarget.position);

        bool isInFrontOfCamera = screenPosition.z > 0f;

        SetWorldKeyVisible(isInFrontOfCamera);

        if (isInFrontOfCamera)
            worldKeyHud.position = screenPosition;
    }

    private void SetWorldKeyVisible(bool visible)
    {
        if (worldKeyHud != null)
            worldKeyHud.gameObject.SetActive(visible);
    }

    private void SetScreenPromptVisible(bool visible)
    {
        if (screenPromptRoot != null)
            screenPromptRoot.SetActive(visible);
    }
}
