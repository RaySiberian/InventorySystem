using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Container : MonoBehaviour
{
    private string savePath;
    public Database Database;
    public SerializableContainer InventoryContainer;
    public ItemObject test;
    public ItemObject test1;
    public Item[] Inventory => InventoryContainer.Inventory;
    public Item[] Equipment => InventoryContainer.Equipment;

    public Item[] CraftStorage = new Item[9];
    public event Action ContainerUpdated;

    public int FreeSlots
    {
        get { return Inventory.Count(t => t.ID == -1); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItemInInventory(test);
            AddItemInInventory(test1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetEquipment(test1.Data);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            CraftItem();
        }
    }

    public void SplitOneItem(Item fromSlot, Item toSlot)
    {
        if (fromSlot.Amount == 1)
        {
            SwapItemsInInventory(fromSlot, toSlot);
            return;
        }

        if (fromSlot.ID == toSlot.ID)
        {
            fromSlot.Amount -= 1;
            toSlot.Amount += 1;
            ContainerUpdated?.Invoke();
            return;
        }
        
        Inventory[FindItemArrayPositionInventory(toSlot)] = new Item(FindObjectInDatabase(fromSlot));
        fromSlot.Amount -= 1;
        ContainerUpdated?.Invoke();
    }

    public void AddItemInInventory(ItemObject itemObject)
    {
        if (itemObject.StackAble)
        {
            //Цикл заглушка, чтоб предмет добовлялся по одному
            for (int i = 0; i < itemObject.Data.Amount; i++)
            {
                AddStackableAmount(itemObject);
            }
        }
        else
        {
            AddUnStackableAmount(itemObject);
        }

        ContainerUpdated?.Invoke();
    }

    private void AddStackableAmount(ItemObject itemObject)
    {
        if (FreeSlots != 0 && !IsItemContainsInInventory(itemObject))
        {
            AddNewItem(itemObject);
            return;
        }

        if (FreeSlots == 0)
        {
            if (IsStackInInventory(itemObject))
            {
                Inventory[GetStack(itemObject)].Amount += 1;
            }
            else
            {
                return;
            }
        }

        if (FreeSlots != 0)
        {
            if (IsStackInInventory(itemObject))
            {
                Inventory[GetStack(itemObject)].Amount += 1;
            }
            else
            {
                AddNewItem(itemObject);
            }
        }
    }

    private void AddUnStackableAmount(ItemObject itemObject)
    {
        if (FreeSlots == 0)
        {
            return;
        }

        AddNewItem(itemObject);
    }

    //Проверка есть ли неполный стак данного предмета
    private bool IsStackInInventory(ItemObject itemObject)
    {
        Item temp = FindItemInInventory(itemObject);

        return Inventory.Any(item => item.ID == temp.ID && item.Amount != itemObject.MaxStuckSize);
    }

    //Получение ID неполного стака данного предмета
    private int GetStack(ItemObject itemObject)
    {
        Item temp = FindItemInInventory(itemObject);
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].ID == temp.ID && Inventory[i].Amount != itemObject.MaxStuckSize)
            {
                return i;
            }
        }

        return -1;
    }

    private void AddNewItem(ItemObject itemObject)
    {
        Inventory[GetFreeSlot()] = new Item(itemObject);
        ContainerUpdated?.Invoke();
    }

    private int FindItemArrayPositionInventory(Item itemToFind)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == itemToFind)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindItemArrayPositionCraft(Item itemToFind)
    {
        for (int i = 0; i < CraftStorage.Length; i++)
        {
            if (CraftStorage[i] == itemToFind)
            {
                return i;
            }
        }

        return -1;
    }

    public void RemoveItemFromInventory(Item item)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == item)
            {
                Inventory[i] = new Item();
            }
        }

        ContainerUpdated?.Invoke();
    }

    public void SwapItemsInInventory(Item item1, Item item2)
    {
        if (CheckForStack(item1, item2))
        {
            ContainerUpdated?.Invoke();
            return;
        }

        int pos1 = 0;
        int pos2 = 0;

        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == item1)
            {
                pos1 = i;
            }

            if (Inventory[i] == item2)
            {
                pos2 = i;
            }
        }

        Inventory[pos1] = item2;
        Inventory[pos2] = item1;

        ContainerUpdated?.Invoke();
    }

    private bool CheckForStack(Item fromSlot, Item toSlot)
    {
        //TODO нечеловеческий костыль 
        {
            bool fromSlotStackable = true;
            bool toSlotStackable = true;

            if (fromSlot.ID != -1)
            {
                fromSlotStackable = FindObjectInDatabase(fromSlot).StackAble;
            }

            if (toSlot.ID != -1)
            {
                toSlotStackable = FindObjectInDatabase(fromSlot).StackAble;
            }

            if (!fromSlotStackable || !toSlotStackable)
            {
                return false;
            }
        }

        if (fromSlot.ID != toSlot.ID)
        {
            return false;
        }

        if (toSlot.Amount == FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            return false;
        }

        if (fromSlot.Amount + toSlot.Amount <= FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            toSlot.Amount = toSlot.Amount + fromSlot.Amount;
            Inventory[FindItemArrayPositionInventory(fromSlot)] = new Item();
            return true;
        }

        if (fromSlot.Amount + toSlot.Amount > FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            int tempSum = fromSlot.Amount + toSlot.Amount;
            fromSlot.Amount = tempSum - FindObjectInDatabase(fromSlot).MaxStuckSize;
            toSlot.Amount = FindObjectInDatabase(toSlot).MaxStuckSize;
            return true;
        }

        return false;
    }

    private bool IsItemContainsInInventory(ItemObject itemObject)
    {
        return Inventory.Any(item => item.ID == itemObject.Data.ID);
    }

    private bool IsItemContainsInInventory(Item item)
    {
        return Inventory.Any(t => t == item);
    }

    private Item FindItemInInventory(ItemObject itemObject)
    {
        return Inventory.FirstOrDefault(item => item.ID == itemObject.Data.ID);
    }

    private ItemObject FindObjectInDatabase(Item item)
    {
        return Database.GetItemByID[item.ID];
    }

    private int GetFreeSlot()
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].ID == -1)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetEquipmentSlot(Item item)
    {
        ItemObject itemObject = FindObjectInDatabase(item);
        EquipmentObject equip = (EquipmentObject)itemObject;
        return equip.EquipmentType switch
        {
            EquipmentType.Head => 0,
            EquipmentType.Torso => 1,
            EquipmentType.Lags => 2,
            EquipmentType.Feet => 3,
            EquipmentType.Hands => 4,
            EquipmentType.Fingers => 5,
            EquipmentType.Neck => 6,
            EquipmentType.Weapon => 7,
            EquipmentType.Shield => 8,
            _ => -1
        };
    }

    public void SetEquipment(Item item)
    {
        Equipment[GetEquipmentSlot(item)] = item;
        ContainerUpdated?.Invoke();
    }

    public void RemoveEquipment(Item item)
    {
        Equipment[GetEquipmentSlot(item)] = new Item();
        ContainerUpdated?.Invoke();
    }

    public void SwapItemsInContainers(Item item1, Item item2)
    {
        Item inventoryItem;
        Item equipmentItem;
        Item empty = new Item();
        //Если содержится, значит item1 - объект из инвентаря
        if (IsItemContainsInInventory(item1))
        {
            inventoryItem = item1;
            equipmentItem = item2;
        }
        else
        {
            inventoryItem = item2;
            equipmentItem = item1;
        }

        if (inventoryItem.ID == -1)
        {
            RemoveEquipment(equipmentItem);
            Inventory[FindItemArrayPositionInventory(inventoryItem)] = equipmentItem;
            ContainerUpdated?.Invoke();
            return;
        }

        if (equipmentItem.ID == -1)
        {
            SetEquipment(inventoryItem);
            Inventory[FindItemArrayPositionInventory(inventoryItem)] = new Item();
            ContainerUpdated?.Invoke();
            return;
        }


        SetEquipment(inventoryItem);
        Inventory[FindItemArrayPositionInventory(inventoryItem)] = equipmentItem;

        ContainerUpdated?.Invoke();
    }

    public void SetItemsInventoryCraft(Item fromSlot, Item toSlot)
    {
        if (IsItemContainsInInventory(fromSlot))
        {
            if (CheckForStackInventoryCraft(fromSlot, toSlot, true))
            {
                ContainerUpdated?.Invoke();
                return;
            }

            Inventory[FindItemArrayPositionInventory(fromSlot)] = toSlot;
            CraftStorage[FindItemArrayPositionCraft(toSlot)] = fromSlot;
        }
        else
        {
            if (CheckForStackInventoryCraft(fromSlot, toSlot, false))
            {
                ContainerUpdated?.Invoke();
                return;
            }

            Inventory[FindItemArrayPositionInventory(toSlot)] = fromSlot;
            CraftStorage[FindItemArrayPositionCraft(fromSlot)] = toSlot;
        }

        ContainerUpdated?.Invoke();
    }

    private bool CheckForStackInventoryCraft(Item fromSlot, Item toSlot, bool itemFromInventory)
    {
        //TODO нечеловеческий костыль 
        {
            bool fromSlotStackable = true;
            bool toSlotStackable = true;

            if (fromSlot.ID != -1)
            {
                fromSlotStackable = FindObjectInDatabase(fromSlot).StackAble;
            }

            if (toSlot.ID != -1)
            {
                toSlotStackable = FindObjectInDatabase(fromSlot).StackAble;
            }

            if (!fromSlotStackable || !toSlotStackable)
            {
                return false;
            }
        }

        if (fromSlot.ID != toSlot.ID)
        {
            return false;
        }

        if (toSlot.Amount == FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            return false;
        }

        if (fromSlot.Amount + toSlot.Amount <= FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            toSlot.Amount = toSlot.Amount + fromSlot.Amount;
            if (itemFromInventory)
            {
                Inventory[FindItemArrayPositionInventory(fromSlot)] = new Item();
            }
            else
            {
                CraftStorage[FindItemArrayPositionCraft(fromSlot)] = new Item();
            }

            return true;
        }

        if (fromSlot.Amount + toSlot.Amount > FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            int tempSum = fromSlot.Amount + toSlot.Amount;
            fromSlot.Amount = tempSum - FindObjectInDatabase(fromSlot).MaxStuckSize;
            toSlot.Amount = FindObjectInDatabase(toSlot).MaxStuckSize;
            return true;
        }

        return false;
    }

    public void SplitOneItemInventoryCraft(Item fromSlot, Item toSlot)
    {
        if (IsItemContainsInInventory(fromSlot))
        {
            if (fromSlot.ID == toSlot.ID || toSlot.ID == -1)
            {
                if (CheckForSplitStackInventoryCraft(fromSlot, toSlot, true))
                {
                    return;
                }

                Inventory[FindItemArrayPositionInventory(fromSlot)].Amount -= 1;
                CraftStorage[FindItemArrayPositionCraft(toSlot)] = new Item(FindObjectInDatabase(fromSlot));
            }
        }
        else
        {
            if (fromSlot.ID == toSlot.ID || toSlot.ID == -1)
            {
                if (CheckForSplitStackInventoryCraft(fromSlot, toSlot, false))
                {
                    return;
                }

                CraftStorage[FindItemArrayPositionCraft(fromSlot)].Amount -= 1;
                Inventory[FindItemArrayPositionInventory(toSlot)] = new Item(FindObjectInDatabase(fromSlot));
            }
        }

        ContainerUpdated?.Invoke();
    }

    private bool CheckForSplitStackInventoryCraft(Item fromSlot, Item toSlot, bool isItemInInventory)
    {
        if (toSlot.ID == -1)
        {
            return false;
        }

        if (!FindObjectInDatabase(toSlot).StackAble)
        {
            return false;
        }

        if (toSlot.ID != fromSlot.ID)
        {
            return false;
        }

        if (toSlot.Amount == FindObjectInDatabase(toSlot).MaxStuckSize)
        {
            return false;
        }

        if (isItemInInventory)
        {
            if (fromSlot.Amount == 1)
            {
                Inventory[FindItemArrayPositionInventory(fromSlot)] = new Item();
                CraftStorage[FindItemArrayPositionCraft(toSlot)].Amount += 1;
                ContainerUpdated?.Invoke();
                return true;
            }

            Inventory[FindItemArrayPositionInventory(fromSlot)].Amount -= 1;
            CraftStorage[FindItemArrayPositionCraft(toSlot)].Amount += 1;
        }
        else
        {
            if (fromSlot.Amount == 1)
            {
                CraftStorage[FindItemArrayPositionCraft(fromSlot)] = new Item();
                Inventory[FindItemArrayPositionInventory(toSlot)].Amount += 1;
                ContainerUpdated?.Invoke();
                return true;
            }

            Inventory[FindItemArrayPositionInventory(toSlot)].Amount += 1;
            CraftStorage[FindItemArrayPositionCraft(fromSlot)].Amount -= 1;
        }

        ContainerUpdated?.Invoke();
        return true;
    }

    private void CraftItem()
    {
        for (int i = 0; i < Database.CraftObjects.Length; i++)
        {
            if (IsCraftArraysEquals(CraftStorage, Database.CraftObjects[i].CraftItems))
            {
                ClearCraft();
                AddNewItem(Database.CraftObjects[i].CraftedItemObject);
            }
        }
    }

    private void ClearCraft()
    {
        for (int i = 0; i < CraftStorage.Length; i++)
        {
            CraftStorage[i] = new Item();
        }
    }

    private bool IsCraftArraysEquals(Item[] craftArray, Item[] objectArray)
    {
        for (int i = 0; i < craftArray.Length; i++)
        {
            if (craftArray[i].ID != objectArray[i].ID)
            {
                return false;
            }
        }

        return true;
    }

    public void SwapItemsInCraft(Item fromSlot, Item toSlot)
    {
        int pos1 = 0;
        int pos2 = 0;

        for (int i = 0; i < CraftStorage.Length; i++)
        {
            if (CraftStorage[i] == fromSlot)
            {
                pos1 = i;
            }

            if (CraftStorage[i] == toSlot)
            {
                pos2 = i;
            }
        }

        CraftStorage[pos1] = toSlot;
        CraftStorage[pos2] = fromSlot;

        ContainerUpdated?.Invoke();
    }

    [ContextMenu("Save")]
    public void Save()
    {
        savePath = string.Concat(gameObject.name, "Inventory.Save");
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }


    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }

        ContainerUpdated?.Invoke();
    }
}