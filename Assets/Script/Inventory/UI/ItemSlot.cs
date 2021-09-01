using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerUpHandler
{
    public Item Item;
    public TextMeshProUGUI InventoryCellText;
    public Image InventoryCellIcon;
    public EquipmentType Type;
    public event Action<ItemSlot, ItemSlot> ItemNeedSwap;
    public event Action<ItemSlot> ItemRemoved;
    public event Action<ItemSlot, ItemSlot> ItemSwapInEquipment;

    private bool isMoved;
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
        isMoved = false;
        
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
        isMoved = true;
    }
    
    // Процесс движения
    public void OnDrag(PointerEventData eventData)
    {
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    
    //Отжатие кнопки
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isMoved)
        {
            Destroy(MouseData.Icon);
            MouseData.ClearData();
        }
    }
    
    //Дроп(отжатие кнопки, но вызывается раньше OnEndDrag)
    public void OnDrop(PointerEventData eventData)
    {
        MouseData.ToSlot = this;

        if (MouseData.FromSlot.Type != EquipmentType.All || MouseData.ToSlot.Type != EquipmentType.All)
        {
            ItemSwapInEquipment?.Invoke(MouseData.FromSlot,MouseData.ToSlot);
        }
        
        if (MouseData.FromSlot.Type == EquipmentType.All && MouseData.ToSlot.Type == EquipmentType.All)
        {
            ItemNeedSwap?.Invoke(MouseData.FromSlot,MouseData.ToSlot);
        }
        Destroy(MouseData.Icon);
        MouseData.ClearData();
    }
    
    //Отжатие кнопки для UI
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
