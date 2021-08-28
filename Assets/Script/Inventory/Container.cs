using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Container : MonoBehaviour
{
    private string savePath;
    public InventoryDatabase Database;
    public SerializableContainer EquipmentContainer;
    public SerializableContainer InventoryContainer;
    public ItemObject test;
    public ItemObject test1;
    public Item[] InventoryStorage => InventoryContainer.Inventory;
    public Item[] EquipmentStorage => EquipmentContainer.Inventory;
    public event Action ContainerUpdated;

    public int FreeSlots
    {
        get { return InventoryStorage.Count(t => t.ID == -1); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(test);
            AddItem(test1);
            InventoryContainer.Inventory[0] = new Item(test1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log(EquipmentStorage[0]);
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
                InventoryStorage[GetStack(itemObject)].Amount += 1;
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
                InventoryStorage[GetStack(itemObject)].Amount += 1;
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
        
        for (int i = 0; i < InventoryStorage.Length; i++)
        {
            if (InventoryStorage[i].ID == temp.ID && InventoryStorage[i].Amount != itemObject.MaxStuckSize)
            {
                return true;
            }
        }

        return false;
    }
    
    //Получение ID неполного стака данного предмета
    private int GetStack(ItemObject itemObject)
    {
        Item temp = FindItemInInventory(itemObject);
        for (int i = 0; i < InventoryStorage.Length; i++)
        {
            if (InventoryStorage[i].ID == temp.ID && InventoryStorage[i].Amount != itemObject.MaxStuckSize)
            {
                return i;
            }
        }

        return -1;
    }
    
    private void AddNewItem(ItemObject itemObject)
    {
        InventoryStorage[GetFreeSlot()] = new Item(itemObject);
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < InventoryStorage.Length; i++)
        {
            if (InventoryStorage[i] == item)
            {
                InventoryStorage[i] = new Item();
            }
        }

        ContainerUpdated?.Invoke();
    }

    public void SwapItems(Item item1, Item item2)
    {
        int pos1 = 0;
        int pos2 = 0;

        for (int i = 0; i < InventoryStorage.Length; i++)
        {
            if (InventoryStorage[i] == item1)
            {
                pos1 = i;
            }

            if (InventoryStorage[i] == item2)
            {
                pos2 = i;
            }
        }

        InventoryStorage[pos1] = item2;
        InventoryStorage[pos2] = item1;

        ContainerUpdated?.Invoke();
    }

    private bool IsItemContains(ItemObject itemObject)
    {
        return InventoryStorage.Any(t => t.ID == itemObject.Data.ID);
    }

    private Item FindItemInInventory(ItemObject itemObject)
    {
        return InventoryStorage.FirstOrDefault(t => t.ID == itemObject.Data.ID);
    }

    private int GetFreeSlot()
    {
        for (int i = 0; i < InventoryStorage.Length; i++)
        {
            if (InventoryStorage[i].ID == -1)
            {
                return i;
            }
        }

        return -1;
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