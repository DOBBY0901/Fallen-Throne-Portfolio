using System;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPopupSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private string itemId;
    private Action<string> onClick;

    public void Bind(Action<string> clickCallback)
    {
        onClick = clickCallback;

        if (button == null)
            return;

        button.onClick.RemoveListener(HandleClick);
        button.onClick.AddListener(HandleClick);
    }

    public void Set(ItemDataSO data)
    {
        itemId = data != null ? data.Id : null;

        if (iconImage != null)
        {
            iconImage.sprite =
                data != null ? data.Icon : null;

            iconImage.enabled =
                iconImage.sprite != null;
        }

        if (button != null)
            button.interactable = !string.IsNullOrEmpty(itemId);
    }

    public void SetEmpty()
    {
        itemId = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (button != null)
            button.interactable = false;
    }

    private void HandleClick()
    {
        if (!string.IsNullOrEmpty(itemId))
            onClick?.Invoke(itemId);
    }
}
