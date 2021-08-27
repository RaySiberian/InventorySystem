using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Sprite sprite;
    private GameObject tempSprite;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        sprite = GetComponent<Image>().sprite;
    }
    
    //Нажатие
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     GameObject temp = new GameObject();
    //     var rt = temp.AddComponent<RectTransform>();
    //     rt.sizeDelta = new Vector2(50, 50);
    //     temp.transform.SetParent(transform.parent);
    //
    //     var image = temp.AddComponent<Image>();
    //     image.sprite = sprite;
    //     image.raycastTarget = false;
    //     tempSprite = temp;
    //     tempSprite.GetComponent<RectTransform>().position = Input.mousePosition;
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseData.Icon = new GameObject();
        var rt = MouseData.Icon.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        MouseData.Icon.transform.SetParent(transform.parent);
        
        var image = MouseData.Icon.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    
    //Начало движения
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }
    
    //Отпуск кнопки
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Destroy(MouseData.Icon);
        MouseData.Icon = null;
    }
    
    // Процесс движения
    public void OnDrag(PointerEventData eventData)
    {
        MouseData.Icon.GetComponent<RectTransform>().position = Input.mousePosition; 
        //rectTransform.anchoredPosition += eventData.delta;
    }
    
    //Дроп
    public void OnDrop(PointerEventData eventData)
    {
        
        Debug.Log(eventData.pointerDrag.name);
    }
}
