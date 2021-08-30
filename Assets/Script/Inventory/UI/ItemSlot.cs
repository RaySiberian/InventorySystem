using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public Item Item;
    public TextMeshProUGUI InventoryCellText;
    public Image InventoryCellIcon;
    public EquipmentType Type;
    public event Action<ItemSlot, ItemSlot> ItemNeedSwap;
    public event Action<ItemSlot> ItemRemoved;
    
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = FindObjectOfType<Canvas>();
    }
    
    //Нажатие
    public void OnPointerDown(PointerEventData eventData)
    {
        MouseData.Icon = new GameObject();
        
        var rt = MouseData.Icon.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        MouseData.Icon.transform.SetParent(canvas.transform);

        var image = MouseData.Icon.AddComponent<Image>();
        image.sprite = InventoryCellIcon.sprite;
        image.raycastTarget = false;
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition;

        MouseData.FromSlot = this;
    }

    //Начало движения
    public void OnBeginDrag(PointerEventData eventData)
    {
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition;
        canvasGroup.blocksRaycasts = false;
    }
    
    //Отпуск кнопки
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Destroy(MouseData.Icon);
        if (eventData.pointerEnter == null)
        {
            ItemRemoved?.Invoke(MouseData.FromSlot);
            Destroy(MouseData.Icon);
            MouseData.ClearData();
        }
    }
    
    // Процесс движения
    public void OnDrag(PointerEventData eventData)
    {
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    
    //Дроп
    public void OnDrop(PointerEventData eventData)
    {
        MouseData.ToSlot = this;
        ItemNeedSwap?.Invoke(MouseData.FromSlot,MouseData.ToSlot);
        Destroy(MouseData.Icon);
        MouseData.ClearData();
    }
}

public static class MouseData
{
    public static ItemSlot FromSlot;
    public static ItemSlot ToSlot;
    public static GameObject Icon;

    public static void ClearData()
    {
        FromSlot = null;
        ToSlot = null;
        Icon = null;
    }
}
