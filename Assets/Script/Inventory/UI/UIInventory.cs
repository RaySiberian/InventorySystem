using System;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private ItemObjectsDatabase database;
    [SerializeField] private GameObject inventoryCellPrefab;
    [SerializeField] private Container playerContainer;
    [SerializeField] private Sprite empty;
    public int InventoryCellsCount;
    public ItemSlot[] Slots;
    
    private void Start()
    {
        InventoryCellsCount = playerContainer.Inventory.Length;
        Slots = new ItemSlot[InventoryCellsCount];
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
        foreach (var slot in Slots)
        {
            slot.ItemNeedSwap -= SwapItemOnInterface;
            slot.ItemRemoved -= RemoveItemInContainer;
        }

        playerContainer.ContainerUpdated -= UpdateCellsData;
    }

    private void CreateInventoryCells()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            GameObject cell = Instantiate(inventoryCellPrefab, this.transform);
            cell.GetComponent<ItemSlot>().Type = SlotType.All;
            Slots[i] = cell.GetComponent<ItemSlot>();
            Slots[i].ItemNeedSwap += SwapItemOnInterface;
            Slots[i].ItemRemoved += RemoveItemInContainer;
        }
    }

    private void RemoveItemInContainer(ItemSlot itemSlot)
    {
        playerContainer.RemoveItem(itemSlot.Item);
    }
    
    private void SwapItemOnInterface(ItemSlot itemSlot1, ItemSlot itemSlot2)
    {
        playerContainer.SwapItems(itemSlot1.Item,itemSlot2.Item);
        UpdateCellsData();
    }
    
    private void UpdateCellsData()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].Item = playerContainer.Inventory[i];
            SetAmount(Slots[i],playerContainer.Inventory[i]);
            SetSpriteByDatabase(Slots[i]);
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
