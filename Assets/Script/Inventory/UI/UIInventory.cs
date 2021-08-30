using System;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private ItemObjectsDatabase database;
    [SerializeField] private GameObject inventoryCellPrefab;
    [SerializeField] private Container playerContainer;
    [SerializeField] private Sprite empty;
    public int InventoryCellsCount;
    public ItemSlot[] InventorySlots;
    public ItemSlot[] EquipmentSlots;
    private void Start()
    {
        InventoryCellsCount = playerContainer.Inventory.Length;
        InventorySlots = new ItemSlot[InventoryCellsCount];
        CreateInventoryCells();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdateCellsData();
        }
    }

    private void OnEnable()
    {
        playerContainer.ContainerUpdated += UpdateCellsData;
    }

    private void OnDisable()
    {
        foreach (var slot in InventorySlots)
        {
            slot.ItemNeedSwap -= SwapItemOnInterface;
            slot.ItemRemoved -= RemoveItemInContainer;
            slot.ItemSwapInEquipment -= SetItemsToEquipment;
        }

        foreach (var slot in EquipmentSlots)
        {
            slot.ItemSwapInEquipment -= SetItemsToEquipment;
            slot.ItemRemoved -= RemoveItemInContainer;
        }
        
        playerContainer.ContainerUpdated -= UpdateCellsData;
    }

    private void CreateInventoryCells()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            GameObject cell = Instantiate(inventoryCellPrefab, this.transform);
            cell.GetComponent<ItemSlot>().Type = EquipmentType.All;
            InventorySlots[i] = cell.GetComponent<ItemSlot>();
            InventorySlots[i].ItemNeedSwap += SwapItemOnInterface;
            InventorySlots[i].ItemRemoved += RemoveItemInContainer;
            InventorySlots[i].ItemSwapInEquipment += SetItemsToEquipment;
        }

        foreach (var slot in EquipmentSlots)
        {
            slot.ItemSwapInEquipment += SetItemsToEquipment;
            slot.ItemRemoved += RemoveItemInContainer;
        }
    }

    private void RemoveItemInContainer(ItemSlot itemSlot)
    {
        if (itemSlot.Type == EquipmentType.All)
        {
            playerContainer.RemoveItemFromInventory(itemSlot.Item); 
        }
        else
        {
            playerContainer.RemoveEquipment(itemSlot.Item);
        }
        
    }

    private void SwapItemOnInterface(ItemSlot fromSlot, ItemSlot toSlot)
    {
        playerContainer.SwapItems(fromSlot.Item,toSlot.Item);
        UpdateCellsData();
    }

    private void SetItemsToEquipment(ItemSlot fromSlot, ItemSlot toSlot)
    {
        if (toSlot.Type != EquipmentType.All)
        {
            ItemObject itemObject = null;
            EquipmentObject equip = null;
            try
            {
                itemObject = database.GetItemByID[fromSlot.Item.ID];
                equip = (EquipmentObject)itemObject;
                if (equip.EquipmentType == toSlot.Type)
                {
                    playerContainer.SetEquipmentFromInventory(fromSlot.Item,toSlot.Item);
                }
            }
            catch
            {
                Debug.Log("Ошибка приведения типов");
            }
            
           
            
            
            //Debug.Log($"fromSlot {fromSlot.Type}, toSlot{toSlot.Type}");
        }
    }
    
    private void UpdateCellsData()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlots[i].Item = playerContainer.Inventory[i];
            SetAmount(InventorySlots[i],playerContainer.Inventory[i]);
            SetSpriteByDatabase(InventorySlots[i]);
        }

        for (int i = 0; i < EquipmentSlots.Length; i++)
        {
            EquipmentSlots[i].Item = playerContainer.Equipment[i];
            SetAmount(EquipmentSlots[i],playerContainer.Equipment[i]);
            SetSpriteByDatabase(EquipmentSlots[i]);
        }
    }

    private void SetSpriteByDatabase(ItemSlot itemSlot)
    {
        for (int i = 0; i < database.ItemObjects.Length; i++)
        {
            if (database.ItemObjects[i].Data.ID == itemSlot.Item.ID)
            {
                itemSlot.InventoryCellIcon.sprite = database.ItemObjects[i].UISprite;
                return;
            }
        }

        itemSlot.InventoryCellIcon.sprite = empty;

    }

    private void SetAmount(ItemSlot itemSlot, Item item)
    {
        itemSlot.InventoryCellText.text = item.Amount > 1 ? item.Amount.ToString() : string.Empty;
    }
    
}
