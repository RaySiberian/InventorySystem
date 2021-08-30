using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Container : MonoBehaviour
{
    private string savePath;
    public ItemObjectsDatabase Database;
    public SerializableContainer InventoryContainer;
    public SerializableContainer EquipmentContainer;
    public ItemObject test;
    public ItemObject test1;
    public Item[] Inventory => InventoryContainer.Inventory;
    public Item[] Equipment => InventoryContainer.Equipment;
    public event Action ContainerUpdated;

    public int FreeSlots
    {
        get { return Inventory.Count(t => t.ID == -1); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(test);
            AddItem(test1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetEquipment(test1.Data);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log(Database.GetItemByID[Inventory[0].ID]);
        }
    }

    public void AddItem(ItemObject itemObject)
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
        if (FreeSlots != 0 && !IsItemContains(itemObject))
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

    public void SwapItems(Item item1, Item item2)
    {
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

    private bool IsItemContains(ItemObject itemObject)
    {
        return Inventory.Any(item => item.ID == itemObject.Data.ID);
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

    public void SetEquipmentFromInventory(Item equipmentItem, Item allItem)
    {
        SetEquipment(equipmentItem);
        RemoveItemFromInventory(equipmentItem);
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