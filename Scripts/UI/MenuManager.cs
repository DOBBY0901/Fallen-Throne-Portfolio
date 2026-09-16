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

    public bool IsOpen =>
        menuRoot != null && menuRoot.activeSelf;

    public MenuTab CurrentTab { get; private set; }

    private void Awake()
    {
        if (menuRoot != null)
            menuRoot.SetActive(false);

        playerInput = FindFirstObjectByType<PlayerInput>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleInventory()
    {
        ToggleTab(MenuTab.Inventory);
    }

    public void ToggleEquipment()
    {
        ToggleTab(MenuTab.Equipment);
    }

    public void ToggleSettings()
    {
        ToggleTab(MenuTab.Settings);
    }

    public void EscAction()
    {
        if (IsOpen)
            CloseMenu();
        else
            OpenMenu(MenuTab.Settings);
    }

    public void Resume()
    {
        CloseMenu();
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
        if (menuRoot == null)
            return;

        menuRoot.SetActive(true);

        if (minimap != null)
            minimap.SetActive(false);

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
        if (menuRoot == null)
            return;

        menuRoot.SetActive(false);

        if (minimap != null)
            minimap.SetActive(true);

        if (bossHpUI != null && bossHpUI.IsBossFightActive)
            bossHpUI.Show();

        interactionUIManager?.ShowAll();

        SetPlayerControl(true);
        SetCursorForMenu(false);

        if (pauseTime)
            Time.timeScale = 1f;
    }

    private void SwitchTab(MenuTab tab)
    {
        CurrentTab = tab;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(tab == MenuTab.Inventory);

        if (equipmentPanel != null)
            equipmentPanel.SetActive(tab == MenuTab.Equipment);

        if (settingsPanel != null)
            settingsPanel.SetActive(tab == MenuTab.Settings);
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
