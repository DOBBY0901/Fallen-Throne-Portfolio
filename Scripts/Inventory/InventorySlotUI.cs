using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button button;

    private int index;
    private Action<int> onClick;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(HandleClick);

        SetEmpty();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }

    public void Bind(int slotIndex, Action<int> clickCallback)
    {
        index = slotIndex;
        onClick = clickCallback;
    }

    public void SetEmpty()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (countText != null)
        {
            countText.text = string.Empty;
            countText.gameObject.SetActive(false);
        }
    }

    public void Set(ItemDataSO data, int count)
    {
        if (data == null)
        {
            SetEmpty();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = data.Icon;
            iconImage.gameObject.SetActive(data.Icon != null);
        }

        if (countText != null)
        {
            bool showCount = count > 1;

            countText.gameObject.SetActive(showCount);

            if (showCount)
                countText.text = count.ToString();
        }
    }

    private void HandleClick()
    {
        onClick?.Invoke(index);
    }
}
