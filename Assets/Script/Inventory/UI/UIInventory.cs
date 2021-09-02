using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private Database database;
    [SerializeField] private GameObject inventoryCellPrefab;
    [SerializeField] private Container playerContainer;
    [SerializeField] private Sprite empty;
    public int InventoryCellsCount;
    public ItemSlot[] InventorySlots;
    public ItemSlot[] EquipmentSlots;
    public ItemSlot[] CraftingSlots;
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
            slot.ItemSwapInCraft += SwapItemToCraft;
        }

        foreach (var slot in EquipmentSlots)
        {
            slot.ItemSwapInEquipment -= SetItemsToEquipment;
            slot.ItemRemoved -= RemoveItemInContainer;
        }
        
        foreach (var slot in CraftingSlots)
        {
            slot.ItemSwapInCraft -= SwapItemToCraft;
        }
        
        playerContainer.ContainerUpdated -= UpdateCellsData;
    }
    
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
            InventorySlots[i].ItemSwapInCraft += SwapItemToCraft;
        }

        foreach (var slot in CraftingSlots)
        {
            slot.ItemSwapInCraft += SwapItemToCraft;
        }
        
        foreach (var slot in EquipmentSlots)
        {
            slot.ItemSwapInEquipment += SetItemsToEquipment;
            slot.ItemRemoved += RemoveItemInContainer;
        }
    }

    private void SwapItemToCraft(ItemSlot fromSlot, ItemSlot toSlot, ButtonPressed buttonPressed)
    {
        if (fromSlot.SlotType == SlotType.Craft && toSlot.SlotType == SlotType.Craft && buttonPressed == ButtonPressed.RightMouseButton) 
        {
            playerContainer.SplitOneItem(fromSlot.Item,toSlot.Item, ContainerType.Craft);
            return;
        }
            
        if (fromSlot.SlotType == SlotType.Craft && toSlot.SlotType == SlotType.Craft)
        {
            playerContainer.SwapItems(fromSlot.Item,toSlot.Item,ContainerType.Craft);
            return;
        }

        if (fromSlot.SlotType != SlotType.Equipment && toSlot.SlotType != SlotType.Equipment)
        {
            playerContainer.SetItemsInventoryCraft(fromSlot.Item, toSlot.Item);
            return;
        }
        
        if (buttonPressed == ButtonPressed.RightMouseButton)
        {
            playerContainer.SplitOneItemInventoryCraft(fromSlot.Item,toSlot.Item);
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

    private void SwapItemOnInterface(ItemSlot fromSlot, ItemSlot toSlot,ButtonPressed buttonPressed)
    {
        if (buttonPressed == ButtonPressed.RightMouseButton)
        {
            playerContainer.SplitOneItem(fromSlot.Item,toSlot.Item,ContainerType.Inventory);
            return;
        }
        playerContainer.SwapItems(fromSlot.Item,toSlot.Item,ContainerType.Inventory);
        UpdateCellsData();
    }

    private void SetItemsToEquipment(ItemSlot fromSlot, ItemSlot toSlot)
    {
        if (toSlot.Type != EquipmentType.All)
        {
            try
            {
                //Если передеваемый предмет является equipmentObject
                ItemObject itemObject = database.GetItemByID[fromSlot.Item.ID];
                EquipmentObject equip = (EquipmentObject)itemObject;
                
                if (equip.EquipmentType == toSlot.Type)
                {
                    playerContainer.SwapItemsInInventoryEquipment(fromSlot.Item,toSlot.Item);
                    return;
                }
            }
            catch 
            {
                Debug.Log("Item is not Equipment");
                return;
            } 
        }

        if (toSlot.Type == EquipmentType.All)
        {
            if (Item.IsEmpty(toSlot.Item))
            {
                playerContainer.SwapItemsInInventoryEquipment(fromSlot.Item,toSlot.Item);
                return;
            }

            try
            {
                //Если предмет инвентаря предмет является equipmentObject
                ItemObject itemObject = database.GetItemByID[toSlot.Item.ID];
                EquipmentObject equip = (EquipmentObject)itemObject;
                
                if (equip.EquipmentType == fromSlot.Type || toSlot.Item == new Item())
                {
                    playerContainer.SwapItemsInInventoryEquipment(fromSlot.Item,toSlot.Item);
                }
            }
            catch 
            {
                Debug.Log("Item is not Equipment");
            } 
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

        for (int i = 0; i < CraftingSlots.Length; i++)
        {
            CraftingSlots[i].Item = playerContainer.CraftStorage[i];
            SetAmount(CraftingSlots[i],playerContainer.CraftStorage[i]);
            SetSpriteByDatabase(CraftingSlots[i]);
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
