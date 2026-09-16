using UnityEngine;
using UnityEngine.InputSystem;

public enum MenuTab
{
    Inventory,
    Equipment,
    Map,
    Settings
}

public class MenuManager : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private InteractionUIManager interactionUIManager;

    [Header("Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("HUD")]
    [SerializeField] private GameObject minimap;
    [SerializeField] private BossHpUI bossHpUI;

    [Header("Options")]
    [SerializeField] private bool pauseTime = true;

    private PlayerInput playerInput;
    private bool isDeathState;

    public bool IsOpen =>
        menuRoot != null && menuRoot.activeSelf;

    public bool IsDeathState => isDeathState;

    public MenuTab CurrentTab { get; private set; }

    private void Awake()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        playerInput = FindFirstObjectByType<PlayerInput>();
        SetCursorForMenu(false);
    }

    private void LateUpdate()
    {
        if (!isDeathState)
            return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void ToggleInventory()
    {
        if (!isDeathState)
            ToggleTab(MenuTab.Inventory);
    }

    public void ToggleEquipment()
    {
        if (!isDeathState)
            ToggleTab(MenuTab.Equipment);
    }

    public void ToggleSettings()
    {
        if (!isDeathState)
            ToggleTab(MenuTab.Settings);
    }

    public void EscAction()
    {
        if (isDeathState)
            return;

        if (IsOpen)
            CloseMenu();
        else
            OpenMenu(MenuTab.Settings);
    }

    public void Resume()
    {
        if (!isDeathState)
            CloseMenu();
    }

    public void EnterDeathState()
    {
        isDeathState = true;

        ForceCloseMenuUI();

        minimap?.SetActive(false);
        bossHpUI?.Hide();
        interactionUIManager?.HideAll();

        SetPlayerControl(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    public void ExitDeathState()
    {
        isDeathState = false;

        ForceCloseMenuUI();

        minimap?.SetActive(true);

        if (bossHpUI != null && bossHpUI.IsBossFightActive)
            bossHpUI.Show();

        interactionUIManager?.ShowAll();

        SetPlayerControl(true);
        SetCursorForMenu(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ToggleTab(MenuTab tab)
    {
        if (isDeathState)
            return;

        if (!IsOpen)
        {
            OpenMenu(tab);
            return;
        }

        if (CurrentTab == tab)
        {
            CloseMenu();
            return;
        }

        SwitchTab(tab);
    }

    private void OpenMenu(MenuTab tab)
    {
        if (isDeathState || menuRoot == null)
            return;

        menuRoot.SetActive(true);

        minimap?.SetActive(false);
        bossHpUI?.Hide();
        interactionUIManager?.HideAll();

        SwitchTab(tab);
        SetPlayerControl(false);
        SetCursorForMenu(true);

        if (pauseTime)
            Time.timeScale = 0f;
    }

    private void CloseMenu()
    {
        if (isDeathState || menuRoot == null)
            return;

        ForceCloseMenuUI();

        minimap?.SetActive(true);

        if (bossHpUI != null && bossHpUI.IsBossFightActive)
            bossHpUI.Show();

        interactionUIManager?.ShowAll();

        SetPlayerControl(true);
        SetCursorForMenu(false);

        if (pauseTime)
            Time.timeScale = 1f;
    }

    private void ForceCloseMenuUI()
    {
        menuRoot?.SetActive(false);
        inventoryPanel?.SetActive(false);
        equipmentPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    private void SwitchTab(MenuTab tab)
    {
        CurrentTab = tab;

        inventoryPanel?.SetActive(tab == MenuTab.Inventory);
        equipmentPanel?.SetActive(tab == MenuTab.Equipment);
        settingsPanel?.SetActive(tab == MenuTab.Settings);
    }

    private void SetPlayerControl(bool enabled)
    {
        if (playerInput != null)
            playerInput.enabled = enabled;
    }

    private static void SetCursorForMenu(bool menuMode)
    {
        Cursor.visible = menuMode;
        Cursor.lockState =
            menuMode
                ? CursorLockMode.None
                : CursorLockMode.Locked;
    }
}
